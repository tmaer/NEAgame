using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class CameraLogic : NetworkBehaviour
{
    void Start()
    {
        // Ensures this code only runs for the local player
        if (IsOwner)
        {
            // Remove the InventoryLayer from the culling mask of the main camera
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Inventory"));
        }
    }
}