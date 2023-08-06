using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public enum PlayerDirection
    {
        Right,
        Left,
        Stopped
    };

    public PlayerDirection _playerDirection;

    [SerializeField] private float playerSpeed;
    [SerializeField] private Transform gunRotate;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform armorBar;
    [SerializeField] [Range(0, 1)] private float smoothMovement;
    [SerializeField] private float gunRotationSpeed;
    [SerializeField] private float rotationThreshold;
    [SerializeField] private Transform bodyTransform;
    private PlayerControls _controls;
    private Vector2 _move;
    private Rigidbody2D _rb;
    private Vector3 _dampSpeed;
    private Vector3 _currentVelocity;
    private SpriteRenderer gunRenderer;

    private PlayerStats _playerStats;
    private float _baseHealth;
    private float _health;
    private float _armor;
    private float _baseArmor;
    private const string BulletTag = "Bullet";
    private const string EnemyTag = "Enemy";
    public float healAmount;
    [HideInInspector] public float healTime;
    [HideInInspector] public float damage;
    private float _pickupRadius;
    [HideInInspector] public float xPMultiplier;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public int coinMultiplier;
    private const int XpMaxCollection = 150;
    public List<GameObject> xpToCollect = new List<GameObject>();
    public CapsuleCollider2D playerCollider;
    [HideInInspector] public List<Enemy> nearbyEnemies = new();

    private void Awake()
    {
        Instance = this;
        _playerStats = Resources.Load<PlayerStats>("PlayerStats/PlayerStats");
        TryGetComponent(out playerCollider);
        damage = _playerStats.damage;
        _baseHealth = _playerStats.health;
        _health = _baseHealth;
        _armor = _playerStats.armor;
        _baseArmor = _armor;
        xPMultiplier = _playerStats.xpMultiplier;
        coinMultiplier = _playerStats.coinMultiplier;
        _pickupRadius = _playerStats.pickupRadius;
        attackSpeed = _playerStats.attackSpeed;
        TryGetComponent(out _rb);
        gunRotate.TryGetComponent(out gunRenderer);
        _controls = new PlayerControls();
        //Movement Player Input
        _controls.Player.Move.performed += tgb => { _move = tgb.ReadValue<Vector2>(); };
        _controls.Player.Move.canceled += tgb => { _move = Vector2.zero; };
        //  _controls.Player.Space.performed += async tgb => await CloudSaveManager.Instance.SaveData();
        // _controls.Player.Fire.performed +=  tgb => WeaponManager.Instance.ChooseWeapon(0);
    }

    private void Start()
    {
        XpPickup.Instance.IncreasePickupRadius(_pickupRadius);
    }

    private void OnEnable()
    {
        //Enables Player Input
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        //Disables Player Input
        _controls.Player.Disable();
    }

    private void FixedUpdate()
    {
        //Move Function
        Move();
        if (!HasArmor()) HealPlayer();
        if (_move.magnitude is > -0.1f and < 0.1f) return;
        RotateGun();
    }

    private void Move()
    {
        //Worlds Space of player
        var movement = _move.magnitude < rotationThreshold ? Vector3.zero : transform.TransformDirection(_move);

        //Speed damp
        _currentVelocity = Vector3.SmoothDamp
        (
            _currentVelocity,
            movement * playerSpeed, //Clamp Speed to whatever de-buff
            ref _dampSpeed,
            smoothMovement
        );

        //Moves Player
        _rb.velocity = _currentVelocity;
        _playerDirection = _rb.velocity.normalized.x switch
        {
            > 0.1f => PlayerDirection.Right,
            < -0.1f => PlayerDirection.Left,
            _ => _playerDirection
        };

        if (_rb.velocity.magnitude is > -0.1f and < 0.1f) return;

        bodyTransform.rotation = Quaternion.Euler(0, _move.x < 0 ? 180 : 0, 0);
    }

    private void RotateGun()
    {
        var moveDir = new Vector2(-_move.y, _move.x);
        gunRenderer.flipY = moveDir.y < 0;
        var rotation = Quaternion.LookRotation(Vector3.forward, moveDir);
        gunRotate.rotation = Quaternion.RotateTowards(gunRotate.transform.rotation, rotation,
            gunRotationSpeed * Time.deltaTime * 100);
    }

    public Transform GunTransform()
    {
        return gunRotate;
    }

    public Quaternion GunRotation()
    {
        return gunRotate.rotation;
    }

    public Vector3 GunPosition()
    {
        return gunRotate.transform.position;
    }

    [NotNull]
    public Transform CurrentPlayerTransform()
    {
        return gameObject.transform;
    }

    public Vector2 DirectionOfPlayer()
    {
        return _rb.velocity;
    }

    private int XpCollected { get; set; }

    public bool CanCollect()
    {
        return XpCollected <= XpMaxCollection;
    }

    public void TakeDamage(float damage)
    {
        //Takes Damage
        if (XpCollected >= XpMaxCollection)
        {
            StartCoroutine(ResetXpCollected());
        }

        (HasArmor() ? (Action<float>)TakeArmorDamage : TakeBaseDamage)(damage);
    }

    private void HealPlayer()
    {
        if (_health >= _baseHealth) return;
        _health += healAmount * Time.deltaTime;
        healthBar.localScale = new Vector3(Mathf.Clamp(_health / _baseHealth, 0, 1), 1, 1);
    }

    private void TakeArmorDamage(float damage)
    {
        _armor -= damage;
        Debug.Log(_armor);
        armorBar.localScale = new Vector3(Mathf.Clamp(_armor / _baseArmor, 0, 1), 1, 1);
    }

    private void RestoreHealth(float health)
    {
        _health = health;
        healthBar.localScale = new Vector3(Mathf.Clamp(_health / _baseHealth, 0, 1), 1, 1);
    }

    private void RestoreArmor(float armor)
    {
        _armor = armor;
        armorBar.localScale = new Vector3(Mathf.Clamp(_armor / _baseArmor, 0, 1), 1, 1);
    }

    private void TakeBaseDamage(float damage)
    {
        _health -= damage;
        healthBar.localScale = new Vector3(Mathf.Clamp(_health / _baseHealth, 0, 1), 1, 1);
    }

    private bool HasArmor()
    {
        return _armor > 0;
    }


    private IEnumerator ResetXpCollected()
    {
        yield return new WaitForSeconds(0.15f);
        XpCollected = 0;
    }

    public void UpgradeBaseHealth(float newBaseHealth)
    {
        _baseHealth += newBaseHealth;
        RestoreHealth(_baseHealth);
    }

    public void UpgradeSpeed(float speed)
    {
        playerSpeed += speed;
    }

    public void IncreaseRange(float radius)
    {
        _pickupRadius += radius;
        XpPickup.Instance.IncreasePickupRadius(_pickupRadius);
    }

    public void IncreaseXpGain(float xp)
    {
        xPMultiplier += xp;
    }

    public void IncreaseArmor(float armor)
    {
        _baseArmor += armor;
    }

    public void IncreaseRegenTime(float regen)
    {
        healAmount += regen;
    }

    public void IncreaseCoinGain(int coinGain)
    {
        coinMultiplier += coinGain;
    }

    public void IncreaseDamage(float damageMulti)
    {
        damage += damageMulti;
    }

    public bool HasShield
    {
        get;
        set;
    }
    
    public bool HaseBinoculars
    {
        get;
        set;
    }

    private float EnemyHealthReduction
    {
        get;
        set;
    }
    
     private float EnemyDamageReduction
    {
        get;
        set;
    }
    
    public void Shield(float damageReduction)
    {
        //Affects Nearby enemy damage
        if(!HasShield) return;
        EnemyDamageReduction = damageReduction;
    }

    public void Binoculars(float healthReduction)
    {
        if(!HaseBinoculars) return;
        EnemyHealthReduction = healthReduction;
    }

    public float NewEnemyHealth(Enemy enemy)
    {
        var newHealth = enemy.health - EnemyHealthReduction;
        return newHealth;
    }

    public float NewEnemyDamage(Enemy enemy)
    {
        var newDamage = enemy._damage - EnemyDamageReduction;
        return newDamage;
    }

    public void IncreaseAttackSpeed(float attackIncrease)
    {
        attackSpeed -= attackIncrease;
    }
}
