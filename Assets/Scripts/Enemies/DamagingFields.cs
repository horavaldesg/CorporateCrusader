using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingFields : MonoBehaviour
{
    [SerializeField] private float damage;
    private GameObject player;
    private bool isPlayerInside = false;

    private void FixedUpdate()
    {
        if (isPlayerInside == true)
        {
            player.gameObject.TryGetComponent(out PlayerController playerController);
            playerController.TakeDamage(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        player = other.gameObject;
        isPlayerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInside = false;
    }
}
