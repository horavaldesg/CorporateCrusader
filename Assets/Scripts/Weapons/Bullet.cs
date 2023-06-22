using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage { get; set; }
    public float speed;
    public Rigidbody2D rb;
    [SerializeField] private float timeAlive;
    [HideInInspector] public Transform whereToShoot;
    
    private void Awake()
    {
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
