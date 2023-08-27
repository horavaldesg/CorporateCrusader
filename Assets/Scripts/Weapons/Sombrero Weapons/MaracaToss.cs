using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MaracaToss : SelectedWeapon
{
    public float throwForce;
    
    protected override void Activate()
    {
        Throw();
        base.Activate();
    }

    private void Throw()
    {
        transform.eulerAngles = RandomRotation();
        var go = Instantiate(instantiatedObject);
        var playerPos = transform.position;

        go.transform.position = playerPos;
        go.transform.eulerAngles = transform.eulerAngles;
        go.TryGetComponent(out Rigidbody2D rb);
        go.TryGetComponent(out MaracaProjectile maracaProjectile);
        
        maracaProjectile.damage = damage;
        
        var force = transform.right * (throwForce * 100);
        
        rb.AddForce(force, ForceMode2D.Force);
        rb.AddTorque(10, ForceMode2D.Impulse);
    }

    private Vector3 RandomRotation()
    {
        return new Vector3(0,0, Random.Range(0,360));
    }
}
