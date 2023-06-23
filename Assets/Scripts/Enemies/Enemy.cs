using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    private EnemyStats _enemyStats;
    public float health;

    private float _baseHealth;
    public float damage;
    [SerializeField] private RectTransform healthBar;

    private void Awake()
    {
        _enemyStats = Resources.Load<EnemyStats>("EnemyStats/Enemy1Stats");
        health = _enemyStats.health;
        _baseHealth = health;
        damage = _enemyStats.damage;
    }

    private void Start()
    {
        _baseHealth = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.localScale = new Vector3(health / _baseHealth, 1, 1);
        health = Mathf.Clamp(health, 0, _baseHealth);
        if(health <= 0)
            EnemyDied();
    }

    private void EnemyDied()
    {
        EnemyDetector.Instance.EnemyKilled(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        other.gameObject.TryGetComponent(out PlayerController playerController);
        playerController.TakeDamage(damage);
    }
}
