using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float Damage { get; set; }
    public float speed;
    public Rigidbody2D rb;
    public float timeAlive;
    [HideInInspector] public Transform whereToShoot;
    
    protected virtual void Awake()
    {
        TryGetComponent(out rb);
        Destroy(gameObject, timeAlive);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy"))return;
        other.TryGetComponent(out Enemy enemy);
        enemy.TakeDamage(Damage);
        Destroy(gameObject);
    }
}
