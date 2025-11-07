using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemAssigner : NetworkBehaviour
{
    // GameObjects
    public GameObject[] parents;

    public GameObject knife;
    public GameObject gun; // assign no????

    [ContextMenu("Spawn and assign knives")]
    public void SpawnKnives()
    {
        parents = GameObject.FindGameObjectsWithTag("Player Parent");

        foreach (GameObject parent in parents)
        {
            if (parent.TryGetComponent<NetworkObject>(out _) && parent.TryGetComponent<IPlayerParent>(out var playerParentScript))
            {
                GameObject player = playerParentScript.GetPlayer();
                Player playerScript = player.GetComponent<Player>();

                GameObject knifeParent = Instantiate(knife, parent.transform.position, Quaternion.identity);

                if (knifeParent.TryGetComponent(out NetworkObject networkObject))
                {
                    ulong ownerClientId = parent.GetComponent<NetworkObject>().OwnerClientId;
                    networkObject.SpawnWithOwnership(ownerClientId);
                }

                playerScript.AddKnifeToInventory(knifeParent);
            }
        }
    }
}
