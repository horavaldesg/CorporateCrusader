using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HatCollectionManager : MonoBehaviour
{
    [Header("Hat Scroll Rects")]
    [SerializeField] private ScrollRect hatScrollRect_Portrait;
    [SerializeField] private ScrollRect hatScrollRect_Landscape;

    [Header("Hat Button Parents")]
    [SerializeField] private RectTransform hatButtons_Portrait;
    [SerializeField] private RectTransform hatButtons_Landscape;

    [Header("Button Selection Outlines")]
    [SerializeField] private RectTransform selectionOutline_Portrait;
    [SerializeField] private RectTransform selectionOutline_Landscape;

    [Header("Hat Info Panels")]
    [SerializeField] private RectTransform hatInfoPanel_Portrait;
    [SerializeField] private RectTransform hatInfoPanel_Landscape;

    private int selection = 0;

    public void OnScrollViewChanged(Vector2 value)
    {
        switch(Screen.orientation)
        {
            case ScreenOrientation.Portrait:
            case ScreenOrientation.PortraitUpsideDown:
                hatScrollRect_Landscape.normalizedPosition = new Vector2(0, value.y);
                hatButtons_Landscape.offsetMin = new Vector2(15, hatButtons_Landscape.offsetMin.y);
                hatButtons_Landscape.offsetMax = new Vector2(0, hatButtons_Landscape.offsetMax.y);
                break;
            case ScreenOrientation.LandscapeLeft:
            case ScreenOrientation.LandscapeRight:
                hatScrollRect_Portrait.normalizedPosition = new Vector2(0, value.y);
                hatButtons_Portrait.offsetMin = new Vector2(15, hatButtons_Landscape.offsetMin.y);
                hatButtons_Portrait.offsetMax = new Vector2(0, hatButtons_Landscape.offsetMax.y);
                break;
        }
    }

    public void HatCollectionButton(Transform button)
    {
        selection = button.GetSiblingIndex();
        UpdateSelection();
    }

    public void UpdateSelection()
    {
        //update button selection outlines
        selectionOutline_Portrait.SetParent(hatButtons_Portrait.GetChild(selection));
        selectionOutline_Portrait.localScale = Vector3.one;
        selectionOutline_Portrait.anchoredPosition = Vector3.zero;
        selectionOutline_Landscape.SetParent(hatButtons_Landscape.GetChild(selection));
        selectionOutline_Landscape.localScale = Vector3.one;
        selectionOutline_Landscape.anchoredPosition = Vector3.zero;
        
        //get currently selected button
        HatCollectionButton button;
        hatButtons_Portrait.GetChild(selection).TryGetComponent<HatCollectionButton>(out button);
        hatButtons_Landscape.GetChild(selection).TryGetComponent<HatCollectionButton>(out button);
        
        //update upgrade info panels
        UpdateInfoPanel(hatInfoPanel_Portrait, button);
        UpdateInfoPanel(hatInfoPanel_Landscape, button);
    }

    private void UpdateInfoPanel(RectTransform infoPanel, HatCollectionButton button)
    {
        //set UI elements according to currently selected hat
        infoPanel.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = button.HatName;
        infoPanel.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Tier " + button.HatTier;
        infoPanel.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = button.HatDescription;
        infoPanel.GetChild(0).GetChild(3).GetComponent<Image>().sprite = button.HatIcon;

        //set tier descriptions
        infoPanel.GetChild(1).GetComponent<TMP_Text>().text = button.Tier1Description;
        infoPanel.GetChild(2).GetComponent<TMP_Text>().text = button.Tier2Description;
        infoPanel.GetChild(3).GetComponent<TMP_Text>().text = button.Tier3Description;
        infoPanel.GetChild(4).GetComponent<TMP_Text>().text = button.Tier4Description;
        
        //set button costs
        infoPanel.GetChild(5).GetChild(0).GetComponent<TMP_Text>().text = "Upgrade (" + button.GoldCost() + "<sprite=0>)";
        infoPanel.GetChild(6).GetChild(0).GetComponent<TMP_Text>().text = "Upgrade (" + button.GemCost() + "<sprite=0>)";

        //lock upgrade buttons if hat is max tier
        if(button.HatTier >= 4)
        {
            infoPanel.GetChild(5).GetComponent<Button>().interactable = false;
            infoPanel.GetChild(6).GetComponent<Button>().interactable = false;
        }
    }
}
