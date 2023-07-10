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
    [SerializeField] [Range(0, 1)] private float smoothMovement;
    [SerializeField] private float gunRotationSpeed;
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
    private const string BulletTag = "Bullet";
    private const string EnemyTag = "Enemy";

    private const int XpMaxCollection = 150;
    public List<GameObject> xpToCollect = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        _playerStats = Resources.Load<PlayerStats>("PlayerStats/PlayerStats");
        _baseHealth = _playerStats.health;
        _health = _baseHealth;
        TryGetComponent(out _rb);
        gunRotate.TryGetComponent(out gunRenderer);
        _controls = new PlayerControls();
        //Movement Player Input
        _controls.Player.Move.performed += tgb => { _move = tgb.ReadValue<Vector2>(); };
        _controls.Player.Move.canceled += tgb => { _move = Vector2.zero; };
        _controls.Player.Space.performed += async tgb => await CloudSaveManager.Instance.SaveData();
        _controls.Player.Fire.performed +=  tgb => WeaponManager.Instance.ChooseWeapon(0);
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
    }

    private void Move()
    {
        //Worlds Space of player
        var movement = transform.TransformDirection(_move);
        
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
        //playerSprite.flipX = _move.x < 0;
        RotateGun();
    }

    private void RotateGun()
    {
        var moveDir = new Vector2(-_move.y, _move.x);
        gunRenderer.flipY = moveDir.y < 0;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, moveDir);
        gunRotate.rotation = Quaternion.RotateTowards(gunRotate.transform.rotation, rotation,
            gunRotationSpeed * Time.deltaTime * 100);
    }

    [NotNull]
    public Transform CurrentPlayerTransform()
    {
        return gameObject.transform;
    }

    public PlayerDirection DirectionOfPlayer()
    {
        return _playerDirection;
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

        _health -= damage;
        healthBar.localScale = new Vector3(Mathf.Clamp(_health / _baseHealth, 0, 1), 1, 1);
    }

    private IEnumerator ResetXpCollected()
    {
        yield return new WaitForSeconds(0.15f);
        XpCollected = 0;
    }
}
