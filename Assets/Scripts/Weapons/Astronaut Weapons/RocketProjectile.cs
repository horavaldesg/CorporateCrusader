using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class RocketProjectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _collider2D;
    [HideInInspector] public float damage;
    [HideInInspector] public float moveSpeed;
    
    private void Start()
    {
        transform.parent = null;
        TryGetComponent(out _rigidbody2D);
        TryGetComponent(out _collider2D);
        Destroy(gameObject, 25);
    }

    private void FixedUpdate()
    {
        var velocity = transform.up * (moveSpeed);
        _rigidbody2D.AddForce(velocity);

        //make rocket jitter left and right
        Vector2 offset = new Vector2(Random.Range(-0.1f, 0.1f), 0);
        transform.GetChild(0).localPosition = offset;
        _collider2D.offset = offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy"))return;
        other.TryGetComponent(out Enemy enemy);
        enemy.TakeDamage(damage);
    }
}
