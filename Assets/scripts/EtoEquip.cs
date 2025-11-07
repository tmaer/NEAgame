using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EtoEquip : NetworkBehaviour
{
    // Variables
    public Vector3 spawnPosition;
    public float equipDistance = 4f;
    // GameObjects
    private GameObject[] playerPositions;
    // GameObject accessors
    // Network variables
    private readonly NetworkVariable<int> showingEquip = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<bool> shouldDespawn = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    // Other files

    private void Start()
    {
        /* Aware of issue:
         * if this EtoEquip gameobject is spawned
         * and new player joins this will not interact with said player
         */
        playerPositions = GameObject.FindGameObjectsWithTag("Player Parent");
        spawnPosition = transform.position + new Vector3(0, -.7f, 0);
    }

    private void Update()
    {
        // Server checks how many players within equip range
        if (IsServer) ServerUpdate();
        // Check for Key E down, despawn if pressed within range
        if (showingEquip.Value > 0) ClientHandleInput();
        // Despawn if any conditions met
        if (shouldDespawn.Value) DespawnEtoEquip();
    }

    private void ServerUpdate()
    {
        int playersInRange = 0;

        foreach (GameObject position in playerPositions)
        {
            float distance = Vector3.Distance(position.transform.position, spawnPosition);
            if (distance < equipDistance)
            {
                playersInRange++;
            }
        }

        showingEquip.Value = playersInRange;

        if (showingEquip.Value == 0)
        {
            shouldDespawn.Value = true;
        }
    }

    private void ClientHandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject localPlayer = FindLocalPlayer();
            if (localPlayer == null) return;

            float distance = Vector3.Distance(localPlayer.transform.position, spawnPosition);
            if (distance < equipDistance)
            {
                if (IsServer) shouldDespawn.Value = true;
                else RequestDespawn();
            }
        }
    }

    private GameObject FindLocalPlayer()
    {
        foreach (GameObject player in playerPositions)
        {
            // Use the NetworkObject component to compare ClientId
            NetworkObject netObj = player.GetComponent<NetworkObject>();
            if (netObj != null && netObj.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                return player;
            }
        }
        return null;
    }

    private void RequestDespawn()
    {
        if (IsServer) shouldDespawn.Value = true;
    }

    private void DespawnEtoEquip()
    {
        if (IsServer) NetworkObject.Despawn(true);
    }
}