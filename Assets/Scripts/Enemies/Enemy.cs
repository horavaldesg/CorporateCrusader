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
    [HideInInspector] public float _baseHealth;
    [HideInInspector]  public float _damage;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private float amountOfXpToDrop;
    
    /// Use EnemyStats to change the values
    /// Can be set on awake
    /// Need to be public to be accessed by child scripts
    [HideInInspector] public float speed;
    [HideInInspector] public float baseSpeed;
    [HideInInspector] public bool takingDamage;
    
    [HideInInspector] public float attackRange;
    [HideInInspector] public float attackCooldown;
    private EnemyStats.Weakness _weakness;
    
    private GameObject _xpObject;
    private GameObject _coinDrop;

    [SerializeField] private bool isBoss;
    private int _xpToAdd;
    private int _coinsToAdd;
    
    [HideInInspector] public Rigidbody2D _rb;
    private Collider2D _collider;
    [HideInInspector] public float attackTime;
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
        attackTime = _bossStats.cooldown;
    }

    private void LoadEnemyStats()
    {
        _enemyStats = Resources.Load<EnemyStats>("EnemyStats/" + enemyName);
        health = _enemyStats.health;
        speed = _enemyStats.speed;
        baseSpeed = speed;
        _baseHealth = health;
        _damage = _enemyStats.damage;
        attackTime = _enemyStats.attackTime;
        attackRange = _enemyStats.attackRange;
        attackCooldown = _enemyStats.attackCooldown;
        _xpToAdd = _enemyStats.xpToDrop;
        _xpToAdd = Random.Range(_xpToAdd, _xpToAdd + 5);
        _coinsToAdd = _enemyStats.coinsToDrop;
        _coinDrop = _enemyStats.coinObject;
        _xpObject = _enemyStats.xpObject;
        _weakness = _enemyStats.weakness;
    }

    private void Start()
    {
        _baseHealth = health;
        StartCoroutine(UpdateNearbyEnemies());
    }

    protected IEnumerator UpdateNearbyEnemies()
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

    protected virtual void Move()
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

    public void StopEnemy()
    {
        _rb.velocity = Vector2.zero;
    }

    public void TakeDamage(float damage, SelectedWeapon.Attributes attributes)
    {
        //Debug.Log(CheckWeakness(attributes));
        takingDamage = true;
        health -= damage;
        healthBar.localScale = new Vector3(health / _baseHealth, 1, 1);
        health = Mathf.Clamp(health, 0, _baseHealth);
        if (health <= 0)
            EnemyDied();
    }

    private bool CheckWeakness(SelectedWeapon.Attributes attributes)
    {
        return _weakness.ToString() == attributes.ToString();
    }

    public void TakeDamageWithCoolDown(float time, float damage, SelectedWeapon.Attributes attributes)
    {
        TakeDamage(damage, attributes);
        StartCoroutine(WaitToTakeDamage(time, damage, attributes));
    }

    private IEnumerator WaitToTakeDamage(float time, float damage, SelectedWeapon.Attributes attributes)
    {
        yield return new WaitForSeconds(time);
        TakeDamageWithCoolDown(time, damage, attributes);
    }

    public void StopTakingDamage()
    {
        takingDamage = false;
        StopAllCoroutines();
    }

    protected virtual void EnemyDied()
    {
        DropXp();
        EnemyDetector.Instance.EnemyKilled(gameObject);
    }

    private void DropXp()
    {
        var randomNum = Random.Range(0, 11);
        if(randomNum % 2 == 0) DropCoins();
        
        for (var i = 0; i < amountOfXpToDrop; i++)
        {
            var randomPos = RandomCirclePos();
            randomPos += transform.position;
            var go = Instantiate(_xpObject, randomPos, Quaternion.identity);
            go.transform.position = randomPos;
            go.TryGetComponent(out XpCollider xpCollider);
            xpCollider.xpToAdd = _xpToAdd;
        }
    }

    private Vector3 RandomCirclePos()
    {
        var radius = 0.75f;
        return Random.insideUnitSphere * radius;
    }

    private void DropCoins()
    {
        var go = Instantiate(_coinDrop);
        var randomPos = RandomCirclePos();
        randomPos += transform.position;
        go.transform.position = randomPos;
        go.TryGetComponent(out CoinCollider coinCollider);
        coinCollider.coinsToAdd = _coinsToAdd;
    }

    public void SlowDownEnemy()
    {
        StartCoroutine(SpeedUp());
    }

    private IEnumerator SpeedUp()
    {
        yield return new WaitForSeconds(1.5f);
        speed = baseSpeed;
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
        attackTime = 0;
    }
}