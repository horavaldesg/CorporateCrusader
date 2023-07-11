public class MaxArmorButton : UpgradeButton
{
    public override string UpgradeDescription()
    {
        string desc = "This is the description for the max armor button. It should be based on the current upgrade level, which is " + UpgradeLevel;
        return desc;
    }

    public override string NextLevelText()  
    {
        string text = "Increase max armor by " + (3 + (UpgradeLevel * 2)) + "%"; //5%, 7%, 9%, etc.
        return text;
    }
}
