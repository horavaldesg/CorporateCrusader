using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCollider : MonoBehaviour
{
    public int goldToAdd;
    private bool _moveTowardsPlayer;
    private bool _canCollect;
    private SpriteRenderer _spriteRenderer;
    
    
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
            GameManager.AddGold(goldToAdd);
            _canCollect = false;
        }
        
        Destroy(gameObject);
        // Destroy(gameObject, Random.Range(2, 3));
    }
}
