using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Color weaponColor = new Color(255, 165, 240);
    [SerializeField] private Color equipmentColor = new Color(181, 165, 255);

    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Transform itemLevelPanel;
    [SerializeField] private TMP_Text itemTierText;

    public void SetWeaponDisplay(SelectedWeapon weapon)
    {
        //set and enable weapon icon
        itemIcon.sprite = weapon.weaponSprite;
        itemIcon.enabled = true;

        //enable item level panel
        itemLevelPanel.gameObject.SetActive(true);

        //loop through and set item level icons (circles representing item level)
        for(int i = 0; i < itemLevelPanel.childCount; i++)
        {
            //get level icon
            Image levelIcon = itemLevelPanel.GetChild(i).GetComponent<Image>();

            //set level icon color based on weapon level 
            if(i + 1 <= WeaponManager.Instance.LevelOfLocalWeapon(weapon)) levelIcon.color = weaponColor;
            else levelIcon.color = Color.black;
        }

        //disable item tier text
        itemTierText.enabled = false;
    }

    public void SetEquipmentDisplay(Equipment equipment)
    {
        //set and enable equipment icon
        itemIcon.sprite = equipment.equipmentSprite;
        itemIcon.enabled = true;

        //enable item level panel
        itemLevelPanel.gameObject.SetActive(true);

        //loop through and set item level icons (circles representing item level)
        for(int i = 0; i < itemLevelPanel.childCount; i++)
        {
            //get level icon
            Image levelIcon = itemLevelPanel.GetChild(i).GetComponent<Image>();

            //set level icon color based on equipment level 
            if(i + 1 <= equipment.level) levelIcon.color = equipmentColor;
            else levelIcon.color = Color.black;
        }

        //disable item tier text
        itemTierText.enabled = false;
    }

    public void HideDisplay()
    {
        //hide all UI elements
        itemIcon.enabled = false;
        itemLevelPanel.gameObject.SetActive(false); 
        itemTierText.enabled = false;
    }

    public void SetHatDisplay()
    {
        //set and enable hat icon
        //itemIcon.sprite =
        itemIcon.enabled = false;

        //disable item level panel
        itemLevelPanel.gameObject.SetActive(false);

        //set and enable item tier text
        //itemTierText.text =
        itemTierText.enabled = true;
    }
}
