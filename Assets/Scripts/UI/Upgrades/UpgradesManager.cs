using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;

public class UpgradesManager : MonoBehaviour
{
    [Header("Upgrade Button Panels")]
    [SerializeField] private RectTransform upgradeButtons_Portrait;
    [SerializeField] private RectTransform upgradeButtons_Landscape;

    [Header("Button Selection Outlines")]
    [SerializeField] private RectTransform selectionOutline_Portrait;
    [SerializeField] private RectTransform selectionOutline_Landscape;

    [Header("Upgrade Info Panels")]
    [SerializeField] private RectTransform upgradeInfoPanel_Portrait;
    [SerializeField] private RectTransform upgradeInfoPanel_Landscape;

    [Header("Level Req Text Colors")]
    [SerializeField] private Color cannotLevelUpColor;
    [SerializeField] private Color canLevelUpColor;

    private int selection = 0;

    private void Start()
    { 
        AuthenticationService.Instance.SignedIn += LoadUpgrades;
    }

    private async void LoadUpgrades()
    {
        //loop through 9 upgrade buttons
        for(int i = 0; i < 9; i++)
        {
            //load saved upgrade level for each button
            int level = await SaveManager.Instance.LoadSomeInt("Button" + i + "UpgradeLevel");
            if(level == 0) level = 1; //set minimum of upgrade level 1
            
            //update button with upgrade level
            UpdateButtonUpgradeLevel(i, level);
        }
    }

    public void UpgradeButton(UpgradeButton button)
    {
        selection = button.transform.GetSiblingIndex();
        UpdateSelection();
    }

    public void UpdateSelection()
    {
        //update button selection outlines
        selectionOutline_Portrait.SetParent(upgradeButtons_Portrait.GetChild(selection));
        selectionOutline_Portrait.localScale = Vector3.one;
        selectionOutline_Portrait.anchoredPosition = Vector3.zero;
        selectionOutline_Landscape.SetParent(upgradeButtons_Landscape.GetChild(selection));
        selectionOutline_Landscape.localScale = Vector3.one;
        selectionOutline_Landscape.anchoredPosition = Vector3.zero;

        //get currently selected button
        UpgradeButton button;
        upgradeButtons_Portrait.GetChild(selection).TryGetComponent<UpgradeButton>(out button);
        upgradeButtons_Landscape.GetChild(selection).TryGetComponent<UpgradeButton>(out button);
        
        //update upgrade info panels
        UpdateInfoPanel(upgradeInfoPanel_Portrait, button);
        UpdateInfoPanel(upgradeInfoPanel_Landscape, button);
    }

    private void UpdateInfoPanel(RectTransform infoPanel, UpgradeButton button)
    {
        //set UI elements according to currently selected button
        infoPanel.GetChild(0).GetChild(0).GetComponent<Image>().sprite = button.UpgradeIcon; //upgrade icon
        infoPanel.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = button.UpgradeName; //upgrade name
        infoPanel.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = "Level " + button.UpgradeLevel; //upgrade level
        infoPanel.GetChild(0).GetChild(3).GetComponent<TMP_Text>().text = button.UpgradeDescription(); //upgrade description

        infoPanel.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Upgrade (" + button.UpgradeCost() + "<sprite=0>)"; //upgrade cost
        infoPanel.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = button.NextLevelText(); //next level effect
        infoPanel.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = "Requires Player Level " + button.LevelReq(); //level requirement

        //set upgrade lock visibility and level req text color based on player's profile level
        infoPanel.GetChild(1).GetChild(3).gameObject.SetActive(ProfileManager.Instance.ProfileInfo.profileLevel < button.LevelReq());
        infoPanel.GetChild(1).GetChild(2).GetComponent<TMP_Text>().color = 
            (ProfileManager.Instance.ProfileInfo.profileLevel < button.LevelReq())? cannotLevelUpColor : canLevelUpColor;
        infoPanel.GetChild(1).GetComponent<Button>().enabled = ProfileManager.Instance.ProfileInfo.profileLevel >= button.LevelReq();

        //set upgrade button interactability based on player profile level and currency
        infoPanel.GetChild(1).GetComponent<Button>().interactable = true;
        if(ProfileManager.Instance.ProfileInfo.profileLevel >= button.LevelReq()
        && ProfileManager.Instance.ProfileInfo.coins < button.UpgradeCost())
            infoPanel.GetChild(1).GetComponent<Button>().interactable = false;
    }

    private void UpdateButtonUpgradeLevel(int index, int level)
    {
        upgradeButtons_Portrait.GetChild(index).GetComponent<UpgradeButton>().UpgradeLevel = level;
        upgradeButtons_Landscape.GetChild(index).GetComponent<UpgradeButton>().UpgradeLevel = level;
        upgradeButtons_Portrait.GetChild(index).GetChild(2).GetComponent<TMP_Text>().text = "Level " + level;
        upgradeButtons_Landscape.GetChild(index).GetChild(2).GetComponent<TMP_Text>().text = "Level " + level;
        SaveManager.Instance.SaveSomeData("Button" + index + "UpgradeLevel", level.ToString());
    }

    public void UpgradeWithCoins()
    {
        //get currently selected button
        UpgradeButton button;
        upgradeButtons_Portrait.GetChild(selection).TryGetComponent<UpgradeButton>(out button);
        upgradeButtons_Landscape.GetChild(selection).TryGetComponent<UpgradeButton>(out button);
        
        //retrieve num coins and check if possible to upgrade
        int coins = ProfileManager.Instance.ProfileInfo.coins;
        if(coins < button.UpgradeCost()) return;

        //upgrade level with coins
        ProfileManager.Instance.ChangeNumCoins(-button.UpgradeCost());
        button.UpgradeLevel++;
        UpdateButtonUpgradeLevel(selection, button.UpgradeLevel);

        //update upgrade info panels
        UpdateInfoPanel(upgradeInfoPanel_Portrait, button);
        UpdateInfoPanel(upgradeInfoPanel_Landscape, button);
    }

    //Dev Option to reset player progress for all 9 upgrades
    public void ResetAllUpgrades()
    {
        //loop through 9 upgrade buttons
        for(int i = 0; i < 9; i++)
        {
            //update button with default upgrade level of 1
            UpdateButtonUpgradeLevel(i, 1);
        }
    }
}
