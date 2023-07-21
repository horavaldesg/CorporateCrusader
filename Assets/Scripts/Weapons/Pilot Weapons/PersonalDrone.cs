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
        Debug.Log("Test");
        if (!GetClosestEnemy()) return;
        Debug.Log("After Test");
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
        Debug.Log("Get Closest Enemy");
        Transform closest = null;
        var minDist = Mathf.Infinity;
        var currentPos = PlayerController.Instance.CurrentPlayerTransform().position;
        Debug.Log("Player Pos: " + currentPos);
        var enemies = GameManager.Instance.enemiesSpawnedList;
        Debug.Log(enemies.Count);
        foreach (var enemiesSpawned in enemies)
        {
            if (!enemiesSpawned) return null;
            var dist = Vector3.Distance(enemiesSpawned.transform.position, currentPos);
            if (!(dist < minDist)) continue;
            closest = enemiesSpawned.transform;
            minDist = dist;
        }
        Debug.Log("Closest: " + closest);
        return closest;
    }
}
