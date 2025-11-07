using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
    // Variables
    private Vector3 mousePosition;
    // GameObjects
    public GameObject bullet;
    // GameObject accessors
    private Collider2D playerCollider;
    // Other files

    public void SpawnBullet()
    {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // If mouse not on player then shootah
        if (!playerCollider.OverlapPoint(mousePosition))
        {
            // Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
        }
    }
}
