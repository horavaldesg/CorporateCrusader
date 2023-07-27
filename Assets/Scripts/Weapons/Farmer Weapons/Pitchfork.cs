using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitchfork : SelectedWeapon
{
    public float knockBack = 35;
    private Collider2D _collider2D;

    protected override void Start()
    {
        TryGetComponent(out _collider2D);
        _collider2D.enabled = false;
        transform.parent = PlayerController.Instance.CurrentPlayerTransform();
        base.Start();
    }

    private void FixedUpdate()
    {
        var position = PlayerController.Instance.CurrentPlayerTransform().position;
        var gunRotation = PlayerController.Instance.GunRotation();

        // transform.position = position;
        transform.rotation = gunRotation;
    }

    protected override void Activate()
    {
        Stab();
        base.Activate();
    }

    private void Stab()
    {
        StartCoroutine(ReturnStab());
    }

    private IEnumerator ReturnStab()
    {
        var newPos = transform;
        ToggleCollider(true);
        transform.localPosition = newPos.right * 2;
        yield return new WaitForSeconds(1);
        ToggleCollider(false);
        transform.localPosition = Vector3.zero;
    }

    private void ToggleCollider(bool state)
    {
        _collider2D.enabled = state;
    }

    private void DamageEnemy(Rigidbody2D rb, Enemy enemy)
    {
        enemy.speed = 0;
        var differenceVector = enemy.transform.position - PlayerController.Instance.CurrentPlayerTransform().position;
        differenceVector = differenceVector.normalized * knockBack * 100;

        rb.AddForce(differenceVector, ForceMode2D.Force);
        enemy.SlowDownEnemy();
        enemy.TakeDamage(damage);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;
        Debug.Log("Collided with Enemy");
        var enemy = other.gameObject;
        enemy.TryGetComponent(out Rigidbody2D rb);
        enemy.TryGetComponent(out Enemy enemyComp);
        DamageEnemy(rb, enemyComp);
    }
}
