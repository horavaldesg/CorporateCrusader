using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpCollider : MonoBehaviour
{
    [SerializeField] private int xpToAdd;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        GameManager.Instance.AddXp(xpToAdd);
        Destroy(gameObject);    }
}
