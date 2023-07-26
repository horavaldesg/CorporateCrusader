using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JerryCanThrowable : MonoBehaviour
{
   private Rigidbody2D _rigidbody2D;

   public float Damage
   {
      get;
      set;
   }
   private void Awake()
   {
      TryGetComponent(out _rigidbody2D);
      StartCoroutine(StopFalling());
   }

   private IEnumerator StopFalling()
   {
      yield return new WaitForSeconds(0.5f);
      _rigidbody2D.gravityScale = 0;
   }
   
   private void OnTriggerEnter2D(Collider2D other)
   {
      if(!other.CompareTag("Enemy"))return;
      other.TryGetComponent(out Enemy enemy);
      enemy.TakeDamage(Damage);
      Destroy(gameObject);
   }
}
