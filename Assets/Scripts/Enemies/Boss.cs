using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : Enemy
{
    public static event Action OnBossKilled;
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        OnBossKilled?.Invoke();
    }

    protected override void Attack()
    {
        base.Attack();
        //Boss Attack
    }
}
