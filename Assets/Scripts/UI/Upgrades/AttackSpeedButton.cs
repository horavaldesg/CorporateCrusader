public class AttackSpeedButton : UpgradeButton
{
    public override string UpgradeDescription()
    {
        string desc = "This is the description for the attack speed button. It should be based on the current upgrade level, which is " + UpgradeLevel;
        return desc;
    }

    public override string NextLevelText()  
    {
        string text = "Increase attack speed by " + (5 + (UpgradeLevel * 3)) + "%"; //8%, 11%, 14%, etc.
        return text;
    }
}
