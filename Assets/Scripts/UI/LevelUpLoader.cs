using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpLoader : MonoBehaviour
{
    public SelectedWeapon selectedWeapon;
    public Equipment selectedEquipment;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image imageReference;
    public GameObject[] levelSpriteReference;
    public int level;

    [HideInInspector] public bool isEquipment;

    [HideInInspector] public bool isGold;
    /*private void Start()
    {
        foreach (var t in levelSpriteReference)
        {
            t.TryGetComponent(out Image image);
            image.color = Color.black;
        }
    }*/

    public void LoadUpgrade()
    {
        nameText.SetText(selectedWeapon.weaponName);
        descriptionText.SetText(selectedWeapon.weaponDescription);
        imageReference.sprite = WeaponManager.Instance.WeaponCanEvolve(selectedWeapon) ? 
            selectedWeapon.evoSprite : 
            selectedWeapon.weaponSprite;
        for (var i = 0; i < levelSpriteReference.Length; i++)
        {
            if (i < level)
            {
                levelSpriteReference[i].TryGetComponent(out Image image);
                image.color = Color.white;
            }
            else
            {
                levelSpriteReference[i].TryGetComponent(out Image image);
                image.color = Color.black;
            }
        }
    }

    public void LoadEquipmentUpgrade()
    {
        nameText.SetText(selectedEquipment.equipmentName);
        descriptionText.SetText(selectedEquipment.equipmentDescription);
        imageReference.sprite = WeaponManager.Instance.EquipmentCanEvolve(selectedEquipment) ? 
            selectedEquipment.evoSprite : 
            selectedEquipment.equipmentSprite;
        for (var i = 0; i < levelSpriteReference.Length; i++)
        {
            if (i < level)
            {
                levelSpriteReference[i].TryGetComponent(out Image image);
                image.color = Color.white;
            }
            else
            {
                levelSpriteReference[i].TryGetComponent(out Image image);
                image.color = Color.black;
            }
        }
    }

    public void LoadGold(Sprite goldSprite)
    {
        nameText.SetText("Gold");
        imageReference.sprite = goldSprite;
    }
    
    public void ClickGold()
    {
        GameManager.AddCoins(1);
    }
}
