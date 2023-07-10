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

    protected override void Activate()
    {
        for (var i = 0; i < 6; i++)
        {
            InstantiateObjects();
            _angle -= 0.3f;
        }
        
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
