using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectionManager : MonoBehaviour
{
    [SerializeField] private List<string> stageNames = new List<string>();
    [SerializeField] private List<Sprite> stageBGSprites = new List<Sprite>();

    [Header("UI References")]
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private Image stageBGImage;
    [SerializeField] private GameObject stageLockedPanel;
    [SerializeField] private Button prevStageButton;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private Button stageSelectButton;

    private int currentStage;

    private void Start()
    {
        currentStage = 0;
        prevStageButton.interactable = false;
        stageNameText.text = "Stage " + (currentStage + 1) + ":\n" + stageNames[currentStage];
        stageBGImage.sprite = stageBGSprites[currentStage];
    }

    public void PreviousStageButton()
    {
        currentStage--;
        StartCoroutine(SwitchStage());
    }

    public void NextStageButton()
    {
        currentStage++;
        StartCoroutine(SwitchStage());
    }

    private IEnumerator SwitchStage()
    {
        //disable previous stage, next stage, and stage select buttons
        prevStageButton.enabled = false;
        nextStageButton.enabled = false;
        stageSelectButton.enabled = false;

        //fade out stage name and background image
        float a = 1;
        while(a > 0)
        {
            a -= Time.deltaTime * 3;
            if(a < 0) a = 0;
            Color nameColor = new Color(0, 0, 0, a);
            Color bgColor = new Color(1, 1, 1, a);
            stageNameText.color = nameColor;
            stageBGImage.color = bgColor;
            yield return null;
        }

        //set new stage name and background image
        stageNameText.text = "Stage " + (currentStage + 1) + ":\n" + stageNames[currentStage];
        stageBGImage.sprite = stageBGSprites[currentStage];

        //update interactability of previous and next stage buttons
        prevStageButton.interactable = currentStage != 0;
        nextStageButton.interactable = currentStage != stageNames.Count - 1;

        int playerLevel = 5; //<-NOTE: reference global player level variable here in the future

        //set locked status of stage (every 5 player levels unlocks a new stage)
        stageLockedPanel.SetActive(playerLevel < currentStage * 5);
        stageSelectButton.interactable = playerLevel >= currentStage * 5;

        //fade in stage name and background image
        while(a < 1)
        {
            a += Time.deltaTime * 3;
            if(a > 1) a = 1;
            Color nameColor = new Color(0, 0, 0, a);
            Color bgColor = new Color(1, 1, 1, a);
            stageNameText.color = nameColor;
            stageBGImage.color = bgColor;
            yield return null;
        }

        //enable previous stage, next stage, and stage select buttons
        prevStageButton.enabled = true;
        nextStageButton.enabled = true;
        stageSelectButton.enabled = true;
    }
}
