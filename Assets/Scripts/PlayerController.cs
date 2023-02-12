using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour {
    private Rigidbody2D _rb;

    [Header("Health Variables")]
    [SerializeField] private int _health;

    [Header("Movement Variables")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _sprintModifier;

    [Header("Jumping Variables")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Dash Variables")]
    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private GameObject _dashShadowPrefab;
    private float _dashCooldownCounter;
    private float _dashTimer;
    private bool _isDashing;

    [Header("Wall Variables")]
    [SerializeField] private float _wallSlidingSpeed;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private LayerMask _wallLayer;

    [SerializeField] private Vector2 _wallJumpForce;
    [SerializeField] private float _wallJumpTimer;
    [SerializeField] private float _wallJumpDuration;
    private float _wallJumpCounter;
    private float _wallJumpDirection;

    [Header("Punch Variables")]
    public int PunchDamage;
    [SerializeField] private float _punchCooldown;
    private float _punchCooldownCounter = 0;
    [HideInInspector] public bool IsAttacking;

    [Header("Block Variables")]
    [SerializeField] private GameObject _shieldObject;
    [SerializeField] [Tooltip("Divider; if you take 2, the damage is only half.")] private float _blockDamageDivider;
    [SerializeField] [Tooltip("Divider; if you take 2, the speed  is only half.")] private float _blockSpeedDivider;
    private bool _isBlocking;

    [Header("User Interface")]
    [SerializeField] private Slider _healthbar;

    public static PlayerController Instance;

    private bool IsInSpecialAction { get { return !IsGrounded() || _isDashing || _isWallJumping || _isWallSliding || IsAttacking; } }
    private Animator _animator;

    void Start() {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();

        _healthbar.maxValue = _health;
        _healthbar.value = _health;
        Instance = this;
    }

    void Update() {
        Movement();
        FlipPlayer();
        Jump();
        Dash();
        Punch();
        Block();
        WallSliding();
        WallJumping();
    }
    #region Movement
    private float _horizontalInput;
    private void Movement() {
        if(_isWallJumping || _isDashing || IsAttacking) {
            return;
        }

        _horizontalInput = Input.GetAxis("Horizontal");

        if(_isBlocking) {
            _horizontalInput /= _blockSpeedDivider;
        }

        if(Input.GetKey(KeyCode.LeftShift) && !_isBlocking) {
            _horizontalInput *= _sprintModifier;
        }

        if(_horizontalInput == 0) {
            _animator.Play("Idle");
        } else {
            _animator.Play("Run");
        }

        _rb.velocity = new Vector2(_horizontalInput * _movementSpeed, _rb.velocity.y);
    }

    private void Jump() {
        if(Input.GetButtonDown("Jump") && IsGrounded()) {
            _animator.Play("Jump");
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void Dash() {
        if(!_isDashing) {
            _dashCooldownCounter += Time.deltaTime;
        }
       
        if(Input.GetKeyDown(KeyCode.LeftControl) && _horizontalInput != 0 && _dashCooldownCounter >= _dashCooldown) { // && IsGrounded()
            _isDashing = true;
            _dashTimer = _dashDuration;
            _dashCooldownCounter = 0;
            foreach(GameObject g in _shadows) {
                if(g != null) {
                    Destroy(g);
                }
            }
            _shadows.Clear();
        }
        if(_isDashing) {
            _rb.velocity = new Vector2(_horizontalInput * _dashForce, _rb.velocity.y);
            _dashTimer -= Time.deltaTime;

            DashShadow();

            if(_dashTimer <= 0) {
                _isDashing = false;
            }
        }
    }

    private List<GameObject> _shadows = new();
    private const float _dashShadowCooldown = 0.05f;
    private float _dashShadowCounter = 0f;
    private void DashShadow() {
        _dashShadowCounter += Time.deltaTime;
        if(_dashShadowCounter < _dashShadowCooldown) {
            return;
        }
        _dashShadowCounter -= _dashShadowCooldown;
        GameObject newShadow = Instantiate(_dashShadowPrefab, transform.position, Quaternion.identity);
        Destroy(newShadow, 1f);
        _shadows.Insert(0, newShadow);

        newShadow.transform.localScale = new Vector3(-transform.localScale.x, 1, 1);

        float alpha = 0.8f;
        foreach(GameObject g in _shadows) {
            alpha -= 0.2f;
            DecreaseAllTransforms(g.transform, alpha);
        }

        void DecreaseAllTransforms(Transform parent, float alpha) {
            foreach(Transform t in parent) {
                DecreaseAllTransforms(t, alpha);
            }
            if(parent.TryGetComponent(out SpriteRenderer sr)) {
                sr.color = new Color(1f, 1f, 1f, alpha);
            }
        }
    }

    

    private bool _isFacingRight = true;
    private void FlipPlayer() {
        if(_isWallJumping) {
            return;
        }

        if(_isFacingRight && _horizontalInput < 0f || !_isFacingRight && _horizontalInput > 0f) {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool _isWallSliding = false;
    private void WallSliding() {
        if(IsWalled() && !IsGrounded() && _horizontalInput != 0) {
            _isWallSliding = true;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -_wallSlidingSpeed, float.MaxValue));
        } else {
            _isWallSliding = false;
        }
    }

    private bool _isWallJumping = false;
    private void WallJumping() {
        if(_isWallSliding) {
            _isWallJumping = false;
            _wallJumpDirection = -transform.localScale.x;
            _wallJumpCounter = _wallJumpTimer;

            CancelInvoke(nameof(StopWallJumping));
        } else {
            _wallJumpCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && _wallJumpCounter > 0f) {
            _isWallJumping = true;
            Debug.Log(_wallJumpDirection * _wallJumpForce.x);
            _rb.velocity = new Vector2(_wallJumpDirection * _wallJumpForce.x, _wallJumpForce.y);
            _wallJumpCounter = 0;

            if(transform.localScale.x != _wallJumpDirection) {
                _isFacingRight = !_isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            Invoke(nameof(StopWallJumping), _wallJumpDuration);
        }
    }

    private void StopWallJumping() {
        _isWallJumping = false;
    }
    private bool IsGrounded() {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }
    private bool IsWalled() {
        return Physics2D.OverlapCircle(_wallCheck.position, 0.2f, _wallLayer);
    }
    #endregion
    #region Fight Stuff
    private void Punch() {
        if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            IsAttacking = false;
        }

        _punchCooldownCounter += Time.deltaTime;
        if(Input.GetMouseButtonDown(0)) {
            if(_punchCooldownCounter >= _punchCooldown) {
                _punchCooldownCounter = 0;
                IsAttacking = true;
                _animator.Play("Attack");
                _rb.velocity = Vector2.zero;
            }
        }
    }
    private void StopAttacking() {

    }

    public void DoDamage(int damage) {
        if(_isBlocking) {
            _health -= (int) (damage / _blockDamageDivider);
        } else {
            _health -= damage;
        }

        _healthbar.value = _health;
        if(_health < 0f) {
            Debug.LogWarning("Player died :(");
        }
    }
    
    #endregion
    #region Collsions
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.TryGetComponent(out Collectable c)) {
            c.CollectObject();
        }
    }
    #endregion
    #region Powerups
    public void SetPowerup(float duration, float strength, PowerupType type) { 
        switch(type) {
            case PowerupType.Speed:
                _movementSpeed += strength;
                StartCoroutine(ResetPowerup(duration, strength, type));
                break;
            case PowerupType.AttackRange:
           //     _punchRange += strength;
           //     StartCoroutine(ResetPowerup(duration, strength, type));
                break;
            case PowerupType.Health:
                _health += (int)strength;
                _healthbar.value = _health;
                break;
        }
    }
    private IEnumerator ResetPowerup(float duration, float strength, PowerupType type) {
        yield return new WaitForSeconds(duration);
        switch(type) {
            case PowerupType.Speed:
                _movementSpeed -= strength;
                break;
            case PowerupType.AttackRange:
         //       _punchRange -= strength;
                break;
            default:
                break;
        }
    }
    #endregion

    #region Blocking
    private void Block() { 
        // right click
        if(Input.GetMouseButton(1) && !IsInSpecialAction) {
            _isBlocking = true;
        } else {
            _isBlocking = false;
        }

        _shieldObject.SetActive(_isBlocking);
    }
    #endregion
}