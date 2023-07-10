using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class NailGunProjectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private float speed = 5;
    public PlayerController.PlayerDirection playerDirection;
    

    private void Awake()
    {
        TryGetComponent(out _rigidbody2D);
    }

    private void FixedUpdate()
    {
        if (playerDirection == PlayerController.PlayerDirection.Left)
        {
            _rigidbody2D.velocity = -transform.right * speed;
        }
        else
        {
            _rigidbody2D.velocity = transform.right * speed;
        }
    }
}
