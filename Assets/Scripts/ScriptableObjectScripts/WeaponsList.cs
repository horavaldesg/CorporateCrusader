using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponsList : ScriptableObject
{
    public List<GameObject> weaponList = new ();
    public List<GameObject> equipmentList = new ();
}