using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class XpCollider : MonoBehaviour
{
    public int xpToAdd;
    private bool _moveTowardsPlayer;
    private bool _canCollect;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        TryGetComponent(out _spriteRenderer);
        _spriteRenderer.color =  Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
    
    
    public void CollectXp()
    {
        _moveTowardsPlayer = true;
        _canCollect = true;
    }

    private void FixedUpdate()
    {
        if(!_moveTowardsPlayer) return;
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
        transform.position = Vector3.MoveTowards(transform.position, 
            playerPosition, 
            0.15f);
        if (!(Vector3.Distance(transform.position, playerPosition) < 0.1f)) return;
        if (_canCollect)
        {
            GameManager.AddXp(xpToAdd);
            _canCollect = false;
        }
        
        Destroy(gameObject);
        // Destroy(gameObject, Random.Range(2, 3));
    }
}
