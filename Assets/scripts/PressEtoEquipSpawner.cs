using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PressEtoEquipSpawner : NetworkBehaviour
{
    // Variables

    // GameObjects
    public GameObject PressEtoEquip;
    // GameObject accessors

    // Other files

    public void SpawnEtoEquip(Vector3 spawnLocation)
    {
        // Debug.Log("Spawning Press E to Equip object");
        if (IsServer)
        {
            GameObject eToEquip = Instantiate(PressEtoEquip, spawnLocation, transform.rotation);

            NetworkObject networkObject = eToEquip.GetComponent<NetworkObject>();
            if (networkObject != null) networkObject.Spawn();
        }
    }
}