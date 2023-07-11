using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Screen Selection Buttons")]
    [SerializeField] private RectTransform hatsButtonBtm;
    [SerializeField] private RectTransform battleButtonBtm;
    [SerializeField] private RectTransform upgradeButtonBtm;
    [SerializeField] private RectTransform hatsButtonLeft;
    [SerializeField] private RectTransform battleButtonLeft;
    [SerializeField] private RectTransform upgradeButtonLeft;

    [Header("Screen Panels")]
    [SerializeField] private GameObject hatCollectionPanel_Portrait;
    [SerializeField] private GameObject hatCollectionPanel_Landscape;
    [SerializeField] private GameObject upgradesPanel_Portrait;
    [SerializeField] private GameObject ugradesPanel_Landscape;

    [Header("Script References")]
    [SerializeField] private UpgradesManager upgradesManager;

    private Animator _anim;
    private bool _pastSplashScreen = false;
    private ScreenOrientation currentOrientation;
    
    private enum ScreenType { StageSelect, Hats, Upgrades };
    private ScreenType currentScreen;

    private void Start()
    {
        _anim = GetComponent<Animator>();

        currentOrientation = Screen.orientation;
    }

    private void Update()
    {
        //check if past splash screen
        if(!_pastSplashScreen) return;
        
        //check if screen orientation changed
        if(currentOrientation != Screen.orientation)
        {
            //update screen orientation
            currentOrientation = Screen.orientation;

            //update screen selection buttons
            UpdateScreenButtons();

            //update panels based on screen orientation
            UpdateScreenPanels();
        }
    }

    public void SplashScreenButton()
    {
        _pastSplashScreen = true;
        _anim.SetTrigger("SplashScreenToStageSelect");
        currentScreen = ScreenType.StageSelect;

        //update screen selection buttons
        UpdateScreenButtons();
    }

    public void HatsButton()
    {
        //update portrait mode buttons
        hatsButtonBtm.SetAsLastSibling();
        hatsButtonBtm.sizeDelta = new Vector2(250, 250);
        hatsButtonBtm.anchoredPosition = new Vector3(-210, 0, 0);
        battleButtonBtm.sizeDelta = new Vector2(200, 200);
        upgradeButtonBtm.sizeDelta = new Vector2(200, 200);
        upgradeButtonBtm.anchoredPosition = new Vector3(190, 0, 0);
        
        //update landscape mode buttons
        hatsButtonLeft.SetAsLastSibling();
        hatsButtonLeft.sizeDelta = new Vector2(175, 175);
        hatsButtonLeft.anchoredPosition = new Vector3(0, 140, 0);
        battleButtonLeft.sizeDelta = new Vector2(125, 125);
        upgradeButtonLeft.sizeDelta = new Vector2(125, 125);
        upgradeButtonLeft.anchoredPosition = new Vector3(0, -120, 0);

        //trigger proper animation
        if(currentScreen == ScreenType.StageSelect) _anim.SetTrigger("StageSelectToHats");
        else if(currentScreen == ScreenType.Upgrades) _anim.SetTrigger("UpgradesToHats");

        //update current screen type and panels
        currentScreen = ScreenType.Hats;
        UpdateScreenPanels();
    }

    public void BattleButton()
    {
        //update portrait mode buttons
        battleButtonBtm.SetAsLastSibling();
        battleButtonBtm.sizeDelta = new Vector2(250, 250);
        hatsButtonBtm.sizeDelta = new Vector2(200, 200);
        hatsButtonBtm.anchoredPosition = new Vector3(-210, 0, 0);
        upgradeButtonBtm.sizeDelta = new Vector2(200, 200);
        upgradeButtonBtm.anchoredPosition = new Vector3(210, 0, 0);
        
        //update landscape mode buttons
        battleButtonLeft.SetAsLastSibling();
        battleButtonLeft.sizeDelta = new Vector2(175, 175);
        hatsButtonLeft.sizeDelta = new Vector2(125, 125);
        hatsButtonLeft.anchoredPosition = new Vector3(0, 140, 0);
        upgradeButtonLeft.sizeDelta = new Vector2(125, 125);
        upgradeButtonLeft.anchoredPosition = new Vector3(0, -140, 0);

        //trigger proper animation
        if(currentScreen == ScreenType.Hats) _anim.SetTrigger("HatsToStageSelect");
        else if(currentScreen == ScreenType.Upgrades) _anim.SetTrigger("UpgradesToStageSelect");

        //update current screen type
        currentScreen = ScreenType.StageSelect;
        UpdateScreenPanels();
    }

    public void UpgradeButton()
    {
        //update portrait mode buttons
        upgradeButtonBtm.SetAsLastSibling();
        upgradeButtonBtm.sizeDelta = new Vector2(250, 250);
        upgradeButtonBtm.anchoredPosition = new Vector3(210, 0, 0);
        battleButtonBtm.sizeDelta = new Vector2(200, 200);
        hatsButtonBtm.sizeDelta = new Vector2(200, 200);
        hatsButtonBtm.anchoredPosition = new Vector3(-190, 0, 0);
        
        //update landscape mode buttons
        upgradeButtonLeft.SetAsLastSibling();
        upgradeButtonLeft.sizeDelta = new Vector2(175, 175);
        upgradeButtonLeft.anchoredPosition = new Vector3(0, -140, 0);
        battleButtonLeft.sizeDelta = new Vector2(125, 125);
        hatsButtonLeft.sizeDelta = new Vector2(125, 125);
        hatsButtonLeft.anchoredPosition = new Vector3(0, 120, 0);

        //trigger proper animation
        if(currentScreen == ScreenType.StageSelect) _anim.SetTrigger("StageSelectToUpgrades");
        else if(currentScreen == ScreenType.Hats) _anim.SetTrigger("HatsToUpgrades");

        //update current screen type
        currentScreen = ScreenType.Upgrades;
        UpdateScreenPanels();
    }

    private void UpdateScreenButtons()
    {
        switch(Screen.orientation)
        {
            case ScreenOrientation.Portrait:
            case ScreenOrientation.PortraitUpsideDown:
                hatsButtonBtm.parent.gameObject.SetActive(true);
                hatsButtonLeft.parent.gameObject.SetActive(false);
                break;
            case ScreenOrientation.LandscapeLeft:
            case ScreenOrientation.LandscapeRight:
                hatsButtonLeft.parent.gameObject.SetActive(true);
                hatsButtonBtm.parent.gameObject.SetActive(false);
                break;
        }
    }

    private void UpdateScreenPanels()
    {
        //update panels based on screen orientation
        switch(Screen.orientation)
        {
            case ScreenOrientation.Portrait:
            case ScreenOrientation.PortraitUpsideDown:
                if(currentScreen == ScreenType.Hats)
                {
                    hatCollectionPanel_Portrait.SetActive(true);
                    hatCollectionPanel_Landscape.SetActive(false);
                }
                else if(currentScreen == ScreenType.Upgrades)
                {
                    upgradesPanel_Portrait.SetActive(true);
                    ugradesPanel_Landscape.SetActive(false);
                    upgradesManager.UpdateSelection();
                }
                break;
            case ScreenOrientation.LandscapeLeft:
            case ScreenOrientation.LandscapeRight:
                if(currentScreen == ScreenType.Hats)
                {
                    hatCollectionPanel_Landscape.SetActive(true);
                    hatCollectionPanel_Portrait.SetActive(false);
                }
                else if(currentScreen == ScreenType.Upgrades)
                {
                    ugradesPanel_Landscape.SetActive(true);
                    upgradesPanel_Portrait.SetActive(false);
                    upgradesManager.UpdateSelection();
                }
                break;
        }
    }
}
