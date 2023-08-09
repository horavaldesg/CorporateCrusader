using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 30;
    [SerializeField] private int energyEarnedPerTimespan = 5;
    [SerializeField] private float secondsToEarnEnergy = 300;
    [SerializeField] private TMP_Text rechargeTimerText;

    private DateTime currentTime;

    [Header("Dev Info")]
    [SerializeField] private float rechargeTime = 0f;

    private void Start()
    { 
        //subscribe to signed in event
        AuthenticationService.Instance.SignedIn += LoadEnergy;

        //if already signed in, load energy
        if(AuthenticationService.Instance.IsSignedIn) LoadEnergy();
    }

    public void ChangeEnergyDevButton(int amount)
    {
        int energyToChange = amount;
        if(energyToChange > maxEnergy - ProfileManager.Instance.ProfileInfo.energy) energyToChange = maxEnergy - ProfileManager.Instance.ProfileInfo.energy;
        if(ProfileManager.Instance.ProfileInfo.energy + energyToChange < 0) energyToChange = 0;
        ProfileManager.Instance.ChangeNumEnergy(energyToChange);
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
}
