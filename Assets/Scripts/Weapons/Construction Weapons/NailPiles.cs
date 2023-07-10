using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class NailPiles : SelectedWeapon
{
    [SerializeField] private GameObject nail;
    public float fireRate;
}
