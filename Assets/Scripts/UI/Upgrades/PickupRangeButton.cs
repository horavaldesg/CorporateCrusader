public class PickupRangeButton : UpgradeButton
{
    public override string UpgradeDescription()
    {
        string desc = "This is the description for the pickup range button. It should be based on the current upgrade level, which is " + UpgradeLevel;
        return desc;
    }

    public override string NextLevelText()  
    {
        string text = "Increase pickup range by " + (6 + (UpgradeLevel * 2)) + "%"; //8%, 10%, 12%, etc.
        return text;
    }
}
