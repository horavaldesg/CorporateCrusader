using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalDrone : SelectedWeapon
{ 
    [SerializeField] private float bulletSpeed;
    
    protected override void Activate()
    {
        TargetEnemy();
        base.Activate();
    }

    private void TargetEnemy()
    {
        if (!GetClosestEnemy()) return;
        var go = Instantiate(instantiatedObject);
        go.transform.position = transform.position;
        go.TryGetComponent(out DroneBullet droneBullet);
        droneBullet.bulletSpeed = bulletSpeed;
        droneBullet.damage = damage;
        droneBullet.timeAlive = 5;
        droneBullet.TargetPos = GetClosestEnemy();
    }
    

    private Transform GetClosestEnemy()
    {
        Transform closest = null;
        var minDist = Mathf.Infinity;
        var currentPos = PlayerController.Instance.CurrentPlayerTransform().position;
        foreach (var enemiesSpawned in EnemySpawner.Instance.enemiesSpawnedList)
        {
            if (!enemiesSpawned) return null;
            var dist = Vector3.Distance(enemiesSpawned.transform.position, currentPos);
            if (!(dist < minDist)) continue;
            closest = enemiesSpawned.transform;
            minDist = dist;
        }
        
        return closest;
    }
}
