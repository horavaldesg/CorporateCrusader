using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JerryCanThrowable : MonoBehaviour
{
   [SerializeField] private SpriteRenderer sR;
   [SerializeField] private Sprite oilPuddleSprite;
   [HideInInspector] public SelectedWeapon.Attributes attributes;
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
      _rigidbody2D.velocity = Vector2.zero;
      sR.sprite = oilPuddleSprite;

      //fade out oil puddle
      float a = 1;
      while(a > 0)
      {
         a -= Time.deltaTime / 10; //oil puddle stays on ground for 10 seconds
         sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, a);
         yield return null;
      }
      Destroy(gameObject);
   }
   
   private void OnTriggerStay2D(Collider2D other)
   {
      if(!other.CompareTag("Enemy"))return;
      other.TryGetComponent(out Enemy enemy);
      enemy.speed = Damage;
      enemy.TakeDamage(Damage, attributes);
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if(!other.CompareTag("Enemy"))return;
      other.TryGetComponent(out Enemy enemy);
      enemy.SlowDownEnemy();
   }
}
