using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 30;
    [SerializeField] private int energyEarnedPerTimespan = 5;
    [SerializeField] private float secondsToEarnEnergy = 300;
    [SerializeField] private TMP_Text rechargeTimerText;

    [Header("Energy Panel UI References")]
    [SerializeField] private GameObject energyScreenBG;
    [SerializeField] private Button adForEnergyButton;
    [SerializeField] private Button coinsForEnergyButton;
    [SerializeField] private Button gemsForEnergyButton;
    [SerializeField] private TMP_Text adChancesLeftText;
    [SerializeField] private TMP_Text coinsChancesLeftText;
    [SerializeField] private TMP_Text gemsChancesLeftText;

    private DateTime currentTime;

    [Header("Dev Info")]
    [SerializeField] private float rechargeTime = 0f;
    [SerializeField] private DateTime lastCheckDateTime;

    private void Start()
    { 
        //subscribe to signed in event
        AuthenticationService.Instance.SignedIn += LoadEnergy;

        //if already signed in, load energy
        if(AuthenticationService.Instance.IsSignedIn) LoadEnergy();

        //subscribe to energy changed event
        ProfileManager.Instance.OnEnergyChanged += async () =>
        {
            //if energy panel is active, update it
            if(energyScreenBG.activeSelf) await UpdateEnergyPanel();
        };
    }

    private async void LoadEnergy()
    {
        //check if need to give initial 30 energy
        int initialEnergyGiven = await SaveManager.Instance.LoadSomeInt("InitialEnergyGiven");
        if(initialEnergyGiven == 0)
        {
            ProfileManager.Instance.ChangeNumEnergy(maxEnergy - ProfileManager.Instance.ProfileInfo.energy);
            SaveManager.Instance.SaveSomeData("InitialEnergyGiven", "1");
            StartCoroutine(AttemptRecharge()); //start attempting to recharge
        }
        else
        {
            //get current time and find difference between last time online
            currentTime = System.DateTime.Now;
            long temp = Convert.ToInt64(PlayerPrefs.GetString("OldTime"));
            DateTime oldTime = DateTime.FromBinary(temp);
            TimeSpan difference = currentTime.Subtract(oldTime);

            //use number of seconds passed to make energy recharge while offline
            float seconds = (float) difference.TotalSeconds + (secondsToEarnEnergy - PlayerPrefs.GetFloat("OldRechargeTime"));
            int energyEarned = energyEarnedPerTimespan * (Mathf.FloorToInt(seconds / secondsToEarnEnergy));
            int energyToAdd = Mathf.Clamp(energyEarned, 0, maxEnergy - ProfileManager.Instance.ProfileInfo.energy);
            ProfileManager.Instance.ChangeNumEnergy(energyToAdd);

            float newRechargeTime = secondsToEarnEnergy; //set new recharge time
            //if didn't fully recharge while offline, update recharge time with leftover seconds
            if(ProfileManager.Instance.ProfileInfo.energy < maxEnergy) newRechargeTime -= seconds % secondsToEarnEnergy;
            StartCoroutine(RechargeEnergy(newRechargeTime)); //start recharge timer
        }
    }

    private IEnumerator AttemptRecharge()
    {
        while(ProfileManager.Instance.ProfileInfo.energy == maxEnergy)
        {
            rechargeTimerText.enabled = false; //disable recahrge timer text if player has max energy
            yield return null;;
        }

        yield return RechargeEnergy(secondsToEarnEnergy);
    }

    private IEnumerator RechargeEnergy(float time)
    {
        //set initial recharge time
        rechargeTime = time;
        int minutes = Mathf.FloorToInt(rechargeTime / 60);
        int seconds = Mathf.FloorToInt(rechargeTime % 60);
        rechargeTimerText.text = string.Format("{0}:{1:00}", minutes, seconds); //rechargeTS.Minutes + ":" + rechargeTS.Seconds;

        //loop during recharge time
        while(rechargeTime > 0)
        {
            //decrement recharge time
            rechargeTime -= Time.deltaTime;

            //check if player still needs to recharge energy
            if(ProfileManager.Instance.ProfileInfo.energy == maxEnergy) yield return AttemptRecharge(); //attempt another recharge

            //update recharge timer text
            rechargeTimerText.enabled = true;
            minutes = Mathf.FloorToInt(rechargeTime / 60);
            seconds = Mathf.FloorToInt(rechargeTime % 60);
            rechargeTimerText.text = string.Format("{0}:{1:00}", minutes, seconds);

            yield return null;
        }
        
        //determine amount of energy to restore, making sure not to go over 30
        int energyToAdd = Mathf.Clamp(energyEarnedPerTimespan, 0, maxEnergy - ProfileManager.Instance.ProfileInfo.energy);
        ProfileManager.Instance.ChangeNumEnergy(energyToAdd); //restore energy
        yield return AttemptRecharge(); //attempt another recharge
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetString("OldTime", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetFloat("OldRechargeTime", rechargeTime);
    }

    public async void ToggleEnergyPanel()
    {
        //if panel is disabled, update it before enabling it
        if(!energyScreenBG.activeSelf)
        {
            await UpdateChancesLeft();
            await UpdateEnergyPanel();
        }

        //toggle visibility of energy panel
        energyScreenBG.SetActive(!energyScreenBG.activeSelf);
    }

    private async Task UpdateChancesLeft()
    {
        //get last check date
        string lastCheckDate = await SaveManager.Instance.LoadSomeString("LastEnergyCheckDate");
        
        //check if this is first time checking energy panel
        if(lastCheckDate == null) ResetChancesLeft(); //if so, reset all chances left to 3
        else
        {
            //if not, load last check date
            long temp = Convert.ToInt64(lastCheckDate);
            lastCheckDateTime = DateTime.FromBinary(temp);

            //reset all chances left to 3 if on a different date than last date
            if(DateTime.Compare(lastCheckDateTime.Date, System.DateTime.Now.Date) != 0) ResetChancesLeft();
        }

        //update last check date with current date
        SaveManager.Instance.SaveSomeData("LastEnergyCheckDate", System.DateTime.Now.ToBinary().ToString());
    }

    private async Task UpdateEnergyPanel()
    {
        //load chances left
        int adChancesLeft = await SaveManager.Instance.LoadSomeInt("AdForEnergyChancesLeft");
        int coinsChancesLeft = await SaveManager.Instance.LoadSomeInt("CoinsForEnergyChancesLeft");
        int gemsChancesLeft = await SaveManager.Instance.LoadSomeInt("GemsForEnergyChancesLeft");

        //update watch ad for energy button and chances left text
        adForEnergyButton.interactable = (adChancesLeft > 0) 
        && (ProfileManager.Instance.ProfileInfo.energy <= maxEnergy - 5);
        adChancesLeftText.text = "Chances left today: " + adChancesLeft;
        
        //update coins for energy button and chances left text
        coinsForEnergyButton.interactable = (coinsChancesLeft > 0) 
        && (ProfileManager.Instance.ProfileInfo.energy <= maxEnergy - 10)
        && (ProfileManager.Instance.ProfileInfo.coins >= 100);
        coinsChancesLeftText.text = "Chances left today: " + coinsChancesLeft;

        //update gems for energy button and chances left text
        gemsForEnergyButton.interactable = (gemsChancesLeft > 0)
        && (ProfileManager.Instance.ProfileInfo.energy <= maxEnergy - 15)
        && (ProfileManager.Instance.ProfileInfo.gems >= 25);
        gemsChancesLeftText.text = "Chances left today: " + gemsChancesLeft;
    }

    //reset all chances left to 3 and save
    public void ResetChancesLeft()
    {
        SaveManager.Instance.SaveSomeData("AdForEnergyChancesLeft", "3");
        SaveManager.Instance.SaveSomeData("CoinsForEnergyChancesLeft", "3");
        SaveManager.Instance.SaveSomeData("GemsForEnergyChancesLeft", "3");
    }

    public void AdForEnergyButton()
    {
        
    }

    public async void CoinsForEnergyButton()
    {
        int coinsChancesLeft = await SaveManager.Instance.LoadSomeInt("CoinsForEnergyChancesLeft") - 1; //get previous chances left and decrement by 1
        SaveManager.Instance.SaveSomeData("CoinsForEnergyChancesLeft", coinsChancesLeft.ToString()); //save chances left
        ProfileManager.Instance.ChangeNumCoins(-100); //subtract coins
        ProfileManager.Instance.ChangeNumEnergy(10); //add energy
    }

    public async void GemsForEnergyButton()
    {
        int gemsChancesLeft = await SaveManager.Instance.LoadSomeInt("GemsForEnergyChancesLeft") - 1; //get previous chances left and decrement by 1
        SaveManager.Instance.SaveSomeData("GemsForEnergyChancesLeft", gemsChancesLeft.ToString()); //save chances left
        ProfileManager.Instance.ChangeNumGems(-25); //subtract gems
        ProfileManager.Instance.ChangeNumEnergy(15); //add energy
    }

    public void ChangeEnergyDevButton(int amount)
    {
        int energyToChange = amount;
        if(energyToChange > maxEnergy - ProfileManager.Instance.ProfileInfo.energy) energyToChange = maxEnergy - ProfileManager.Instance.ProfileInfo.energy;
        if(ProfileManager.Instance.ProfileInfo.energy + energyToChange < 0) energyToChange = 0;
        ProfileManager.Instance.ChangeNumEnergy(energyToChange);
    }
}
