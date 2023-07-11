public class XPGainButton : UpgradeButton
{
    public override string UpgradeDescription()
    {
        string desc = "This is the description for the XP gain button. It should be based on the current upgrade level, which is " + UpgradeLevel;
        return desc;
    }

    public override string NextLevelText()  
    {
        string text = "Increase XP gain by " + (4 + (UpgradeLevel * 3)) + "%"; //7%, 10%, 13%, etc.
        return text;
    }
}
