using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitchfork : SelectedWeapon
{
    public float knockBack = 35;
    public float range = 1.5f;
    
    private Collider2D _collider2D;
    private bool isStabbing = false;

    protected override void Start()
    {
        TryGetComponent(out _collider2D);
        _collider2D.enabled = false;
        transform.parent = PlayerController.Instance.CurrentPlayerTransform();
        base.Start();
    }

    private void FixedUpdate()
    {
        if(isStabbing) return;

        //rotate pitchfork in direction player is facing
        var gunRotation = PlayerController.Instance.GunRotation();
        transform.rotation = Quaternion.Euler(0, 0, gunRotation.eulerAngles.z);
    }

    protected override void Activate()
    {
        Stab();
        base.Activate();
    }

    private void Stab()
    {
        StartCoroutine(StabAnimation());
    }

    private IEnumerator StabAnimation()
    {
        float t = 0;
        float xPos = 0;
        ToggleCollider(false);
        isStabbing = true;

        while(t < coolDown)
        {
            t += Time.deltaTime; //increment time

            if(t < coolDown / 9)
            {
                //pull back the pitchfork before stabbing
                xPos = Mathf.Lerp(0, -0.5f, t / (coolDown/ 9));
            }
            else if(t < coolDown / 9 * 2)
            {
                //stab pitchfork
                xPos = Mathf.Lerp(-0.5f, range, (t - (coolDown / 9)) / (coolDown / 9));
                ToggleCollider(true);
            }
            else if(t < coolDown / 9 * 4)
            {
                //retract pitchfork
                xPos = Mathf.Lerp(range, 0, (t - (2 * (coolDown / 9))) / (2 * (coolDown / 9)));
            }
            else
            {
                //allow pitchfork to rotate
                ToggleCollider(false);
                isStabbing = false;
            }

            var newPos = transform;
            transform.localPosition = newPos.right * xPos;
            yield return null;
        }
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
        enemy.TakeDamage(damage, attribute);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;
        //Debug.Log("Collided with Enemy");
        var enemy = other.gameObject;
        enemy.TryGetComponent(out Rigidbody2D rb);
        enemy.TryGetComponent(out Enemy enemyComp);
        DamageEnemy(rb, enemyComp);
    }
}
