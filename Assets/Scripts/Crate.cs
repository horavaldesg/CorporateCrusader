using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D),
   typeof(Rigidbody2D),
   typeof(SpriteRenderer))]
public class Crate : MonoBehaviour
{
   private enum WhatToDrop
   {
      LittleGold = 0,
      ALotGold = 1,
      Magnet = 2,
      Bomb = 3
   }

   [Header("Place in this order (Little Gold, A lot of Gold, Magnet, Bomb)")]
   [SerializeField] private Sprite[] referenceImages;
   [SerializeField] private Sprite defaultImage;
   

   private WhatToDrop _whatToDrop;
   private Collider2D _collider2D;
   private Rigidbody2D _rigidbody2D;
   
   private SpriteRenderer _image;
   
   private void Awake()
   {
      TryGetComponent(out _image);
      TryGetComponent(out _collider2D);
      TryGetComponent(out _rigidbody2D);
      _image.sprite = defaultImage;
      _collider2D.isTrigger = false;
      _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if(!other.CompareTag("Player")) return;
      CheckWhatDropped();
   }

   private void OnCollisionEnter2D(Collision2D other)
   {
      if(!other.collider.CompareTag("Player") || other.collider.CompareTag("Bullet")) return;
      BreakBox();
   }

   private void BreakBox()
   {
      ChooseWhatToDrop();
      _collider2D.isTrigger = true;
   }

   private void CheckWhatDropped()
   {
      switch (_whatToDrop)
      {
         case WhatToDrop.LittleGold:
            break;
         case WhatToDrop.ALotGold:
            break;
         case WhatToDrop.Magnet:
            break;
         case WhatToDrop.Bomb:
            break;
      }

      Destroy(gameObject);
   }

   private void ChooseWhatToDrop()
   {
      var whatToDrop =  Random.Range(0, Enum.GetValues(typeof(WhatToDrop)).Length);
      _whatToDrop = (WhatToDrop)whatToDrop;
      ChangeImage(whatToDrop);
   }

   private void ChangeImage(int i)
   {
      Debug.Log(i);
      _image.sprite = referenceImages[i];
   }
}
