using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpPickup : MonoBehaviour
{
    private const string XpCollider = "XPCollider";
    private const string Coins = "CoinCollider";
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case XpCollider:
                CollectXp(other);
                break;
            case Coins:
                CollectCoins(other);
                break;
        }
    }

    private void CollectXp(Collider2D other)
    {
        other.TryGetComponent(out XpCollider xpCollider);
        xpCollider.CollectXp();
    }

    private void CollectCoins(Collider2D other)
    {
        other.TryGetComponent(out CoinCollider coinCollider);
        coinCollider.CollectXp();
    }
}
