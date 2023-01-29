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
    [SerializeField] private float _punchRange;
    [SerializeField] private int _punchDamage;
    [SerializeField] private float _punchCooldown;
    [SerializeField] private Transform _punchStartPosition;
    [SerializeField] private LayerMask _punchableLayer;
    [SerializeField] private LineRenderer _punchLine;
    [SerializeField] private float _punchLineTime;
    private float _punchCooldownCounter = 0;

    [Header("User Interface")]
    [SerializeField] private Slider _healthbar;

    /**
     * Crisp Movement
     * Animations
     * Attack

     * Swords/weapons
     * monster
     * sprites
     */

    void Start() {
        _rb = GetComponent<Rigidbody2D>();

        _healthbar.maxValue = _health;
        _healthbar.value = _health;
    }

    void Update() {
        Movement();
        FlipPlayer();
        Jump();
        Dash();
        Punch();
        WallSliding();
        WallJumping();
    }
    #region Movement
    private float _horizontalInput;
    private void Movement() {
        if(_isWallJumping && _isDashing) {
            return;
        }

        _horizontalInput = Input.GetAxis("Horizontal");

        if(Input.GetKey(KeyCode.LeftShift)) {
            _horizontalInput *= _sprintModifier;
        }

        _rb.velocity = new Vector2(_horizontalInput * _movementSpeed, _rb.velocity.y);
    }

    private void Jump() {
        if(Input.GetButtonDown("Jump") && IsGrounded()) {
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
        }
        if(_isDashing) {
            _rb.velocity = new Vector2(_horizontalInput * _dashForce, _rb.velocity.y);
            _dashTimer -= Time.deltaTime;
            if(_dashTimer <= 0) {
                _isDashing = false;
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
        _punchCooldownCounter += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.X)) {
            if(_punchCooldownCounter >= _punchCooldown) {
                _punchCooldownCounter = 0;

                Vector3 dir = new Vector3(_isFacingRight ? 1 : -1, 0, 0);
                RaycastHit2D hit = Physics2D.Raycast(_punchStartPosition.position, dir, _punchRange, _punchableLayer);
                _punchLine.enabled = true;
                _punchLine.SetPosition(0, _punchStartPosition.position);
                _punchLine.SetPosition(1, dir * _punchRange + _punchStartPosition.position);
                CancelInvoke(nameof(RemovePunchLine));
                Invoke(nameof(RemovePunchLine), _punchLineTime);
                if(hit) {
                    if(hit.transform.TryGetComponent(out Punchable pa)) {
                        pa.GetPunched(transform, _punchDamage);
                    }
                }
            }
        }
    }
    private void RemovePunchLine() {
        _punchLine.enabled = false;
    }
    public void DoDamage(int damage) {
        _health -= damage;
        _healthbar.value = _health;
        if(_health < 0f) {
            Debug.LogWarning("Player died :(");
        }
    }
    
    #endregion
    #region Collsions
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.TryGetComponent(out Collectable c)) {
            c.Collect();
        }
    }
    #endregion

}