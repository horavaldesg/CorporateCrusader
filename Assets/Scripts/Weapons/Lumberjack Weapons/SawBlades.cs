using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlades : SelectedWeapon
{
   [SerializeField]private List<Vector3> blades = new List<Vector3>(4);
   private List<GameObject> bladesTransforms = new List<GameObject>(4);
   private float _distanceFromPlayer;
   private bool _bladesLoaded;
   

   protected override void Start()
   {
      transform.parent = null;
      for (var i = 0; i < 4; i++)
      {
         var newDirections = new Vector3();
         blades.Add(newDirections);
      }
      
      _distanceFromPlayer = 150;
      
      base.Start();
   }

   protected override void Activate()
   {
      ActivateBlades();
      base.Activate();
   }

   
   private void FixedUpdate()
   {
      var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;

      transform.position = Vector2.Lerp(transform.position, playerPos, 0.5f);
   }
   

   private void ActivateBlades()
   {
      blades[0] = transform.right * _distanceFromPlayer;
      blades[1] = transform.right * -_distanceFromPlayer;
      blades[2] = transform.up * _distanceFromPlayer;
      blades[3] = transform.up * -_distanceFromPlayer;
      
      for (var i = 0; i < 4; i++)
      {
         var go = Instantiate(instantiatedObject);
         go.transform.position = transform.position;
         go.TryGetComponent(out SawBladeProjectile sawBladeProjectile);
         sawBladeProjectile.Damage = damage;
         sawBladeProjectile.ShootBlades(blades[i]);
      }
   }
}
