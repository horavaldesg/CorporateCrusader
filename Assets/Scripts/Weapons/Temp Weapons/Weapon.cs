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
    [SerializeField] public float bulletForce;
    [SerializeField] private Animator animator;
    
    private protected EnemyDetector EnemyDetector;
    private protected PlayerController PlayerController;

    private PlayerControls _playerControls;

    public float burstFire;
    public int amountOfBurstBullets;
    private int bulletsShot;
    private static readonly int Shooting = Animator.StringToHash(IsShooting);

    private const string IsShooting = "IsShooting";
    private protected void Awake()
    {
        _playerControls = new PlayerControls();
        EnemyDetector = transform.GetComponentInChildren<EnemyDetector>();
        //_playerControls.Player.Fire.performed += tgb => Shoot();
    }

    private protected void OnEnable()
    {
        _playerControls.Enable();
      //  EnemyDetector.StartCheck += CheckShoot;
       // EnemyDetector.EndShoot += EndShoot;
    }

    private protected void OnDisable()
    {
        _playerControls.Disable();
      //  EnemyDetector.StartCheck -= CheckShoot;
       // EnemyDetector.EndShoot -= EndShoot;
    }

    private protected void Start()
    {
        transform.root.TryGetComponent(out PlayerController);
        damage = PlayerController.damage;
        CheckShoot();
    }

    private void EndShoot()
    {
        StopAllCoroutines();
    }
    
    private void CheckShoot()
    {
        //if(EnemyDetector.enemies.Count == 0) return;
        StartCoroutine(bulletsShot <= amountOfBurstBullets ? Shoot() : BurstFire());
    }
    

    private IEnumerator BurstFire()
    {
        bulletsShot = 0;
        animator.SetBool(Shooting, false);
        yield return new WaitForSeconds(burstFire);
        StartCoroutine(Shoot());
    }

    private protected virtual IEnumerator Shoot()
    {
        bulletsShot++;
        animator.SetBool(Shooting, true);
        yield return new WaitForSeconds(fireRate);
        CheckShoot();
        //Shoots Gun
    }
}
