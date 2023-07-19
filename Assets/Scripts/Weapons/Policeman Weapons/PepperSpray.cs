using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PepperSpray : SelectedWeapon
{
    public float radius;
    public float slowDownMultiplier;

    protected override void Awake()
    {
        transform.localScale = new Vector3(radius,radius, 1);
    }

    public void IncreaseRadius(float r)
    {
        radius += r;
        transform.localScale = new Vector3(radius,radius, 1);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent(out Enemy enemy);
        if (!enemy) return;
        SlowDownEnemy(enemy);
        if(enemy.takingDamage) return;
        enemy.TakeDamageWithCoolDown(coolDown, damage);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        other.TryGetComponent(out Enemy enemy);
        if (!enemy) return;
        RestoreEnemySpeed(enemy);
    }

    private void SlowDownEnemy(Enemy enemy)
    {
        enemy.speed -= slowDownMultiplier;
    }

    private void RestoreEnemySpeed(Enemy enemy)
    {
        enemy.speed = enemy.baseSpeed;
        enemy.StopTakingDamage();
    }
}
