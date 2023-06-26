using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Update()
    {
        var playerPosition = PlayerController.Instance.CurrentPlayerTransform().position;
        transform.position = new Vector3(playerPosition.x, playerPosition.y, transform.position.z);
    }
}
