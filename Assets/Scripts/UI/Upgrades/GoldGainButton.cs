public class GoldGainButton : UpgradeButton
{
    public override string UpgradeDescription()
    {
        string desc = "This is the description for the gold gain button. It should be based on the current upgrade level, which is " + UpgradeLevel;
        return desc;
    }

    public override string NextLevelText()  
    {
        string text = "Increase gold gain by " + (5 + (UpgradeLevel * 2)) + "%"; //7%, 9%, 11%, etc.
        return text;
    }
}
