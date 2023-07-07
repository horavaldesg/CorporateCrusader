using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponStats : ScriptableObject
{
   public enum Attributes
   {
      Puncture,
      Blunt,
      Chemical,
   }

   public Attributes attribute;

   public float damage;
   public float fireRate;
   public float range;
   public float timeAlive;
}
