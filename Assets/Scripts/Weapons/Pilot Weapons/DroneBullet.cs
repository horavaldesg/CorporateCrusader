using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class DroneBullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float bulletSpeed;
    public float damage;
    public float timeAlive;
    [HideInInspector] public SelectedWeapon.Attributes attributes;
    public Transform TargetPos
    {
        get;
        set;
    }

    private void Awake()
    {
       // Destroy(gameObject, timeAlive);
        TryGetComponent(out _rb);
    }

    private void FixedUpdate()
    {
        if(!TargetPos) return;
        var position = _rb.transform.position;
        var direction = (TargetPos.position - position).normalized;
        _rb.MovePosition(position + direction * (bulletSpeed * Time.fixedDeltaTime));
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy"))return;
        other.TryGetComponent(out Enemy enemy);
        enemy.TakeDamage(damage, attributes);
        Destroy(gameObject);
    }
}
