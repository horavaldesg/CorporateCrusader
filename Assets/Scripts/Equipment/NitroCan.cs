using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NitroCan : Equipment
{
    public float attackDecrease;
    
    public override void Level1()
    {
        base.Level1();
        PlayerController.Instance.IncreaseAttackSpeed(attackDecrease);
    }

    public override void Level2()
    {
        base.Level2();
        PlayerController.Instance.IncreaseAttackSpeed(attackDecrease);
    }

    public override void Level3()
    {
        base.Level3();
        PlayerController.Instance.IncreaseAttackSpeed(attackDecrease);
    }

    public override void Level4()
    {
        base.Level4();
        PlayerController.Instance.IncreaseAttackSpeed(attackDecrease);
    }

    protected override void Level5()
    {
        base.Level5();
        PlayerController.Instance.IncreaseAttackSpeed(attackDecrease);
    }
}
