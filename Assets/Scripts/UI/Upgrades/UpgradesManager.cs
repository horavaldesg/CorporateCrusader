using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private int selection = 0;

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
        infoPanel.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Upgrade (" + button.UpgradeCost() + ")"; //upgrade cost   <-NOTE: NEED TO ADD COIN ICON
        infoPanel.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = button.NextLevelText(); //next level effect
        infoPanel.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = "Requires Player Level " + button.LevelReq(); //level requirement

        int playerLevel = 5; //<-NOTE: Get player level here when it is implemented
        infoPanel.GetChild(1).GetChild(3).gameObject.SetActive((playerLevel < button.LevelReq())); //upgrade lock
    }
}
