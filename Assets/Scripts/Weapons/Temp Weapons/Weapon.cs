using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private protected float fireRate;
    [SerializeField] private protected Transform barrelTransform;
    [SerializeField] private protected GameObject bullet;
    [SerializeField] private protected float damage;
    [SerializeField] private protected float bulletForce;

    private protected EnemyDetector EnemyDetector;
    private protected PlayerController PlayerController;

    private PlayerControls _playerControls;

    private protected void Awake()
    {
        _playerControls = new PlayerControls();
        EnemyDetector = transform.GetComponentInChildren<EnemyDetector>();
        
        //_playerControls.Player.Fire.performed += tgb => Shoot();
    }

    private protected void OnEnable()
    {
        _playerControls.Enable();
        EnemyDetector.StartCheck += CheckShoot;
        EnemyDetector.EndShoot += EndShoot;
    }

    private protected void OnDisable()
    {
        _playerControls.Disable();
        EnemyDetector.StartCheck -= CheckShoot;
        EnemyDetector.EndShoot -= EndShoot;
    }

    private protected void Start()
    {
        transform.root.TryGetComponent(out PlayerController);
        damage = PlayerController.damage;
    }

    private void EndShoot()
    {
        StopCoroutine(Shoot());
    }
    
    private void CheckShoot()
    {
        if(EnemyDetector.enemies.Count == 0) return;
        StartCoroutine(Shoot());
    }

    private protected virtual IEnumerator Shoot()
    {
        yield return new WaitForSeconds(fireRate);
        CheckShoot();
        //Shoots Gun
    }
}
