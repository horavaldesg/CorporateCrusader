using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockets : SelectedWeapon
{
    private Camera _mainCamera;
    public float velocity;
    
    protected override void Start()
    {
        _mainCamera = Camera.main;
        base.Start();
    }

    protected override void Activate()
    {
        LaunchRockets();
        base.Activate();
    }

    private void LaunchRockets()
    {
        var go = Instantiate(instantiatedObject);
        go.transform.position = RandomPos();
        go.TryGetComponent(out RocketProjectile rocketProjectile);
        rocketProjectile.damage = damage;
        rocketProjectile.moveSpeed = velocity;
        rocketProjectile.attributes = attribute;
    }

    private Vector3 RandomPos()
    {
        return _mainCamera.ScreenToWorldPoint(
            new Vector3(Random.Range(25, Screen.width - 25), -100, 10));
    }
}
