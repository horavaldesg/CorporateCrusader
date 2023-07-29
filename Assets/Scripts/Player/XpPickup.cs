using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpPickup : MonoBehaviour
{
    private const string XpCollider = "XPCollider";
    private const string Gold = "GoldCollider";
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case XpCollider:
                CollectXp(other);
                break;
            case Gold:
                CollectGold(other);
                break;
        }
    }

    private void CollectXp(Collider2D other)
    {
        other.TryGetComponent(out XpCollider xpCollider);
        xpCollider.CollectXp();
    }

    private void CollectGold(Collider2D other)
    {
        other.TryGetComponent(out GoldCollider goldCollider);
        goldCollider.CollectXp();
    }
}
