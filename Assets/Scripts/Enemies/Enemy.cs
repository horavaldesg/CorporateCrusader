using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private EnemyStats _enemyStats;
    public float health;
    
    private float _baseHealth;
    public float damage;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private float amountOfXpToDrop;
    [SerializeField] private GameObject xpObject;

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
        DropXp();
        EnemyDetector.Instance.EnemyKilled(gameObject);
    }

    private void DropXp()
    {
        for (int i = 0; i < amountOfXpToDrop; i++)
        {
            var radius = 0.75f;
            var randomPos = Random.insideUnitSphere * radius;
            randomPos += transform.position;

            var go = Instantiate(xpObject, randomPos, Quaternion.identity);
            go.transform.position = randomPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        other.gameObject.TryGetComponent(out PlayerController playerController);
        playerController.TakeDamage(damage);
    }
}
