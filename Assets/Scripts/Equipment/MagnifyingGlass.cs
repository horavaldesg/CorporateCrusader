using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyingGlass : Equipment
{
    public float sizeMultiplier;
    
    public override void Level1()
    {
        base.Level1();
        PlayerController.Instance.IncreaseProjectileSize(sizeMultiplier);
    }
   
    public override void Level2()
    {
        base.Level2();
        PlayerController.Instance.IncreaseProjectileSize(sizeMultiplier);
    }
   
    public override void Level3()
    {
        base.Level3();
        PlayerController.Instance.IncreaseProjectileSize(sizeMultiplier);
    }
   
    public override void Level4()
    {
        base.Level4();
        PlayerController.Instance.IncreaseProjectileSize(sizeMultiplier);
    }

    protected override void Level5()
    {
        base.Level5();
        PlayerController.Instance.IncreaseProjectileSize(sizeMultiplier);
    }
}
