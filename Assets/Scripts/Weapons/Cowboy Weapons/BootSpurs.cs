using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class BootSpurs : SelectedWeapon
{
    public float knockBack;

    protected override void Start()
    {
        TryGetComponent(out CapsuleCollider2D capsuleCollider2D);
        var playerCollider = PlayerController.Instance.playerCollider;
        capsuleCollider2D.direction = playerCollider.direction;
        capsuleCollider2D.offset = playerCollider.offset;
        capsuleCollider2D.size = playerCollider.size;
    }
    

    private void DamageEnemy([NotNull] Rigidbody2D rb, [NotNull] Enemy enemy)
    {
        enemy.speed = 0;
        var differenceVector = enemy.transform.position - PlayerController.Instance.CurrentPlayerTransform().position;
        differenceVector = differenceVector.normalized * knockBack * 100;
        
        rb.AddForce(differenceVector, ForceMode2D.Force);
        enemy.SlowDownEnemy();
        enemy.TakeDamage(damage);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.CompareTag("Enemy")) return;
        Debug.Log("Collided with Enemy");
        var enemy = other.gameObject;
        enemy.TryGetComponent(out Rigidbody2D rb);
        enemy.TryGetComponent(out Enemy enemyComp);
        DamageEnemy(rb, enemyComp);
    }
}
