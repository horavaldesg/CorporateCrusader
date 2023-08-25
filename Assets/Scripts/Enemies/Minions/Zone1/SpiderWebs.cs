using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(
    typeof(Rigidbody2D),
    typeof(CircleCollider2D))]
public class SpiderWebs : MonoBehaviour
{
    private Rigidbody2D _rb;
    [HideInInspector] public float damage;
    public float force;
    
    private void Awake()
    {
        TryGetComponent(out _rb);
        Destroy(gameObject, 5);
    }
    

    public void Shoot(Transform spiderPos)
    {
        var shootDirection = PlayerController.Instance.CurrentPlayerTransform().position - spiderPos.position;

        var shootForce = shootDirection * (force * Time.deltaTime);
        _rb.AddForce(shootForce * 100);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent(out PlayerController playerController);
        if (!playerController)return;
        playerController.TakeDamage(damage);
        Destroy(gameObject);
    }
}
