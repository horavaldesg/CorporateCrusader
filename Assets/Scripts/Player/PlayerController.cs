using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private Transform gunRotate;
    [SerializeField] private RectTransform healthBar;
    [SerializeField][Range(0,1)] private float smoothMovement;
    [SerializeField] private float gunRotationSpeed;
    private PlayerControls _controls;
    private Vector2 _move;
    private Rigidbody2D _rb;
    private Vector3 _dampSpeed;
    private Vector3 _currentVelocity;
    private SpriteRenderer playerSprite;
    private SpriteRenderer gunRenderer;

    private PlayerStats _playerStats;
    private float _baseHealth;
    private float _health;
    private const string BulletTag = "Bullet";
    private const string EnemyTag = "Enemy";
    
    private void Awake()
    {
        _playerStats = Resources.Load<PlayerStats>("PlayerStats/PlayerStats");
        _baseHealth = _playerStats.health;
        _health = _baseHealth;
        TryGetComponent(out _rb);
        TryGetComponent(out playerSprite);
        gunRotate.TryGetComponent(out gunRenderer);
        _controls = new PlayerControls();
        //Movement Player Input
        _controls.Player.Move.performed += tgb => { _move = tgb.ReadValue<Vector2>(); };
        _controls.Player.Move.canceled += tgb => { _move = Vector2.zero; };
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
            movement * playerSpeed,
            ref _dampSpeed,
            smoothMovement
        );

        //Moves Player
        _rb.velocity = _currentVelocity;
        if (_rb.velocity.magnitude is > -0.1f and < 0.1f) return;
        playerSprite.flipX = _move.x < 0;
        RotateGun();
    }

    private void RotateGun()
    {
        var moveDir = new Vector2(-_move.y, _move.x);
        gunRenderer.flipY = moveDir.y < 0;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, moveDir);
        gunRotate.rotation = Quaternion.RotateTowards(gunRotate.transform.rotation, rotation, gunRotationSpeed * Time.deltaTime * 100);
    }

    public void TakeDamage(float damage)
    {
        //Takes Damage
        _health -= damage;
        healthBar.localScale = new Vector3(_health / _baseHealth, 1, 1);
    }
}
