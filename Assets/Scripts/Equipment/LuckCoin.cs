using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckCoin : Equipment
{
    public override void Level1()
    {
        base.Level1();
        LevelUpUpgradeManager.Instance.HasLuckCoin = true;
        Debug.Log("has luck coin");
    }
}
