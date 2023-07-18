using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HatSelectionManager : MonoBehaviour
{
    [SerializeField] private List<Image> selectedHatIcons;
    [SerializeField] private Sprite questionMarkIcon;
    [SerializeField] private GameObject hatSelectionBG;
    [SerializeField] private RectTransform hatSelectionButtons;
    [SerializeField] private RectTransform hatSelectionOutline;
    [SerializeField] private RectTransform hatInfoPanel;
    [SerializeField] private Button startButton;
    private Hat _chosenHats;

    private int currentHatSlot;
    private HatCollectionButton[] selectedHats;
    private HatCollectionButton currentHat;

    private void OnEnable()
    {
        _chosenHats = Resources.Load<Hat>("PlayerStats/ChosenHats");
        _chosenHats.chosenHats.Clear();
        //reset selected hats and start button
        selectedHats = new HatCollectionButton[6];
        foreach(Image i in selectedHatIcons) i.sprite = questionMarkIcon;
        startButton.interactable = false;
    }

    public void SelectedHatButton(int index)
    {
        //update current hat slot
        currentHatSlot = index;

        //enable all hat selection buttons
        foreach(Transform t in hatSelectionButtons) t.GetComponent<Button>().interactable = true;

        //update hat buttons based on hats already selected
        for(int i = 0; i < selectedHats.Length; i++)
        {
            if(i == index) continue; //skip the current slot, as it is being changed

            if(selectedHats[i] == null) continue; //skip empty slots

            //for hats that are already selected and not the current slot, make their buttons uninteractable
            foreach(Transform t in hatSelectionButtons)
            {
                HatCollectionButton button = t.GetComponent<HatCollectionButton>();
                if(button == selectedHats[i]) t.GetComponent<Button>().interactable = false;
            }
        }

        //if slot already had a hat selected, set it to selected in the hat selection screen
        if(selectedHats[index] != null) currentHat = selectedHats[index];
        else if(currentHat == null) currentHat = hatSelectionButtons.GetChild(0).GetComponent<HatCollectionButton>();
        else
        {
            //else if slot was empty, make sure current hat isn't uninteractable, otherwise change current hat
            int newHatIndex = 0;
            while(!currentHat.GetComponent<Button>().interactable)
            {
                currentHat = hatSelectionButtons.GetChild(newHatIndex).GetComponent<HatCollectionButton>();
                newHatIndex++;
            }
        }

        //enable hat selection screen and update selection
        hatSelectionBG.SetActive(true);
        UpdateSelectedHat();
    }

    public void HatSelectionBGButton() => hatSelectionBG.SetActive(false);

    public void HatSelectionButton(HatCollectionButton button)
    {
        currentHat = button;
        UpdateSelectedHat();
    }

    public void SelectButton()
    {
        //select current hat and set its icon
        selectedHats[currentHatSlot] = currentHat;
        selectedHatIcons[currentHatSlot].sprite = currentHat.HatIcon;

        //disable hat selection screen
        hatSelectionBG.SetActive(false);

        //enable start button if all hats have been selected
        for(int i = 0; i < selectedHats.Length; i++)
        {
            //break if any hat slots are empty
            if(selectedHats[i] == null) break;

            //enable start button after checking last slot
            if(i == selectedHats.Length - 1) startButton.interactable = true;
        }
        
        _chosenHats.chosenHats.Add(currentHat.chosenHat);
    }

    private void UpdateSelectedHat()
    {
        //update button selection outline
        hatSelectionOutline.SetParent(currentHat.transform);
        hatSelectionOutline.localScale = Vector3.one;
        hatSelectionOutline.anchoredPosition = Vector3.zero;

        //update UI elements of hat info panel
        hatInfoPanel.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = currentHat.HatName;
        hatInfoPanel.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Tier " + currentHat.HatTier;
        hatInfoPanel.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = currentHat.HatDescription;
        hatInfoPanel.GetChild(0).GetChild(3).GetComponent<Image>().sprite = currentHat.HatIcon;
    }
}
