using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalDrone : SelectedWeapon
{ 
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float orbitSize;
    [SerializeField] private float orbitSpeed;

    private Transform playerTransform;
    private float angle = 0f;

    protected override void Start()
    {
        playerTransform = PlayerController.Instance.CurrentPlayerTransform();
        base.Start();
    }

    private void Update()
    {
        //make drone orbit around the player's position
        transform.localPosition = new Vector2(1.5f * Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle)) * orbitSize;
        angle += orbitSpeed * Time.deltaTime;
    }

    protected override void Activate()
    {
        TargetEnemy();
        base.Activate();
    }

    private void TargetEnemy()
    {
        if (!GetClosestEnemy()) return;
        var go = Instantiate(instantiatedObject);
        go.transform.position = transform.GetChild(0).position;
        go.TryGetComponent(out DroneBullet droneBullet);
        droneBullet.bulletSpeed = bulletSpeed;
        droneBullet.damage = damage;
        droneBullet.attributes = attribute;
        droneBullet.timeAlive = 5;
        droneBullet.TargetPos = GetClosestEnemy();
        float angle = Mathf.Atan2(droneBullet.TargetPos.position.y - go.transform.position.y, 
                                  droneBullet.TargetPos.position.x - go.transform.position.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    

    private Transform GetClosestEnemy()
    {
        Transform closest = null;
        var minDist = Mathf.Infinity;
        var currentPos = playerTransform.position;
        var enemies = GameManager.Instance.enemiesSpawnedList;
        foreach (var enemiesSpawned in enemies)
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
