using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectionManager : MonoBehaviour
{
    public bool canSelectStage = true;

    [SerializeField] private List<string> stageNames = new List<string>();
    [SerializeField] private List<Sprite> stageBGSprites = new List<Sprite>();
    [SerializeField] private List<LevelLoader> stageLevelLoaders = new ();

    [SerializeField] private int energyReqPerLevel = 5;

    [Header("UI References")]
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private Image stageBGImage;
    [SerializeField] private GameObject stageLockedPanel;
    [SerializeField] private Button prevStageButton;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private Button stageSelectButton;

    private InGameLevelLoader _inGameLevelLoader;
    
    private int currentStage;

    private void Awake()
    {
        _inGameLevelLoader = Resources.Load<InGameLevelLoader>("InGameLevel");

        //subscribe to energy changed and profile XP changed events
        ProfileManager.Instance.OnEnergyChanged += UpdateStageSelectButton;
        ProfileManager.Instance.OnProfileXPChanged += UpdateStageSelectButton;
    }

    private void Start()
    {
        currentStage = 0;
        prevStageButton.interactable = false;
        stageNameText.text = "Stage " + (currentStage + 1) + ":\n" + stageNames[currentStage];
        stageBGImage.sprite = stageBGSprites[currentStage];
        SetStage();
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

    private void SetStage()
    {
        _inGameLevelLoader.levelLoader = stageLevelLoaders[currentStage];
    }

    private IEnumerator SwitchStage()
    {
        SetStage();

        //disable previous stage, next stage, and stage select buttons
        prevStageButton.enabled = false;
        nextStageButton.enabled = false;
        canSelectStage = false;

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

        //update interactability of stage select button
        UpdateStageSelectButton();

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
        canSelectStage = true;
    }

    private void UpdateStageSelectButton()
    {
        //set locked status of stage (every 5 player levels unlocks a new stage)
        int profileLevel = ProfileManager.Instance.ProfileInfo.profileLevel;
        stageLockedPanel.SetActive(profileLevel < currentStage * 5);
        stageSelectButton.interactable = profileLevel >= currentStage * 5;

        //don't check energy if stage is locked
        if(!stageSelectButton.interactable) return;

        //check if enough energy to play stage and enable/disable button
        int energy = ProfileManager.Instance.ProfileInfo.energy;
        stageSelectButton.interactable = energy >= energyReqPerLevel;
    }
}
