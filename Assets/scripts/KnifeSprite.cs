using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class KnifeSprite : NetworkBehaviour
{
    // Variables
    public Vector3 spawnPosition;

    public float equipDistance = 3f;
    public float distance;

    public int showingEquip = 0;
    public bool isPressEtoEquipVisible = false;

    private readonly HashSet<IPlayerParent> processedPlayers = new();
    // GameObjects
    private GameObject[] playerPositions;
    // GameObject accessors
    // Network variables
    private readonly NetworkVariable<bool> networkIsEquipped = new(false);
    // Other files
    private Player playerScript;
    private PressEtoEquipSpawner pressEtoEquip;
    public KnifeScript parentScript;

    private void Awake()
    {
        playerPositions = GameObject.FindGameObjectsWithTag("Player Parent");
        pressEtoEquip = GameObject.FindGameObjectWithTag("Spawner").GetComponent<PressEtoEquipSpawner>();
    }

    public void Update()
    {
        if (TryGetComponent<IItemParent>(out IItemParent parentScript))
        {
            if (parentScript.IsEquipped) return;
        }
        if (!IsServer) return;

        foreach (GameObject position in playerPositions)
        {
            distance = Vector3.Distance(position.transform.position, transform.position);

            if (position.TryGetComponent<IPlayerParent>(out var playerParent))
            {
                playerScript = playerParent.GetPlayer().GetComponent<Player>();
                if (distance <= equipDistance && !processedPlayers.Contains(playerParent))
                {
                    playerScript.IncreaseEquipableItemCount();
                    processedPlayers.Add(playerParent); // Mark player as processed
                    showingEquip++;
                }
                else if (distance > equipDistance && processedPlayers.Contains(playerParent))
                {
                    playerScript.DecreaseEquipableItemCount();
                    processedPlayers.Remove(playerParent); // Remove player from processed list
                    showingEquip--;
                }
            }
        }

        // Handle visibility
        if (!isPressEtoEquipVisible && showingEquip > 0)
        {
            spawnPosition = transform.position + new Vector3(0, .7f, 0);
            pressEtoEquip.SpawnEtoEquip(spawnPosition);
            isPressEtoEquipVisible = true;
        }
        else if (isPressEtoEquipVisible && showingEquip <= 0)
        {
            isPressEtoEquipVisible = false;
        }
    }
}