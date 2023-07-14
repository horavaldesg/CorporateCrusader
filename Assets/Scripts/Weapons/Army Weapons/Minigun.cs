using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Minigun : SelectedWeapon
{
    [SerializeField] private float bulletSpread;
    
    protected override void Activate()
    {
        Shoot();
        base.Activate();
    }

    private void FixedUpdate()
    {
        var gunPos = PlayerController.Instance.GunPosition();
        var gunRotation = PlayerController.Instance.GunRotation();
        transform.position = gunPos;
        transform.rotation = gunRotation;
    }

    private void Shoot()
    {
        var go = Instantiate(instantiatedObject);
        go.transform.position = PlayerController.Instance.GunPosition();
        go.transform.eulerAngles = RandomRotation();
    }

    private Vector3 RandomRotation()
    {
        return new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - Random.Range(-bulletSpread, bulletSpread));
    }
}
