using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour {
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _damage;
    [SerializeField] private int _health = 100;
    [SerializeField] private GameObject _enemyDeathParticle;
    [SerializeField] private Slider _enemyHealthBar;

    private Transform _playerTransform;
    private Rigidbody2D _rb;
    private void Start() {
        _enemyHealthBar.maxValue = _health;
        _enemyHealthBar.value = _health;
        _playerTransform = FindObjectOfType<PlayerController>().transform;
        _rb = GetComponent<Rigidbody2D>();
    }

    private Vector3 _eHBOffset = new Vector2(0, 0.75f);

    private void FixedUpdate() {
        float xMove = 1f;
        if(_playerTransform.position.x < transform.position.x) {
            xMove = -1;
        }
        float yMove = 1f;
        if(_playerTransform.position.y < transform.position.y) {
            yMove = -1;
        }
        _rb.velocity = new Vector2(xMove * _movementSpeed, yMove * _movementSpeed);
        _enemyHealthBar.transform.position = transform.position + _eHBOffset;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerController>().DoDamage(_damage);
        }
    }

    public void DoDamage(Transform sender, int amount) {
        _health -= amount;
        _enemyHealthBar.value = _health;
        _enemyHealthBar.gameObject.SetActive(true);
        if(_health <= 0) {
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(Instantiate(_enemyDeathParticle, transform.position, Quaternion.identity), 10);
            Destroy(gameObject);
        }
    }
}
