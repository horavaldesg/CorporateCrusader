using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform hatsButtonBtm, battleButtonBtm, upgradeButtonBtm;
    [SerializeField] private RectTransform hatsButtonLeft, battleButtonLeft, upgradeButtonLeft;
    [SerializeField] private GameObject HatCollectionPanel_Portrait, HatCollectionPanel_Landscape;
    [SerializeField] private GameObject UpgradesPanel_Portrait, UpgradesPanel_Landscape;

    private Animator _anim;
    private bool _pastSplashScreen = false;
    
    private enum ScreenType { StageSelect, Hats, Upgrades };
    private ScreenType currentScreen;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!_pastSplashScreen) return;

        switch(Screen.orientation)
        {
            case ScreenOrientation.Portrait:
            case ScreenOrientation.PortraitUpsideDown:
                hatsButtonBtm.parent.gameObject.SetActive(true);
                hatsButtonLeft.parent.gameObject.SetActive(false);
                if(currentScreen == ScreenType.Hats)
                {
                    HatCollectionPanel_Portrait.SetActive(true);
                    HatCollectionPanel_Landscape.SetActive(false);
                }
                else if(currentScreen == ScreenType.Upgrades)
                {
                    UpgradesPanel_Portrait.SetActive(true);
                    UpgradesPanel_Landscape.SetActive(false);
                }
                break;
            case ScreenOrientation.LandscapeLeft:
            case ScreenOrientation.LandscapeRight:
                hatsButtonLeft.parent.gameObject.SetActive(true);
                hatsButtonBtm.parent.gameObject.SetActive(false);
                if(currentScreen == ScreenType.Hats)
                {
                    HatCollectionPanel_Landscape.SetActive(true);
                    HatCollectionPanel_Portrait.SetActive(false);
                }
                else if(currentScreen == ScreenType.Upgrades)
                {
                    UpgradesPanel_Landscape.SetActive(true);
                    UpgradesPanel_Portrait.SetActive(false);
                }
                break;
        }
    }

    public void SplashScreenButton()
    {
        _pastSplashScreen = true;
        _anim.SetTrigger("SplashScreenToStageSelect");
        currentScreen = ScreenType.StageSelect;
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

        //update current screen type
        currentScreen = ScreenType.Hats;
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
    }
}
