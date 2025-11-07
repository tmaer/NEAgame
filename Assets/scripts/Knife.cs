using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Knife : NetworkBehaviour
{
    // Variables
    private readonly float stabCooldown = 0.3f;
    private readonly float stabAngle = 180;
    private readonly float stabDuration = 0.2f;

    public bool isStabbing = false;
    public Coroutine stabCheck;

    // Networked variables
    private readonly NetworkVariable<Quaternion> knifeRotation = new(
        Quaternion.identity,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private readonly NetworkVariable<Vector3> knifePosition = new(
        Vector3.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    // GameObjects
    public GameObject knife;
    // Other files
    public KnifeScript parentScript;

    void Start()
    {
        if (IsOwner) knife.SetActive(false);
    }
    
    void Update()
    {
        if (!IsOwner)
        {
            transform.SetPositionAndRotation(knifePosition.Value, knifeRotation.Value);
            return;
        }
        if (parentScript.playerCollider == null) return;

        UpdateKnife();

        if (Input.GetMouseButtonDown(0) && stabCheck == null)
        {
            StartStabServerRpc();
        }
    }

    private void UpdateKnife()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate knife to face mouse
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), 0.3f);

        float popDistance = 0.05f;
        if (!parentScript.playerCollider.OverlapPoint(mousePosition))
        {
            Vector2 closestPoint = parentScript.playerCollider.ClosestPoint(mousePosition);
            transform.position = Vector3.Lerp(transform.position, closestPoint, 0.1f) + direction * popDistance;
        }

        knifePosition.Value = transform.position;
        knifeRotation.Value = transform.rotation;
    }

    [ServerRpc]
    private void StartStabServerRpc()
    {
        if (IsOwner && stabCheck == null)
        {
            stabCheck = StartCoroutine(Stab());
            StartStabClientRpc();
        }
    }

    [ClientRpc]
    private void StartStabClientRpc()
    {
        if (!IsOwner && stabCheck == null)
        {
            stabCheck = StartCoroutine(Stab());
        }
    }

    public IEnumerator Stab()
    {
        isStabbing = true;

        float elapsedTime = 0f;
        float startZAngle = transform.eulerAngles.z;
        float targetZAngle = startZAngle + stabAngle;

        // Stab
        while (elapsedTime < stabDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / stabDuration;
            float currentZAngle = Mathf.Lerp(startZAngle, targetZAngle, t);
            transform.rotation = Quaternion.Euler(0f, 0f, currentZAngle);
            yield return null;
        }

        // Reset position
        transform.rotation = Quaternion.Euler(0f, 0f, targetZAngle);

        // Return to Original Position
        elapsedTime = 0f;
        while (elapsedTime < stabDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / stabDuration;
            float currentZAngle = Mathf.Lerp(targetZAngle, startZAngle, t);
            transform.rotation = Quaternion.Euler(0f, 0f, currentZAngle);
            yield return null;
        }

        // Reset position
        transform.rotation = Quaternion.Euler(0f, 0f, startZAngle);
        isStabbing = false;
        yield return new WaitForSeconds(stabCooldown);
        
        stabCheck = null;
    }
}