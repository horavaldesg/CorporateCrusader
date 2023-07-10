using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class NailGun : SelectedWeapon
{
    public int distanceFromPlayer;
    private int _baseDistance;

    private float _angle;
    private float _baseAngle;

    protected override void Start()
    {
        _angle = 0.8f;
        _baseAngle = _angle;
        _baseDistance = distanceFromPlayer;

        base.Start();
    }
    private void InstantiateObjectsOnHalfCircle()
    {
        var numberOfObjects = 10; // Number of objects to instantiate
        var centerPoint = transform.position + (transform.right + transform.up).normalized * 0.5f;
        var angleIncrement = 180f / numberOfObjects;
        for (var i = 0; i < numberOfObjects; i++)
        {
            var angle = transform.eulerAngles.z + (i * angleIncrement);
            var radianAngle = Mathf.Deg2Rad * angle;
            var x = centerPoint.x + Mathf.Sin(radianAngle) * 3;
            var y = centerPoint.y + Mathf.Cos(radianAngle) * -3;
            var z = centerPoint.z;
            var position = new Vector3(x, y, z);
            var go = Instantiate(instantiatedObject);
            go.transform.position = position;
            go.transform.rotation = transform.rotation;
            Destroy(go, 2);
        }
    }
    
    protected override void Activate()
    {
        InstantiateObjectsOnHalfCircle();
           // _angle -= 0.3f;
       

        base.Activate();
    }

    protected override IEnumerator ActivateCoolDown()
    {
        _angle = _baseAngle;
        return base.ActivateCoolDown();
    }

    private void InstantiateObjects()
    {
        var go = Instantiate(instantiatedObject);
        if (PlayerController.Instance.DirectionOfPlayer() == PlayerController.PlayerDirection.Left)
        {
            distanceFromPlayer = -_baseDistance;
        }
        else
        {
            distanceFromPlayer = _baseDistance;
        }

        go.TryGetComponent(out NailGunProjectile nailGunProjectile);
        nailGunProjectile.playerDirection = PlayerController.Instance.DirectionOfPlayer();

        var randomPoint = RandomPointOnXZCircle(PlayerController.Instance.CurrentPlayerTransform().position, distanceFromPlayer);
        go.transform.position = randomPoint;
        go.transform.rotation = quaternion.Euler(0,0,_angle);
        Destroy(go, 3);
    }

    Vector3 RandomPointOnXZCircle(Vector3 center, float radius) {
        return center + new Vector3(Mathf.Cos(_angle), Mathf.Sin(_angle),1 ) * radius;
    }
}
