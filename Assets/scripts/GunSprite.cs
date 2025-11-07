using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunSprite : NetworkBehaviour
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

    // Other files
    private Player playerScript;
    private PressEtoEquipSpawner pressEtoEquip;

    private void Awake()
    {
        playerPositions = GameObject.FindGameObjectsWithTag("Player Parent");
        pressEtoEquip = GameObject.FindGameObjectWithTag("Spawner").GetComponent<PressEtoEquipSpawner>();
        spawnPosition = transform.position + new Vector3(0, .7f, 0);
    }

    public void Update()
    {
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
            pressEtoEquip.SpawnEtoEquip(spawnPosition);
            isPressEtoEquipVisible = true;
        }
        else if (isPressEtoEquipVisible && showingEquip <= 0)
        {
            isPressEtoEquipVisible = false;
        }
    }
}
