using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcGenWorld : MonoBehaviour
{
    public GameObject objectToInstantiate; // The object you want to instantiate
    public Vector2 edgeDistance = new Vector2(1.0f, 1.0f); // Distance from the edge to trigger instantiation

    private bool canInstantiate = true;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // Check if the player is near the edge
        if (IsNearEdge())
        {
            if (canInstantiate)
            {
                InstantiateObject();
                canInstantiate = false;
            }
        }
        else
        {
            canInstantiate = true;
        }
    }

    private bool IsNearEdge()
    {
        // Calculate the player's position on the screen
        Vector2 screenPosition = _mainCamera.WorldToScreenPoint(transform.position);
        float screenEdgeX = Screen.width * 0.05f; // Adjust this value as needed.
        float screenEdgeY = Screen.height * 0.05f; // Adjust this value as needed.

        if (screenPosition.x < edgeDistance.x + screenEdgeX || screenPosition.x > Screen.width - edgeDistance.x - screenEdgeX)
        {
            if (screenPosition.y < edgeDistance.y + screenEdgeY || screenPosition.y > Screen.height - edgeDistance.y - screenEdgeY)
            {
                return true;
            }
        }

        return false;
    }

    private void InstantiateObject()
    {
        // Instantiate the object near the player's position
        Vector3 instantiatePosition = transform.position + new Vector3(edgeDistance.x, edgeDistance.y, 0); // You can adjust the direction and distance as needed.
       // Instantiate(objectToInstantiate, instantiatePosition, Quaternion.identity);
       Debug.Log("Spawn ground");
    }
}
