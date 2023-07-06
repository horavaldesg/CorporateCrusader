using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
   public float health;
   public float damage;
   public float speed;
   public float regenTime;
   public float attackSpeed;
   public float criticalDamage;
   public float criticalSpeed;
   public float pickupRadius;
   public float xpMultiplier;
}
