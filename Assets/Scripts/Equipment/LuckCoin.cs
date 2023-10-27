using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckCoin : Equipment
{
    public override void Level1()
    {
        base.Level1();
        GameManager.Instance.SetLuckCoinList(16);
        LevelUpUpgradeManager.Instance.HasLuckCoin = true;
        Debug.Log("has luck coin");
    }

    public override void Level2()
    {
        base.Level2();
        GameManager.Instance.SetLuckCoinList(21);

    }

    public override void Level3()
    {
        base.Level3();
        GameManager.Instance.SetLuckCoinList(26);
    }

    public override void Level4()
    {
        base.Level4();
        GameManager.Instance.SetLuckCoinList(31);
    }

    protected override void Level5()
    {
        base.Level5();
        GameManager.Instance.SetLuckCoinList(46);
    }
}
