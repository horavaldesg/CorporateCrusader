using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Minigun : SelectedWeapon
{
    [SerializeField] private float bulletSpread;

    private PlayerControls _controller;
    
    private void OnEnable()
    {
        _controller.Enable();
    }

    private void OnDisable()
    {
        _controller.Disable();
    }

    protected override void Awake()
    {
        var gunPos = PlayerController.Instance.GunPosition();
        var gunRotation = PlayerController.Instance.GunRotation();
        transform.position = gunPos;
        transform.rotation = gunRotation;
        base.Awake();
    }
    //360 rotation 
    // Change to place and then cooldown and then place again

    private void Place()
    {
        var gunPos = PlayerController.Instance.GunPosition();
        var gunRotation = PlayerController.Instance.GunRotation();
        transform.position = gunPos;
        transform.rotation = gunRotation;
    }

    protected override void Activate()
    {
        Shoot();
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
        var go = Instantiate(instantiatedObject);
        go.transform.position = transform.position;
        go.transform.eulerAngles = RandomRotation();
    }

    private Vector3 RandomRotation()
    {
        return new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - Random.Range(-bulletSpread, bulletSpread));
    }
}
