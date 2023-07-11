public class RegenerationButton : UpgradeButton
{
    public override string UpgradeDescription()
    {
        string desc = "This is the description for the regeneration button. It should be based on the current upgrade level, which is " + UpgradeLevel;
        return desc;
    }

    public override string NextLevelText()  
    {
        string text = "Increase regeneration rate by " + (UpgradeLevel * 5) + "%"; //5%, 10%, 15%, etc.
        return text;
    }
}
