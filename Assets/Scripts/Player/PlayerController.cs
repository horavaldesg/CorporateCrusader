using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    #region variables

    public static PlayerController Instance;

    public static event Action<object[]> MoveStickAction;
    public static event Action<object[]> LookStickAction;

    public const string LeftStick = "LeftStick";
    public const string RightStick = "RightStick";
    public enum PlayerDirection
    {
        Right,
        Left,
        Stopped
    };

    public PlayerDirection playerDirection;

    #region Player Transforms

    //Player Transforms
    [Header("Player Transforms")]
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform armorBar;
    [SerializeField] private Transform gunRotate;
    [SerializeField] private Transform bodyTransform;
    
    #endregion

    #region Player Movement

    //Player Movement
    [Header("Player Movement")]
    [SerializeField] private bool useKeyboardInput;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerBaseSpeed;
    [SerializeField] private float gunRotationSpeed;
    [SerializeField] private float rotationThreshold;
    [SerializeField] [Range(0, 1)] private float smoothMovement;
    
    #endregion

    #region Private Fields
    
    //Private Fields
    private Vector2 _move;
    private Vector2 _look;
    private Vector3 _dampSpeed;
    private Vector3 _currentVelocity;
    private float _healTimer;
   
    #endregion

    #region Player Components

    //Player Components
    [Header("Player Components")]
    public CapsuleCollider2D playerCollider;
    [SerializeField] private List<SpriteRenderer> playerLegs;
    [SerializeField] private Sprite corgiLegBootSprite;
    private PlayerControls _controls;
    private Rigidbody2D _rb;
    private SpriteRenderer _gunRenderer;

    #endregion
    
    #region Player Stats

    //Player Stats
    [Header("Player Stats")]
    public float bulletSpeed;
    private PlayerStats _playerStats;
    private float _baseHealth;
    private float _health;
    private float _armor;
    private float _baseArmor;
    public float healAmount;
    [HideInInspector] public float healTime;
    [HideInInspector] public float damage;
    private float _pickupRadius;
    [HideInInspector] public float xPMultiplier;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public int coinMultiplier;
    private float _hatCoolDown;
    private const int XpMaxCollection = 350;
    
    #endregion

    #region Hat List
    
    //Hat List
    private List<Hat.ChosenHat> _chosenHats = new();
  
    #endregion

    #region Enemy Manager
    
    //Enemy Manager
    [HideInInspector] public List<Enemy> nearbyEnemies = new();
    
    #endregion

    #endregion
    
    private void Awake()
    {
        Instance = this;
        _playerStats = Resources.Load<PlayerStats>("PlayerStats/PlayerStats");
        TryGetComponent(out HatSelection hatSelection);
        _chosenHats = hatSelection.chosenHats;
        TryGetComponent(out playerCollider);
        //Player Stats
        damage = _playerStats.damage;
        _baseHealth = _playerStats.health;
        _health = _baseHealth;
        _armor = _playerStats.armor;
        _baseArmor = _armor;
        xPMultiplier = _playerStats.xpMultiplier;
        coinMultiplier = _playerStats.coinMultiplier;
        _pickupRadius = _playerStats.pickupRadius;
        attackSpeed = _playerStats.attackSpeed;
        bulletSpeed = _playerStats.bulletSpeed;
        _hatCoolDown = _playerStats.hatCooldown;
        healTime = _playerStats.regenTime;
        playerBaseSpeed = playerSpeed;
        ProjectileSizeMultiplier = 1;
        TryGetComponent(out _rb);
        gunRotate.TryGetComponent(out _gunRenderer);
        
        //Player Input
        _controls = new PlayerControls();
        //Movement Player Input
        _controls.Player.Move.performed += tgb => { _move = tgb.ReadValue<Vector2>(); };
        var param = new object[]{};
        _controls.Player.Move.performed += tgb =>
        {
            param = new object[] { true, LeftStick };
        };
        _controls.Player.Move.performed += tgb => {MoveStickAction?.Invoke(param); };
        _controls.Player.Move.canceled += _ => { _move = Vector2.zero; };
        _controls.Player.Move.canceled += _ => {   param = new object[] { false, LeftStick }; };
        _controls.Player.Move.canceled += tgb => {MoveStickAction?.Invoke(param); };

        if(useKeyboardInput)
        {
            _controls.Player.Look.performed += tgb => { param = new object[] { true, RightStick }; };
            _controls.Player.Look.performed += tgb => {LookStickAction?.Invoke(param); };
            _controls.Player.Move.performed += tgb => { _look = tgb.ReadValue<Vector2>(); };
            _controls.Player.Move.canceled += _ => { _look = Vector2.zero; };
            _controls.Player.Look.canceled += _ => {  param = new object[] { false, RightStick }; };
            _controls.Player.Look.canceled += tgb => {LookStickAction?.Invoke(param); };
        }
        
        _controls.Player.Look.performed += tgb => { _look = tgb.ReadValue<Vector2>(); };
       
        _controls.Player.Look.performed += tgb => { param = new object[] { true, RightStick }; };
        _controls.Player.Look.performed += tgb => {LookStickAction?.Invoke(param); };
        _controls.Player.Look.canceled += _ => { _look = Vector2.zero; };
        _controls.Player.Look.canceled += _ => {  param = new object[] { false, RightStick }; };
        _controls.Player.Look.canceled += tgb => {LookStickAction?.Invoke(param); };
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
        if (!HasArmor()) HealthCheck();
        if (_look.magnitude is > -0.1f and < 0.1f) return;
        RotateGun();
    }

    #region Player Movement
    
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
        playerDirection = _rb.velocity.normalized.x switch
        {
            > 0.1f => PlayerDirection.Right,
            < -0.1f => PlayerDirection.Left,
            _ => playerDirection
        };

        if (_rb.velocity.magnitude is > -0.1f and < 0.1f) return;

        bodyTransform.rotation = Quaternion.Euler(0, _move.x < 0 ? 180 : 0, 0);
    }

    private void RotateGun()
    {
        var moveDir = new Vector2(-_look.y, _look.x);
        _gunRenderer.flipY = moveDir.y < 0;
        var rotation = Quaternion.LookRotation(Vector3.forward, moveDir);
        gunRotate.rotation = Quaternion.Lerp(gunRotate.rotation, rotation, gunRotationSpeed * Time.deltaTime * 100);
    }
    
    #endregion

    #region Player Transforms
    
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

    #endregion
    
    #region Player Health Managers

    public void TakeDamage(float damageToTake)
    {
        //Takes Damage
        if (XpCollected >= XpMaxCollection)
        {
            StartCoroutine(ResetXpCollected());
        }

        (HasArmor() ? (Action<float>)TakeArmorDamage : TakeBaseDamage)(damageToTake);
    }
    
    private void HealthCheck()
    {
        (PlayerIsAlive() ? (Action)HealPlayer : PlayerDied)();
    }

    private void HealPlayer()
    {
        _healTimer += Time.deltaTime;
        if(_healTimer > healTime) return;
        _health += healAmount;
        _health = Mathf.Clamp(_health, 0, _baseHealth);
        healthBar.localScale = new Vector3(_health/_baseHealth, 1, 1);
        _healTimer = 0;
    }

    private void TakeArmorDamage(float damageToTake)
    {
        _armor -= damageToTake;
        //Debug.Log(_armor);
        armorBar.localScale = new Vector3(Mathf.Clamp(_armor / _baseArmor, 0, 1), 1, 1);
    }

    public void SlowDownPlayer(float slowDown)
    {
        playerSpeed = slowDown;
    }

    public void RestoreSpeed()
    {
        playerSpeed = playerBaseSpeed;
    }

    public float GetCurrentSpeed()
    {
        return playerSpeed;
    }

    public float GetBaseSpeed()
    {
        return playerSpeed;
    }

    public void RevivePlayer() => RestoreHealth(_baseHealth);

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

    private void TakeBaseDamage(float damageToTake)
    {
        _health -= damageToTake;
        _health = Mathf.Clamp(_health, 0, _baseHealth);
        healthBar.localScale = new Vector3(_health / _baseHealth, 1, 1);
    }

    private bool HasArmor()
    {
        return _armor > 0;
    }

    private bool PlayerIsAlive()
    {
        return _health > 0;
    }

    private void PlayerDied()
    {
        //death condition
        UIManager.Instance.PlayerDeath();
    }
    
    #endregion

    #region Player XP Managers
    
    private IEnumerator ResetXpCollected()
    {
        yield return new WaitForSeconds(0.15f);
        XpCollected = 0;
    }

    public bool CanCollect()
    {
        return XpCollected <= XpMaxCollection;
    }
    
    private int XpCollected { get; set; }
    
    #endregion

    #region Player Upgrades
    
    public void UpgradeBaseHealth(float newBaseHealth)
    {
        _baseHealth += newBaseHealth;
        RestoreHealth(_baseHealth);
    }

    public void UpgradeSpeed(float speed)
    {
        playerSpeed += speed;
        playerBaseSpeed = playerSpeed;
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
        RestoreArmor(_baseArmor);
    }

    public void IncreaseRegenTime(float regen)
    {
        healAmount += regen;
        healTime -= 0.2f;
        healTime = Mathf.Clamp(healTime, 0.25f, 2);
    }

    public void IncreaseCoinGain(int coinGain)
    {
        coinMultiplier += coinGain;
    }

    public void IncreaseDamage(float damageMulti)
    {
        damage += damageMulti;
    }

    public bool HasShield { get; set; }

    public bool HaseBinoculars { get; set; }

    public List<Hat.ChosenHat> ChosenHat()
    {
        return _chosenHats;
    }

    private float EnemyHealthReduction { get; set; }

    private float EnemyDamageReduction { get; set; }

    public void Shield(float damageReduction)
    {
        //Affects Nearby enemy damage
        if (!HasShield) return;
        EnemyDamageReduction = damageReduction;
    }

    public void Binoculars(float healthReduction)
    {
        if (!HaseBinoculars) return;
        EnemyHealthReduction = healthReduction;
    }

    public void BootSpurs()
    {
        foreach(SpriteRenderer sr in playerLegs) sr.sprite = corgiLegBootSprite;
    }

    public float NewEnemyHealth([NotNull] Enemy enemy)
    {
        var newHealth = enemy.health - EnemyHealthReduction;
        return newHealth;
    }

    public float NewEnemyDamage([NotNull] Enemy enemy)
    {
        var newDamage = enemy._damage - EnemyDamageReduction;
        return newDamage;
    }

    public void IncreaseAttackSpeed(float attackIncrease)
    {
        attackSpeed -= attackIncrease;
    }

    public void IncreaseProjectileSpeed(float speedIncrease)
    {
        bulletSpeed += speedIncrease;
    }

    public void AbilityCoolDown(float coolDown)
    {
        _hatCoolDown -= coolDown;
    }

    public void IncreaseProjectileSize(float sizeIncrease)
    {
        ProjectileSizeMultiplier += sizeIncrease;
    }

    public float ProjectileSizeMultiplier
    {
        get;
        set;
    }
    
    #endregion
}
