using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField][Range(0,1)] private float smoothMovement;
    private PlayerControls _controls;
    private Vector2 _move;
    private CharacterController _cc;
    private Vector3 _dampSpeed;
    private Vector3 _currentVelocity;
    
    private void Awake()
    {
        TryGetComponent(out _cc);
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
    }
}
