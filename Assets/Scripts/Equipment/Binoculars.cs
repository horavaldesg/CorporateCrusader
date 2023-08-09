using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Binoculars :  Equipment
{
    public float enemyHealthReduction;
    
    public override void Level1()
    {
        base.Level1();
        PlayerController.Instance.HaseBinoculars = true;
        PlayerController.Instance.Binoculars(enemyHealthReduction);
    }

    public override void Level2()
    {
        base.Level2();
        PlayerController.Instance.Binoculars(enemyHealthReduction);
    }

    public override void Level3()
    {
        base.Level3();
        PlayerController.Instance.Binoculars(enemyHealthReduction);
    }

    public override void Level4()
    {
        base.Level4();
        PlayerController.Instance.Binoculars(enemyHealthReduction);
    }

    protected override void Level5()
    {
        base.Level5();
        PlayerController.Instance.Binoculars(enemyHealthReduction);
    }
}
