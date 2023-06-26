using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpCollider : MonoBehaviour
{
    [SerializeField] private int xpToAdd;
    private bool _moveTowardsPlayer;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("XPPickUpCollider")) return;
        GameManager.Instance.AddXp(xpToAdd);
        _moveTowardsPlayer = true;
    }

    private void FixedUpdate()
    {
        if(!_moveTowardsPlayer) return;
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
        transform.position = Vector3.MoveTowards(transform.position, 
            playerPosition, 
            0.15f);
        if(Vector3.Distance(transform.position, playerPosition) < 0.1f)
            Destroy(gameObject);
    }
}
