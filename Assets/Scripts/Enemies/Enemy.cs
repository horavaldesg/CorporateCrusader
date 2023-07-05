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
    private float _damage;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private float amountOfXpToDrop;
    [SerializeField] private float speed;
    [SerializeField] private GameObject xpObject;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private readonly List<Transform> _nearbyEnemies = new List<Transform>();
    private const float AvoidanceRadius = 0.15f;
    
    private void Awake()
    {
        _enemyStats = Resources.Load<EnemyStats>("EnemyStats/Enemy1Stats");
        health = _enemyStats.health;
        _baseHealth = health;
        _damage = _enemyStats.damage;
        TryGetComponent(out _rb);
        TryGetComponent(out _collider);
    }

    private void Start()
    {
        _baseHealth = health;
        StartCoroutine(UpdateNearbyEnemies());
    }

    private IEnumerator UpdateNearbyEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _nearbyEnemies.Clear();
            var colliders = Physics2D.OverlapCircleAll(transform.position, AvoidanceRadius);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.transform != transform)
                {
                    _nearbyEnemies.Add(collider.transform);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
        var movement = (Vector2)(playerPos - transform.position).normalized;
        foreach (var enemy in _nearbyEnemies)
        {
            if (!enemy) continue;
            Vector2 avoidanceVector = (transform.position - enemy.position).normalized;
            movement += avoidanceVector;
        }

        if (_nearbyEnemies.Count > 0)
        {
            movement /= _nearbyEnemies.Count;
        }

        _rb.velocity = movement.normalized * speed;
    }

    public virtual void OnDestroy()
    {
        EnemySpawner.Instance.RemoveEnemyFromList(gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.localScale = new Vector3(health / _baseHealth, 1, 1);
        health = Mathf.Clamp(health, 0, _baseHealth);
        if (health <= 0)
            EnemyDied();
    }

    private void EnemyDied()
    {
        DropXp();
        EnemyDetector.Instance.EnemyKilled(gameObject);
    }

    private void DropXp()
    {
        for (var i = 0; i < amountOfXpToDrop; i++)
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
        if (!other.CompareTag("Player")) return;
        other.gameObject.TryGetComponent(out PlayerController playerController);
        playerController.TakeDamage(_damage);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.gameObject.TryGetComponent(out PlayerController playerController);
        playerController.TakeDamage(_damage);
    }

    public virtual void Attack()
    {
    }
}