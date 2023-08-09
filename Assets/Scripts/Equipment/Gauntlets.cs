using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets : Equipment
{
    public float damageMultiplier;
    
    public override void Level1()
    {
        base.Level1();
        PlayerController.Instance.IncreaseDamage(damageMultiplier);
    }

    public override void Level2()
    {
        base.Level2();
        PlayerController.Instance.IncreaseDamage(damageMultiplier);
    }

    public override void Level3()
    {
        base.Level3();
        PlayerController.Instance.IncreaseDamage(damageMultiplier);
    }

    public override void Level4()
    {
        base.Level4();
        PlayerController.Instance.IncreaseDamage(damageMultiplier);
    }

    protected override void Level5()
    {
        base.Level5();
        PlayerController.Instance.IncreaseDamage(damageMultiplier);
    }
}
