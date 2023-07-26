using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public Attributes attribute;

    public float damage;
    public int level;

    public float coolDown;
    
    public GameObject instantiatedObject;

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
}
