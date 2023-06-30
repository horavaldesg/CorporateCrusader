using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform hatsButtonBtm, battleButtonBtm, upgradeButtonBtm;
    [SerializeField] private RectTransform hatsButtonLeft, battleButtonLeft, upgradeButtonLeft;

    private void Update()
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

    public void HatsButton()
    {
        hatsButtonBtm.SetAsLastSibling();
        hatsButtonBtm.sizeDelta = new Vector2(250, 250);
        hatsButtonBtm.anchoredPosition = new Vector3(-210, 0, 0);
        battleButtonBtm.sizeDelta = new Vector2(200, 200);
        upgradeButtonBtm.sizeDelta = new Vector2(200, 200);
        upgradeButtonBtm.anchoredPosition = new Vector3(190, 0, 0);
        
        hatsButtonLeft.SetAsLastSibling();
        hatsButtonLeft.sizeDelta = new Vector2(175, 175);
        hatsButtonLeft.anchoredPosition = new Vector3(0, 140, 0);
        battleButtonLeft.sizeDelta = new Vector2(125, 125);
        upgradeButtonLeft.sizeDelta = new Vector2(125, 125);
        upgradeButtonLeft.anchoredPosition = new Vector3(0, -120, 0);
    }

    public void BattleButton()
    {
        battleButtonBtm.SetAsLastSibling();
        battleButtonBtm.sizeDelta = new Vector2(250, 250);
        hatsButtonBtm.sizeDelta = new Vector2(200, 200);
        hatsButtonBtm.anchoredPosition = new Vector3(-210, 0, 0);
        upgradeButtonBtm.sizeDelta = new Vector2(200, 200);
        upgradeButtonBtm.anchoredPosition = new Vector3(210, 0, 0);
        
        battleButtonLeft.SetAsLastSibling();
        battleButtonLeft.sizeDelta = new Vector2(175, 175);
        hatsButtonLeft.sizeDelta = new Vector2(125, 125);
        hatsButtonLeft.anchoredPosition = new Vector3(0, 140, 0);
        upgradeButtonLeft.sizeDelta = new Vector2(125, 125);
        upgradeButtonLeft.anchoredPosition = new Vector3(0, -140, 0);
    }

    public void UpgradeButton()
    {
        upgradeButtonBtm.SetAsLastSibling();
        upgradeButtonBtm.sizeDelta = new Vector2(250, 250);
        upgradeButtonBtm.anchoredPosition = new Vector3(210, 0, 0);
        battleButtonBtm.sizeDelta = new Vector2(200, 200);
        hatsButtonBtm.sizeDelta = new Vector2(200, 200);
        hatsButtonBtm.anchoredPosition = new Vector3(-190, 0, 0);
        
        upgradeButtonLeft.SetAsLastSibling();
        upgradeButtonLeft.sizeDelta = new Vector2(175, 175);
        upgradeButtonLeft.anchoredPosition = new Vector3(0, -140, 0);
        battleButtonLeft.sizeDelta = new Vector2(125, 125);
        hatsButtonLeft.sizeDelta = new Vector2(125, 125);
        hatsButtonLeft.anchoredPosition = new Vector3(0, 120, 0);
    }
}
