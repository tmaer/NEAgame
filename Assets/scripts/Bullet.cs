using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class Bullet : MonoBehaviour
{
    // Variables
    private Vector3 mousePosition;
    private Vector3 direction;
    private Vector3 firePoint;

    public float distanceToTravel = 200f;
    public float speed = 5f;  // Constant speed

    private readonly float magnitude = .7f;
    private float angle;
    private float bulletOffsetX;
    private float bulletOffsetY;
    // GameObjects

    // GameObject accessors

    // Other files
    private GameObject gun;

    void Start()
    {
        gun = GameObject.FindGameObjectWithTag("PlayerGun"); // THIS DOESNT KNOW WHICH GUN TO GO TO...

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        direction = (mousePosition - gun.transform.position).normalized;
        firePoint = gun.transform.position;

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bulletOffsetX = Mathf.Cos(angle * Mathf.Deg2Rad) * magnitude;
        bulletOffsetY = Mathf.Sin(angle * Mathf.Deg2Rad) * magnitude;

        transform.position = gun.transform.position + new Vector3(bulletOffsetX, bulletOffsetY, 0);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 1/1000 for bullet sound effect otherwise normal sound
    }

    void Update()
    {
        // Travel until distance travelled is above distanceToTravel
        transform.position += speed * Time.deltaTime * direction;

        if (Vector3.Distance(firePoint, transform.position) >= distanceToTravel) // || player collision)
        {
            // Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.CompareTag("Map"))
        {
            // Destroy(gameObject);
        }
    }
}