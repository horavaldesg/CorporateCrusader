using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(
    typeof(Rigidbody2D),
    typeof(CircleCollider2D))]
public class GasDamage : MonoBehaviour
{
    [HideInInspector] public float damage;
    
    private void Awake()
    {
        StartCoroutine(ExpandGas());
    }

    private IEnumerator ExpandGas()
    {
        var t = 0.0f;
        while (t < 1)
        {
            transform.localScale = new Vector3(
                transform.localScale.x + 0.15f, 
                transform.localScale.y + 0.15f, 
                1);
            yield return new WaitForSeconds(0.1f);
            t += 0.1f;
        }
        
        Destroy(gameObject, 0.25f);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent(out PlayerController playerController);
        if(!playerController) return;
        playerController.TakeDamage(damage);
    }
}
