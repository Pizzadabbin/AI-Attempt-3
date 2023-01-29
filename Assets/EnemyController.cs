using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _damage;

    private Transform _playerTransform;
    private Rigidbody2D _rb;
    private void Start() {
        _playerTransform = FindObjectOfType<PlayerController>().transform;
        _rb = GetComponent<Rigidbody2D>();
    }


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
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerController>().DoDamage(_damage);
        }
    }

    public void DoDamage(Transform sender, int amount) {
        Debug.Log(gameObject.name + " got punched from: " + sender.name + " with: " + amount + " damage");
    }
}
