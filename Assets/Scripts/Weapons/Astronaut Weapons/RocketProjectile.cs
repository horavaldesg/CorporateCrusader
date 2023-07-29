using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketProjectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    [HideInInspector] public float damage;
    [HideInInspector] public float moveSpeed;
    
    private void Start()
    {
        transform.parent = null;
        TryGetComponent(out _rigidbody2D);
        Destroy(gameObject, 25);
    }

    private void FixedUpdate()
    {
        var velocity = transform.up * (moveSpeed);
        _rigidbody2D.AddForce(velocity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy"))return;
        other.TryGetComponent(out Enemy enemy);
        enemy.TakeDamage(damage);
        //Destroy(gameObject);
    }
}
