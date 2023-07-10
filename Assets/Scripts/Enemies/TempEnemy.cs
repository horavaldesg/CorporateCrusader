using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy : Enemy
{
    protected override void Awake()
    {
        _enemyName = "TempEnemy";
        base.Awake();
    }

    protected override void Attack()
    {
        //Atack Player
        Slash();
        base.Attack();
    }

    private void Slash()
    {
        playerDist.TryGetComponent(out PlayerController playerController);
        playerController.TakeDamage(_damage);
    }
}
