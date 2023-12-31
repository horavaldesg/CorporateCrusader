using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TempAR : Weapon
{
    private protected override IEnumerator Shoot()
    {
        var go = Instantiate(
            bullet, 
            barrelTransform.position, 
            barrelTransform.rotation
        );
        go.TryGetComponent(out Bullet bulletComp);
        
        bulletComp.Damage = PlayerController.Instance.damage;
        bulletComp.whereToShoot = barrelTransform;
        bulletComp.speed = PlayerController.Instance.bulletSpeed;
        //bulletComp.rb.AddForce(barrelTransform.right.normalized * (bulletForce * Time.deltaTime * 1000));
        return base.Shoot();
    }
}
