using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class NailGun : SelectedWeapon
{
    public float radius;
    public int amountOfObjectsToSpawn;
    public float distanceFromPlayer;
    

    private void InstantiateObjectsOnHalfCircle()
    {
        transform.eulerAngles = PlayerController.Instance.GunRotation().eulerAngles;
        var numberOfObjects = amountOfObjectsToSpawn; // Number of objects to instantiate
        var centerPoint = transform.position + (transform.right + transform.up).normalized * distanceFromPlayer;
        var angleIncrement = 180f / numberOfObjects;
        var initialRotation = PlayerController.Instance.GunRotation().eulerAngles.z;

        for (var i = 0; i < numberOfObjects; i++)
        {
            var angle = initialRotation + (i * angleIncrement);

         //   var angle = transform.eulerAngles.z + (i * angleIncrement);
            var radianAngle = Mathf.Deg2Rad * angle;
            var x = centerPoint.x + Mathf.Sin(radianAngle) * radius;
            var y = centerPoint.y + Mathf.Cos(radianAngle) * -radius;
            var z = centerPoint.z;
            var position = new Vector3(x, y, z);
            var go = Instantiate(instantiatedObject);
            go.transform.position = position;
            var rotationAngle = angle - 90f;  // Adding 90 degrees to face outward

            go.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
            go.TryGetComponent(out NailGunProjectile nailGunProjectile);
            nailGunProjectile.damage = damage;
            Destroy(go, 2);
        }
    }
    
    protected override void Activate()
    {
        InstantiateObjectsOnHalfCircle();
        base.Activate();
    }
}
