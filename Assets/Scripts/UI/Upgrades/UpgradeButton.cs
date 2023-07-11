using UnityEngine;

public abstract class UpgradeButton : MonoBehaviour
{
    public string UpgradeName;
    public Sprite UpgradeIcon;
    public int UpgradeLevel;

    public abstract string UpgradeDescription();
    public abstract string NextLevelText();

    public int UpgradeCost()
    {
        int cost = UpgradeLevel * 120;
        return cost;
    }

    public int LevelReq()
    {
        int levelReq = UpgradeLevel * 5;
        return levelReq;
    }
}
