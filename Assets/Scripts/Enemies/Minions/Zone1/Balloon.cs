using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : Enemy
{
    [SerializeField] private GameObject gasToSpawn;
    [SerializeField] private Transform gasSpawnPoint;

    protected override void EnemyDied()
    {
        var go = Instantiate(gasToSpawn, null, true);
        go.transform.position = gasSpawnPoint.position;
        go.TryGetComponent(out GasDamage gasDamage);
        gasDamage.damage = _damage;
        base.EnemyDied();
    }
}
