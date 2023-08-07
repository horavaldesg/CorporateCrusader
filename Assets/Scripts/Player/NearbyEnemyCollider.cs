using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyEnemyCollider : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        other.TryGetComponent(out Enemy enemy);
        if(!enemy) return;
        PlayerController.Instance.nearbyEnemies.Remove(enemy);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        other.TryGetComponent(out Enemy enemy);
        if(!enemy) return;
        if(PlayerController.Instance.HasShield)
        {
            enemy._damage = PlayerController.Instance.NewEnemyDamage(enemy);
        }

        if (PlayerController.Instance.HaseBinoculars)
        {
            enemy.health = PlayerController.Instance.NewEnemyHealth(enemy);
        }
    }
}
