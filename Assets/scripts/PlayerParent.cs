using Unity.Netcode;
using UnityEngine;

public class PlayerParent : NetworkBehaviour, IPlayerParent
{
    public GameObject inventory;

    public GameObject player;

    private void Start()
    {   
        if (IsOwner)
        {
            ShowObject(inventory);
        }
        else
        {
            HideObject(inventory);
        }
    }
    public GameObject GetPlayer()
    {
        return player;
    }

    public void ShowObject(GameObject gameObject)
    {
        // Make it visible to the local player
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
    }

    public void HideObject(GameObject gameObject)
    {
        // Hide from others
        SetLayerRecursively(inventory, LayerMask.NameToLayer("Inventory"));
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
