using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JerryCan : SelectedWeapon
{
    [SerializeField] private float throwForce;
    
    protected override void Activate()
    {
        Throw();
        base.Activate();
    }

    private void Throw()
    {
        transform.eulerAngles = GetRandomRotation();
        var go = Instantiate(instantiatedObject);
        go.transform.position = transform.position;
        go.transform.eulerAngles = transform.eulerAngles;
        go.TryGetComponent(out Rigidbody2D rb);
        go.TryGetComponent(out JerryCanThrowable jerryCanThrowable);
        jerryCanThrowable.Damage = damage;
        jerryCanThrowable.attributes = attribute;
        Vector3 forceAngle = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + 1, transform.position.z) - transform.position;
        rb.AddForce(forceAngle * (throwForce * 100));
    }

    private void FixedUpdate()
    {
        var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
        transform.position = playerPos;
    }

    private Vector3 GetRandomRotation()
    {
        return new Vector3(0, 0, Random.Range(-120, 120));
    }
}
