using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;

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

    private void Start()
    { 
        AuthenticationService.Instance.SignedIn += LoadHatTiers;
    }

    private async void LoadHatTiers()
    {
        //loop through 30 hat buttons
        for(int i = 0; i < 30; i++)
        {
            //load saved hat tier for each button
            int tier = await SaveManager.Instance.LoadSomeInt("Button" + i + "HatTier");
            if(tier == 0) tier = 1; //set minimum of hat tier 1
            
            //update button with hat tier
            UpdateButtonHatTier(i, tier);
        }
    }

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
        infoPanel.GetChild(5).GetChild(0).GetComponent<TMP_Text>().text = "Upgrade (" + button.CoinCost() + "<sprite=0>)";
        infoPanel.GetChild(6).GetChild(0).GetComponent<TMP_Text>().text = "Upgrade (" + button.GemCost() + "<sprite=0>)";

        //set button interactability based on player currency
        infoPanel.GetChild(5).GetComponent<Button>().interactable = ProfileManager.Instance.ProfileInfo.coins >= button.CoinCost();
        infoPanel.GetChild(6).GetComponent<Button>().interactable = ProfileManager.Instance.ProfileInfo.gems >= button.GemCost();

        //show/hide upgrade buttons depending on hat tier (hide buttons if max tier)
        infoPanel.GetChild(5).GetChild(0).gameObject.SetActive(button.HatTier < 4);
        infoPanel.GetChild(6).GetChild(0).gameObject.SetActive(button.HatTier < 4);
        infoPanel.GetChild(5).GetComponent<Image>().enabled = button.HatTier < 4;
        infoPanel.GetChild(6).GetComponent<Image>().enabled = button.HatTier < 4;
    }

    private void UpdateButtonHatTier(int index, int hatTier)
    {
        hatButtons_Portrait.GetChild(index).GetComponent<HatCollectionButton>().HatTier = hatTier;
        hatButtons_Landscape.GetChild(index).GetComponent<HatCollectionButton>().HatTier = hatTier;
        hatButtons_Portrait.GetChild(index).GetChild(2).GetComponent<TMP_Text>().text = "Tier " + hatTier;
        hatButtons_Landscape.GetChild(index).GetChild(2).GetComponent<TMP_Text>().text = "Tier " + hatTier;
        SaveManager.Instance.SaveSomeData("Button" + index + "HatTier", hatTier.ToString());
    }

    public void UpgradeWithCoins()
    {
        //get currently selected button
        HatCollectionButton button;
        hatButtons_Portrait.GetChild(selection).TryGetComponent<HatCollectionButton>(out button);
        hatButtons_Landscape.GetChild(selection).TryGetComponent<HatCollectionButton>(out button);

        //retrieve num coins and check if possible to upgrade
        int coins = ProfileManager.Instance.ProfileInfo.coins;
        if(coins < button.CoinCost()) return;

        //upgrade hat tier with coins
        ProfileManager.Instance.ChangeNumCoins(-button.CoinCost());
        button.HatTier++;
        UpdateButtonHatTier(selection, button.HatTier);

        //update upgrade info panels
        UpdateInfoPanel(hatInfoPanel_Portrait, button);
        UpdateInfoPanel(hatInfoPanel_Landscape, button);
    }

    public void UpgradeWithGems()
    {
        //get currently selected button
        HatCollectionButton button;
        hatButtons_Portrait.GetChild(selection).TryGetComponent<HatCollectionButton>(out button);
        hatButtons_Landscape.GetChild(selection).TryGetComponent<HatCollectionButton>(out button);

        //retrieve num gems and check if possible to upgrade
        int gems = ProfileManager.Instance.ProfileInfo.gems;
        if(gems < button.GemCost()) return;

        //upgrade hat tier with coins
        ProfileManager.Instance.ChangeNumGems(-button.GemCost());
        button.HatTier++;
        UpdateButtonHatTier(selection, button.HatTier);

        //update upgrade info panels
        UpdateInfoPanel(hatInfoPanel_Portrait, button);
        UpdateInfoPanel(hatInfoPanel_Landscape, button);
    }

    //Dev Option to reset player progress
    public void ResetAllHatTiers()
    {
        //loop through all 30 hat buttons
        for(int i = 0; i < 30; i++)
        {
            //update button with default hat tier of 1
            UpdateButtonHatTier(i, 1);
        }
    }
}
