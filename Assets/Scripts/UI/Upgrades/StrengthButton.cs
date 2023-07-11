public class StrengthButton : UpgradeButton
{
    public override string UpgradeDescription()
    {
        string desc = "This is the description for the strength button. It should be based on the current upgrade level, which is " + UpgradeLevel;
        return desc;
    }

    public override string NextLevelText()  
    {
        string text = "Increase strength by " + (8 + (UpgradeLevel * 2)) + "%"; //10%, 12%, 14%, etc.
        return text;
    }
}
