using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Equipment
{
    public float enemyDamageReduction;
    
    public override void Level1()
    {
        base.Level1();
        PlayerController.Instance.HasShield = true;
        PlayerController.Instance.Shield(enemyDamageReduction);
    }

    public override void Level2()
    {
        base.Level2();
        PlayerController.Instance.Shield(enemyDamageReduction);
    }

    public override void Level3()
    {
        base.Level3();
        PlayerController.Instance.Shield(enemyDamageReduction);
    }

    public override void Level4()
    {
        base.Level4();
        PlayerController.Instance.Shield(enemyDamageReduction);
    }

    public override void Level5()
    {
        base.Level5();
        PlayerController.Instance.Shield(enemyDamageReduction);
    }
}
