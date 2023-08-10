using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPalette : Equipment
{
    public float coolDown;
    
    public override void Level1()
    {
        base.Level1();
        PlayerController.Instance.AbilityCoolDown(coolDown);
    }

    public override void Level2()
    {
        base.Level2();
        PlayerController.Instance.AbilityCoolDown(coolDown);
    }

    public override void Level3()
    {
        base.Level3();
        PlayerController.Instance.AbilityCoolDown(coolDown);
    }

    public override void Level4()
    {
        base.Level4();
        PlayerController.Instance.AbilityCoolDown(coolDown);
    }

    protected override void Level5()
    {
        base.Level5();
        PlayerController.Instance.AbilityCoolDown(coolDown);
    }
}
