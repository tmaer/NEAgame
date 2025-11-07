using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Gun : NetworkBehaviour
{

    // bullet magazine, reloading


    // Variables

    // GameObjects
    public GameObject gun;
    // GameObject accessors
    private Collider2D playerCollider;
    // Other files
    private BulletSpawner bulletSpawner;

    private void Awake()
    {
        bulletSpawner = FindObjectsOfType<GameObject>().FirstOrDefault(static obj => obj.layer == 10)?.GetComponent<BulletSpawner>();
    }
    void Start()
    {
        gun.SetActive(false);
    }

    public void AssignPlayer(GameObject player)
    {
        playerCollider = player.GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate gun to face mouse
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 0.1f);

        if (!playerCollider.OverlapPoint(mousePosition))
        {
            Vector2 closestPoint = playerCollider.ClosestPoint(mousePosition);
            transform.position = Vector3.Lerp(transform.position, closestPoint, 0.1f);
        }

        // Shootah
        if (Input.GetMouseButtonDown(0))
        {
            bulletSpawner.SpawnBullet();
        }
    }
}
