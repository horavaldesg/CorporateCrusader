using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHoseBullet : Bullet
{
    private SpriteRenderer sr;
    private float alpha = 1;

    private void Start() => sr = GetComponentInChildren<SpriteRenderer>();

    private void Update()
    {
        alpha -= Time.deltaTime / timeAlive;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
    }
}
