using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : MonoBehaviour
{
   [HideInInspector] public float damage;

   private void OnTriggerEnter2D(Collider2D other)
   {
      if(!other.CompareTag("Enemy"))return;
      other.TryGetComponent(out Enemy enemy);
      enemy.TakeDamage(damage);
      Destroy(gameObject);
   }
}
