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

   [Header("Pickup Variables")] [SerializeField]
   private int littleCoinsToAdd;

   [SerializeField] private int aLotOfCoinsToAdd;
   [SerializeField] private float pickupRadius;
   

   [Header("Place in this order (Little Gold, A lot of Gold, Magnet, Bomb)")] [SerializeField] 
   private Sprite[] referenceImages;
   
   [SerializeField] private Sprite defaultImage;
   

   private WhatToDrop _whatToDrop;
   private Collider2D _collider2D;
   private Rigidbody2D _rigidbody2D;
   
   private SpriteRenderer _image;
   private List<Vector2> _imageSizes = new();
   
  
   private const string CrateTag = "Crate";
   private const string PickUpTag = "PickUp";
   
   
   private void Awake()
   {
      TryGetComponent(out _image);
      TryGetComponent(out _collider2D);
      TryGetComponent(out _rigidbody2D);
      _image.sprite = defaultImage;
      _collider2D.isTrigger = true;
      _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
      gameObject.tag = CrateTag;
      _image.size = new Vector2(1, 1);
      for (var i = 0; i < 4; i++)
      {
         _imageSizes.Add(Vector2.zero);
      }
      
      _imageSizes[0] = new Vector2(1.5f, 1);
      _imageSizes[1] = new Vector2(1.5f, 1);
      _imageSizes[2] = new Vector2(0.75f, 0.75f);
      _imageSizes[3] = new Vector2(0.8f, 0.8f);
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if(!other.CompareTag("Player")) return;
      (gameObject.CompareTag("Crate") ? (Action)BreakBox : CheckWhatDropped)();
   }

 
   public void BreakBox()
   {
      ChooseWhatToDrop();
      gameObject.tag = PickUpTag;
   }

   private void CheckWhatDropped()
   {
      switch (_whatToDrop)
      {
         case WhatToDrop.LittleGold:
         {
            GameManager.AddCoins(littleCoinsToAdd);
         }
            break;
         case WhatToDrop.ALotGold:
         {
            GameManager.AddCoins(littleCoinsToAdd);
         }
            break;
         case WhatToDrop.Magnet:
            //Call function in player
            XpPickup.Instance.PickupPowerUp(pickupRadius);
            break;
         case WhatToDrop.Bomb:
            //Kill all enemies alive
            GameManager.Instance.KillAllEnemies();
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
     // _image.size = _imageSizes[i];
      _image.sprite = referenceImages[i];
   }

   private void OnDestroy()
   {
      GameManager.Instance.RemoveCrate(gameObject);
   }
}
