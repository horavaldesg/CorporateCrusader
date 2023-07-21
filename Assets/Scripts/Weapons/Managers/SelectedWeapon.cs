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
    public string weaponDescription;
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

    protected virtual IEnumerator ActivateCoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        Activate();
    }

    public void UpgradeWeapon()
    {
        level++;
    }
}
