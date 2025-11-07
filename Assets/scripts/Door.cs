using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Door : NetworkBehaviour
{
    // Variables
    public float openingDistance = 3f;
    public bool opened = false;
    // GameObjects

    // GameObject accessors
    public SpriteRenderer sprite;
    // Other files
    public Sprite closedDoor;
    public Sprite openDoor;

    private bool GameObjectsNearby()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, openingDistance);
        foreach (Collider2D collider in nearbyObjects)
        {
            if (collider.CompareTag("Monster") || collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        bool gameObjectsNearby = GameObjectsNearby();

        if (gameObjectsNearby && !opened)
        {
            sprite.sprite = openDoor;
            opened = true;
        }
        else if (!gameObjectsNearby && opened)
        {
            sprite.sprite = closedDoor;
            opened = false;
        }
    }
}
