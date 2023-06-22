using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private Transform gunRotate;
    [SerializeField][Range(0,1)] private float smoothMovement;
    [SerializeField] private float gunRotationSpeed;
    private PlayerControls _controls;
    private Vector2 _move;
    private CharacterController _cc;
    private Vector3 _dampSpeed;
    private Vector3 _currentVelocity;
    private SpriteRenderer playerSprite;
    private SpriteRenderer gunRenderer;

    private const string BulletTag = "Bullet";
    private void Awake()
    {
        TryGetComponent(out _cc);
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

    private void Update()
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
        _cc.Move(_currentVelocity * Time.deltaTime);
        if (_cc.velocity.magnitude == 0) return;
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var whatHit = hit.collider.transform.tag;

        switch (whatHit)
        {
            case BulletTag:
                break;
        }
    }

    private void TakeDamage()
    {
        //Takes Damage
    }
}
