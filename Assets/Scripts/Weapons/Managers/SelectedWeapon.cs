using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedWeapon : MonoBehaviour
{
    public enum Attributes
    {
        Puncture,
        Blunt,
        Chemical,
    }

    public string weaponName;
    [TextArea(5,10)]public string weaponDescription;
    public Sprite weaponSprite;
    public Sprite evoSprite;
    
    public Attributes attribute;
    public Hat.ChosenHat hat;
    
    public float damage;
    [HideInInspector] public int level;

    public float coolDown;
    
    public GameObject instantiatedObject;

    public bool WeaponEvolved
    {
        get;
        private set;
    }
    
    protected virtual void Start()
    {
        Activate();
    }

    protected virtual void Activate()
    {
        StartCoroutine(ActivateCoolDown());
    }

    private IEnumerator ActivateCoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        Activate();
    }

    public void UpgradeWeapon()
    {
        if (level >=6) return;
        level++;
        UpgradeCheck();
    }

    private void UpgradeCheck()
    {
        switch (level)
        {
            case 1:
                Level1Upgrade();
                break;
            case 2:
                Level2Upgrade();
                break;
            case 3:
                Level3Upgrade();
                break;
            case 4:
                Level4Upgrade();
                break;
            case 5:
                Level5Upgrade();
                break;
            case 6:
                EvolveWeapon();
                break;
        }
    }

    protected virtual void Level1Upgrade()
    {
    }
    
    protected virtual void Level2Upgrade()
    {
    }
    
    protected virtual void Level3Upgrade()
    {
    }
    
    protected virtual void Level4Upgrade()
    {
    }
    
    protected virtual void Level5Upgrade()
    {
    }

    protected virtual void EvolveWeapon()
    {
        if(!CheckEvolution()) return;
        WeaponEvolved = true;
        Debug.Log("Weapon Evolved");
    }

    private bool CheckEvolution()
    {
        var canEvolve = false;
        foreach (var _ in HatSelection.Instance.chosenHats.Where(chosenHat => chosenHat == hat))
        {
            canEvolve = true;
        }

        return canEvolve;
    }
}
