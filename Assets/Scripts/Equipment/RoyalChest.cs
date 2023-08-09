using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RoyalChest : Equipment
{
    public int coinMultiplier;
    
    public override void Level1()
    {
        base.Level1();
        PlayerController.Instance.IncreaseCoinGain(coinMultiplier);
    }

    public override void Level2()
    {
        base.Level2();
        PlayerController.Instance.IncreaseCoinGain(coinMultiplier);
    }

    public override void Level3()
    {
        base.Level3();
        PlayerController.Instance.IncreaseCoinGain(coinMultiplier);
    }

    public override void Level4()
    {
        base.Level4();
        PlayerController.Instance.IncreaseCoinGain(coinMultiplier);
    }

    protected override void Level5()
    {
        base.Level5();
        PlayerController.Instance.IncreaseCoinGain(coinMultiplier);
    }
}
