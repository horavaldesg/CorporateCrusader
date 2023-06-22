using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;

    private float _baseHealth;

   [SerializeField] private RectTransform healthBar;
    
    private void Start()
    {
        _baseHealth = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.localScale = new Vector3(health / _baseHealth, 1, 1);
        health = Mathf.Clamp(health, 0, _baseHealth);
    }

    private void Update()
    {
        if (!(health <= 0)) return;
        EnemyDetector.Instance.enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
