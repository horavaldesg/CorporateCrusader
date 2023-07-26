using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Minigun : SelectedWeapon
{
    [SerializeField] private float bulletSpread;

    public float shootCoolDown;

    private void Awake()
    {
        transform.parent = null;
    }

    //360 rotation 
    // Change to place and then cooldown and then place again
    private void Update()
    {
        var rotation = transform.eulerAngles;
        rotation.z += Time.deltaTime * -90;
        transform.localEulerAngles = rotation;
    }

    private void Place()
    {
        StopAllCoroutines();
        var gunPos = PlayerController.Instance.CurrentPlayerTransform().position;
        transform.position = gunPos;
        Shoot();
    }

    protected override void Activate()
    {
        Place();
        base.Activate();
    }

    /*private void FixedUpdate()
    {
        var gunPos = PlayerController.Instance.GunPosition();
        var gunRotation = PlayerController.Instance.GunRotation();
        transform.position = gunPos;
        transform.rotation = gunRotation;
    }*/

    private void Shoot()
    {
        StartCoroutine(WaitToShoot());
    }

    private IEnumerator WaitToShoot()
    {
        var go = Instantiate(instantiatedObject);
        go.TryGetComponent(out Bullet bullet);
        bullet.Damage = damage;
        go.transform.position = transform.position;
        go.transform.eulerAngles = RandomRotation();
        yield return new WaitForSeconds(shootCoolDown);
        Shoot();
    }

    private Vector3 RandomRotation()
    {
        return new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - Random.Range(-bulletSpread, bulletSpread));
    }
}
