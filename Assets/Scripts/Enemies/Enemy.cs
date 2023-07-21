using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class Enemy : MonoBehaviour
{
    [Tooltip("Change in every script to name in Resources/EnemyStats/")]public string enemyName;
    
    private EnemyStats _enemyStats;
    private BossStats _bossStats;
    [HideInInspector] public float health;
    private float _baseHealth;
    [HideInInspector]  public float _damage;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private float amountOfXpToDrop;
    
    /// Use EnemyStats to change the values
    /// Can be set on awake
    /// Need to be public to be accessed by child scripts
    [HideInInspector] public float speed;
    [HideInInspector] public float baseSpeed;
    [HideInInspector] public bool takingDamage;
    
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    
    [SerializeField] private GameObject xpObject;

    [SerializeField] private bool isBoss;
    
    public Rigidbody2D _rb;
    private Collider2D _collider;
    private float _attackTime;
    private readonly List<Transform> _nearbyEnemies = new List<Transform>();
    private const float AvoidanceRadius = 0.15f;
    
    public virtual void Awake()
    {
        (isBoss ? (Action)LoadBossStats : LoadEnemyStats)();

        TryGetComponent(out _rb);
        TryGetComponent(out _collider);
    }

    private void LoadBossStats()
    {
        _bossStats = Resources.Load<BossStats>("EnemyStats/" + enemyName);
        health = _bossStats.health;
        speed = _bossStats.movespeed;
        _baseHealth = health;
        _damage = _bossStats.damage;
        _attackTime = _bossStats.cooldown;
    }

    private void LoadEnemyStats()
    {
        _enemyStats = Resources.Load<EnemyStats>("EnemyStats/" + enemyName);
        health = _enemyStats.health;
        speed = _enemyStats.speed;
        baseSpeed = speed;
        _baseHealth = health;
        _damage = _enemyStats.damage;
        _attackTime = _enemyStats.attackTime;
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

    public virtual void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
        //var differenceVector = playerPos - transform.position;
        /*if (differenceVector.magnitude < attackRange)
        {
            //Cooldown attack
            _attackTime += Time.deltaTime;
            if(_attackTime > attackCooldown)
                Attack();
        }*/
        
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
        takingDamage = true;
        health -= damage;
        healthBar.localScale = new Vector3(health / _baseHealth, 1, 1);
        health = Mathf.Clamp(health, 0, _baseHealth);
        if (health <= 0)
            EnemyDied();
    }

    public void TakeDamageWithCoolDown(float time, float damage)
    {
        TakeDamage(damage);
        StartCoroutine(WaitToTakeDamage(time, damage));
    }

    private IEnumerator WaitToTakeDamage(float time, float damage)
    {
        yield return new WaitForSeconds(time);
        TakeDamageWithCoolDown(time, damage);
    }

    public void StopTakingDamage()
    {
        takingDamage = false;
        StopAllCoroutines();
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

    protected virtual void Attack()
    {
        //Attacks
        _attackTime = 0;
    }
}