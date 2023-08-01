using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 30;
    [SerializeField] private int energyEarnedPerTimespan = 5;
    [SerializeField] private float secondsToEarnEnergy = 300;

    private DateTime currentTime;

    [Header("Dev Info")]
    [SerializeField] private float rechargeTime = 0f;

    private void Start()
    { 
        AuthenticationService.Instance.SignedIn += LoadEnergy;
    }

    private void LoadEnergy()
    {
        //check if need to give initial 30 energy
        if(PlayerPrefs.GetInt("InitialEnergyGiven", -1) == -1)
        {
            ProfileManager.Instance.ChangeNumEnergy(maxEnergy - ProfileManager.Instance.ProfileInfo.energy);
            PlayerPrefs.SetInt("InitialEnergyGiven", 1);
        }
        else
        {
            //get current time and find difference between last time online
            currentTime = System.DateTime.Now;
            long temp = Convert.ToInt64(PlayerPrefs.GetString("OldTime"));
            DateTime oldTime = DateTime.FromBinary(temp);
            TimeSpan difference = currentTime.Subtract(oldTime);

            //use number of seconds passed to make energy recharge while offline
            float seconds = (float) difference.TotalSeconds + (300 - PlayerPrefs.GetFloat("OldRechargeTime"));
            int energyEarned = energyEarnedPerTimespan * (Mathf.FloorToInt(seconds / secondsToEarnEnergy));
            int energyToAdd = Mathf.Clamp(energyEarned, 0, maxEnergy - ProfileManager.Instance.ProfileInfo.energy);
            ProfileManager.Instance.ChangeNumEnergy(energyToAdd);

            float newRechargeTime = secondsToEarnEnergy; //set new recharge time
            //if didn't fully recharge while offline, update recharge time with leftover seconds
            if(ProfileManager.Instance.ProfileInfo.energy < maxEnergy) newRechargeTime -= seconds % secondsToEarnEnergy;
            StartCoroutine(RechargeEnergy(newRechargeTime)); //start recharge timer
        }
    }

    private IEnumerator RechargeEnergy(float time)
    {
        //wait for time to expire
        rechargeTime = time;
        while(rechargeTime > 0)
        {
            rechargeTime -= Time.deltaTime;
            yield return null;
        }
        
        //determine amount of energy to restore, making sure not to go over 30
        int energyToAdd = Mathf.Clamp(energyEarnedPerTimespan, 0, maxEnergy - ProfileManager.Instance.ProfileInfo.energy);
        ProfileManager.Instance.ChangeNumEnergy(energyToAdd); //restore energy
        yield return RechargeEnergy(secondsToEarnEnergy); //repeat recharge on 5 minute timer
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetString("OldTime", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetFloat("OldRechargeTime", rechargeTime);
    }
}
