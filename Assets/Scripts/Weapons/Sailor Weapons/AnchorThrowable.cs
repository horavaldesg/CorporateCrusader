using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(
    typeof(Rigidbody2D),
    typeof(CircleCollider2D))]
public class AnchorThrowable : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float damage;

    private void Start()
    {
        TryGetComponent(out CircleCollider2D circleCollider2D);
        circleCollider2D.isTrigger = true;
        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy"))return;
        other.TryGetComponent(out Enemy enemy);
        enemy.TakeDamage(damage);
    }
}
