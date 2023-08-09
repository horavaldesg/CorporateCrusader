using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardeningGloves : Equipment
{
    public float bulletSpeedIncrease;

    public override void Level1()
    {
        base.Level1();
        PlayerController.Instance.IncreaseProjectileSpeed(bulletSpeedIncrease);
    }

    public override void Level2()
    {
        base.Level2();
        PlayerController.Instance.IncreaseProjectileSpeed(bulletSpeedIncrease);
    }

    public override void Level3()
    {
        base.Level3();
        PlayerController.Instance.IncreaseProjectileSpeed(bulletSpeedIncrease);
    }

    public override void Level4()
    {
        base.Level4();
        PlayerController.Instance.IncreaseProjectileSpeed(bulletSpeedIncrease);
    }

    public override void Level5()
    {
        base.Level5();
        PlayerController.Instance.IncreaseProjectileSpeed(bulletSpeedIncrease);
    }
}