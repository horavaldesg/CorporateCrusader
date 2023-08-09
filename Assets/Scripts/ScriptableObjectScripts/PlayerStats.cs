using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
   public string playerName;
   public float health;
   public float armor;
   public float damage;
   public float speed;
   public float regenTime;
   public float attackSpeed;
   public float criticalDamage;
   public float criticalSpeed;
   public float pickupRadius;
   public float xpMultiplier;
   public int coinMultiplier;
   public float bulletSpeed;
}
