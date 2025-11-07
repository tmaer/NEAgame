using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
    // Variables
    Vector3 direction;

    private int keysPressed;
    private bool wKeyPressed, aKeyPressed, sKeyPressed, dKeyPressed;

    public int itemsReadyToEquip = 0;

    private bool recentlyDiagonal;
    private Coroutine equipAntiSpam = null;
    private Coroutine diagonalGraceCoroutine;
    public Coroutine dashCheck = null;

    public float equipCooldown = 0.5f, diagonalGracePeriod = 0.05f, dashCooldown = 2f;
    public float speed = 7f;

    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    // GameObjects
    public GameObject parent;
    public GameObject player;
    private GameObject closestParent;

    public GameObject inventory;
    public GameObject droppedGameObject;
    // GameObject accessors
    // Network variables
    private readonly NetworkVariable<Quaternion> networkPlayerRotation = new(
            Quaternion.identity,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
    public NetworkVariable<ulong> knifeNetworkObjectId = new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
    // Other files
    private Inventory inventoryScript;

    private void Awake()
    {
        inventoryScript = inventory.GetComponent<Inventory>();
        knifeNetworkObjectId.OnValueChanged += NetworkAddKnifeToInventory;
    }

    public override void OnNetworkSpawn()
    {
        networkPlayerRotation.OnValueChanged += (oldValue, newValue) =>
        {
            transform.rotation = newValue;
        };
        if (IsOwner)
        {

        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleInput();
        HandleMovement();
        HandleEquip();
        HandleDash();
        HandleInventory();
    }

    private void HandleInput()
    {
        // Check input keys
        wKeyPressed = Input.GetKey(KeyCode.W);
        aKeyPressed = Input.GetKey(KeyCode.A);
        sKeyPressed = Input.GetKey(KeyCode.S);
        dKeyPressed = Input.GetKey(KeyCode.D);

        direction = Vector3.zero;
        if (wKeyPressed) direction += Vector3.up;
        if (aKeyPressed) direction += Vector3.left;
        if (sKeyPressed) direction += Vector3.down;
        if (dKeyPressed) direction += Vector3.right;

        keysPressed = (wKeyPressed ? 1 : 0) + (aKeyPressed ? 1 : 0) + (sKeyPressed ? 1 : 0) + (dKeyPressed ? 1 : 0);

        if (keysPressed == 1 && recentlyDiagonal)
        {
            diagonalGraceCoroutine ??= StartCoroutine(DiagonalTimer());
        }
    }

    private IEnumerator DiagonalTimer()
    {
        yield return new WaitForSeconds(diagonalGracePeriod);
        recentlyDiagonal = false;
        diagonalGraceCoroutine = null;
    }

    private void HandleMovement()
    {
        if (direction != Vector3.zero)
        {
            direction.Normalize(); // Normalize to keep consistent speed

            // Check if player is walking into wall
            RaycastHit2D hitVertical = Physics2D.Raycast(transform.position, new Vector2(0, direction.y), 0.1f, LayerMask.GetMask("Map"));
            RaycastHit2D hitHorizontal = Physics2D.Raycast(transform.position, new Vector2(direction.x, 0), 0.1f, LayerMask.GetMask("Map"));

            bool canMoveVertically = (hitVertical.collider == null);
            bool canMoveHorizontally = (hitHorizontal.collider == null);

            Vector3 newPosition = parent.transform.position;

            // Apply movement based on the allowed directions
            if (canMoveVertically)
            {
                newPosition.y += direction.y * speed * Time.deltaTime;
            }

            if (canMoveHorizontally)
            {
                newPosition.x += direction.x * speed * Time.deltaTime;
            }

            // Update position
            parent.transform.position = newPosition;
        }

        Quaternion newRotation = FindRotation();

        // Only update the network variable if the rotation has changed
        if (newRotation != transform.rotation)
        {
            transform.rotation = newRotation;
            networkPlayerRotation.Value = newRotation;
        }
    }

    public Quaternion FindRotation()
    {
        // assigns diagonal rotation
        if (keysPressed == 2)
        {
            recentlyDiagonal = true;
            StopGrace();

            if (wKeyPressed && aKeyPressed) return Quaternion.Euler(0, 0, 45);
            if (aKeyPressed && sKeyPressed) return Quaternion.Euler(0, 0, 135);
            if (sKeyPressed && dKeyPressed) return Quaternion.Euler(0, 0, -135);
            if (dKeyPressed && wKeyPressed) return Quaternion.Euler(0, 0, -45);
        }
        // assigns straight rotation
        else
        {
            if (keysPressed == 1)
            {
                if (recentlyDiagonal) return transform.rotation;
                if (wKeyPressed) return Quaternion.Euler(0, 0, 0);
                if (aKeyPressed) return Quaternion.Euler(0, 0, 90);
                if (sKeyPressed) return Quaternion.Euler(0, 0, 180);
                if (dKeyPressed) return Quaternion.Euler(0, 0, -90);
            }
            else if (keysPressed == 3)
            {
                if (wKeyPressed && aKeyPressed && sKeyPressed) return Quaternion.Euler(0, 0, 90);
                if (aKeyPressed && sKeyPressed && dKeyPressed) return Quaternion.Euler(0, 0, 180);
                if (sKeyPressed && dKeyPressed && wKeyPressed) return Quaternion.Euler(0, 0, -90);
                if (dKeyPressed && wKeyPressed && aKeyPressed) return Quaternion.Euler(0, 0, 0);
            }
        }
        return transform.rotation;
    }

    private void StopGrace()
    {
        if (diagonalGraceCoroutine != null)
        {
            StopCoroutine(diagonalGraceCoroutine);
            diagonalGraceCoroutine = null;
        }
    }

    private void HandleEquip()
    {
        if (IsOwner && Input.GetKey(KeyCode.E) && itemsReadyToEquip > 0 && equipAntiSpam == null)
        {
            FindClosestParentObject();
            if (closestParent != null)
            {
                // Debug.Log(closestParent.tag);
                IItemParent closestParentScript = closestParent.GetComponent<IItemParent>();
                closestParentScript.Equip(player);
                inventoryScript.AddToInventory(closestParent.GetComponent<NetworkObject>().NetworkObjectId, parent);
                equipAntiSpam = StartCoroutine(EquipTimer());
            }
        }

        if (IsOwner && Input.GetKey(KeyCode.G))
        {
            droppedGameObject = inventoryScript.DropInventory();
        }
    }

    private IEnumerator EquipTimer()
    {
        yield return new WaitForSeconds(equipCooldown);
        equipAntiSpam = null;
    }

    private void FindClosestParentObject()
    {
        GameObject[] parents = GameObject.FindObjectsOfType<GameObject>(true);
        GameObject nearestParent = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject itemParent in parents)
        {
            if (itemParent.layer == 3)
            {
                float distanceToParent = Vector3.Distance(parent.transform.position, itemParent.transform.position);

                // Debug.Log(distanceToParent);
                if (distanceToParent < closestDistance)
                {
                    closestDistance = distanceToParent;
                    nearestParent = itemParent;
                }
            }
        }
        closestParent = nearestParent;
        // Debug.Log(closestParent);
    }

    public void AddKnifeToInventory(GameObject knife)
    {
        if (!IsOwner) return;
        knifeNetworkObjectId.Value = knife.GetComponent<NetworkObject>().NetworkObjectId;
    }

    public void NetworkAddKnifeToInventory(ulong previous, ulong current)
    {
        inventoryScript.AddToInventory(current, parent);
    }

    public void IncreaseEquipableItemCount()
    {
        //Debug.Log("Increase");
        itemsReadyToEquip++;
    }

    public void DecreaseEquipableItemCount()
    {
        //Debug.Log("Decrease");
        itemsReadyToEquip = Mathf.Max(0, itemsReadyToEquip - 1);
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCheck == null && inventoryScript.InventoryCheck() == "Knife")
        {
            StartCoroutine(Dash());
            dashCheck = StartCoroutine(DashTimer());
        }
    }

    private IEnumerator DashTimer()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashCheck = null;
    }

    public IEnumerator Dash()
    {
        if (direction != Vector3.zero)
        {
            direction.Normalize();

            float startTime = Time.time;
            Vector3 originalPosition = parent.transform.position;

            while (Time.time < startTime + dashDuration)
            {
                RaycastHit2D hitVertical = Physics2D.Raycast(transform.position, new Vector2(0, direction.y), 0.1f, LayerMask.GetMask("Map"));
                RaycastHit2D hitHorizontal = Physics2D.Raycast(transform.position, new Vector2(direction.x, 0), 0.1f, LayerMask.GetMask("Map"));

                bool canMoveVertically = hitVertical.collider == null;
                bool canMoveHorizontally = hitHorizontal.collider == null;

                Vector3 dashPosition = parent.transform.position;

                if (canMoveVertically)
                {
                    dashPosition.y += direction.y * dashSpeed * Time.deltaTime;
                }
                if (canMoveHorizontally)
                {
                    dashPosition.x += direction.x * dashSpeed * Time.deltaTime;
                }

                parent.transform.position = dashPosition;

                // Stop dash early if a wall is hit
                if (!canMoveVertically && !canMoveHorizontally)
                {
                    break; // Exit dash if both directions are blocked
                }

                yield return null;
            }
        }
    }

    public void HandleInventory()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventoryScript.invSlot = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventoryScript.invSlot = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventoryScript.invSlot = 2;
        }
    }
}