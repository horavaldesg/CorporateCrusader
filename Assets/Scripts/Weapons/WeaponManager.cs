using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    private WeaponsList _weaponsList;
    [SerializeField] private List<Transform> weaponPositions = new ();
    
    private void Awake()
    {
        Instance = this;
        _weaponsList = Resources.Load<WeaponsList>("Weapons/WeaponsList");
    }


    public void ChooseWeapon(int i)
    {
        var weapon = Instantiate(_weaponsList.weaponList[i]);
        var chosenPos = weaponPositions[GetRandom(weaponPositions.Count)].position;
        weapon.transform.position = new Vector3(chosenPos.x, chosenPos.y, 1);
    }

    private int GetRandom(int i)
    {
        return Random.Range(0, i);
    }
}
