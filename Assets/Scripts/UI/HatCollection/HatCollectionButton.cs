using UnityEngine;

public class HatCollectionButton : MonoBehaviour
{
    public string HatName;
    public Sprite HatIcon;
    public int HatTier;
    public string HatDescription;

    [Header("Tier Descriptions")]
    public string Tier1Description;
    public string Tier2Description;
    public string Tier3Description;
    public string Tier4Description;

    public int GoldCost()
    {
        int cost = 100 + (HatTier * 200);
        return cost;
    }

    public int GemCost()
    {
        int cost = 20 + (HatTier * 10);
        return cost;
    }
}
