using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeProjectile : MonoBehaviour
{
   private Rigidbody2D _rb;
   [SerializeField]private float rotationMax;
   private float _currentRotation;
   [HideInInspector] public float Damage;
   private Collider2D _collider2D;

   private void Awake()
   {
      TryGetComponent(out _rb);
      Destroy(gameObject, 10);
   }

   private void Start()
   {
      TryGetComponent(out _collider2D);
      rotationMax = 10;
   }

   private void FixedUpdate()
   {
      var rotation = transform.eulerAngles;
     // _currentRotation = Mathf.PingPong(Time.time, rotationMax);
      rotation.z += Time.deltaTime * -90 * rotationMax;
      transform.localEulerAngles = rotation;
      // _collider2D.enabled = !(_currentRotation < 3.5f);
   }

   public void ShootBlades(Vector3 force)
   {
      _rb.AddForce(force);
   }

   private void OnTriggerStay2D(Collider2D other)
   {
      if (!other.CompareTag("Enemy")) return;
      other.TryGetComponent(out Enemy enemy);
      StartCoroutine(TakeDamageWithTimer(enemy));
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if (!other.CompareTag("Enemy")) return;
      StopAllCoroutines();
   }

   private IEnumerator TakeDamageWithTimer(Enemy enemy)
   {
      yield return new WaitForSeconds(0.2f);
      enemy.TakeDamage(Damage);
   }
}
