using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MaracaProjectile : MonoBehaviour
{
    [HideInInspector] public float damage;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Enemy"))return;
        other.TryGetComponent(out Enemy enemy);
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
