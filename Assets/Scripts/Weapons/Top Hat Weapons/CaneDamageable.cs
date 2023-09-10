using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CaneDamageable : MonoBehaviour
{
    private Cane _cane;

    private void Awake()
    {
        transform.parent.TryGetComponent(out _cane);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent(out Enemy enemy);
        if(!enemy) return;
        Debug.Log("Collided with Enemy");
        enemy.TakeDamage(_cane.damage, _cane.attribute);
    }
}
