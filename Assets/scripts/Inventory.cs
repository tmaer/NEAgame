using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    // Variables
    private Dictionary<string, GameObject> itemPrefabs;
    private Vector3 highlightedSlotOffset;

    public int invSlot = 0;

    public string itemTagVisibility;
    public string itemTagInventory;
    // GameObjects
    public GameObject highlightedSlot;
    public GameObject player;
    private GameObject storedItem;

    // public GameObject itemname;
    public GameObject gunPrefab;
    public GameObject knifePrefab;
    // GameObject accessors
    // Network variables 
    public NetworkVariable<ulong>[] inventory = new NetworkVariable<ulong>[3]
    {
        new(0),
        new(0),
        new(0)
    };
    public NetworkVariable<ulong> itemParentNetworkObjectId = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    // Other files

    private void Awake()
    {
        // Initialise item prefabs
        itemPrefabs = new Dictionary<string, GameObject>
        {
            { "Gun", gunPrefab },
            { "Knife", knifePrefab }
            // { Exact prefab tag, variable name }
        };

        highlightedSlotOffset = highlightedSlot.transform.position - transform.position;

        itemParentNetworkObjectId.OnValueChanged += OnParentSet;
    }

    private void OnParentSet(ulong previousValue, ulong newValue)
    {
        if (newValue == 0) return; // temporary, remove item parent
        GameObject parent = GetGameObjectByNetworkId(newValue);
        storedItem.transform.SetParent(parent.transform); // HOW TO ASSIGN THIS ON EVERY1
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleHighlightedSlot();
        ItemVisibilityHandler();
    }

    private void HandleHighlightedSlot()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        switch (scrollInput)
        {
            case > 0f:
                // Debug.Log("Scroll Wheel Up");
                invSlot = invSlot == 0 ? 2 : invSlot - 1;
                break;

            case < 0f:
                // Debug.Log("Scroll Wheel Down");
                invSlot = invSlot == 2 ? 0 : invSlot + 1;
                break;

            default:
                // No scroll input
                break;
        }
        Vector3 newSlotPosition;
        if (invSlot == 0) newSlotPosition = new Vector3(-1.9f, -.1f, transform.position.z);
        else if (invSlot == 1) newSlotPosition = new Vector3(0f, -.1f, transform.position.z);
        else newSlotPosition = new Vector3(1.9f, -.1f, transform.position.z);

        // Set position of highlighted slot based on invSlot
        highlightedSlot.transform.position = transform.position + highlightedSlotOffset + newSlotPosition;
    }

    private void ItemVisibilityHandler()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].Value != 0)
            {
                if (i == invSlot)
                {
                    //Debug.Log("Show");
                    IItemParent item = GetGameObjectByNetworkId(inventory[i].Value).GetComponent<IItemParent>();
                    item.ShowItem();
                }
                else
                {
                    //Debug.Log("Hide");
                    IItemParent item = GetGameObjectByNetworkId(inventory[i].Value).GetComponent<IItemParent>();
                    item.HideItem();
                }
            }
        }
    }

    private GameObject GetGameObjectByNetworkId(ulong networkObjectId)
    {
        foreach (var networkObject in FindObjectsOfType<NetworkObject>())
        {
            if (networkObject.NetworkObjectId == networkObjectId)
            {
                return networkObject.gameObject;
            }
        }
        return null; // Not found
    }

    public void AddToInventory(ulong itemNetworkObjectId, GameObject parent)
    {
        if (!IsOwner) return;

        GameObject item = GetGameObjectByNetworkId(itemNetworkObjectId);
        NetworkObject itemNetworkObject = item.GetComponent<NetworkObject>();
        itemNetworkObject.ChangeOwnership(NetworkManager.Singleton.LocalClientId);
        item.transform.SetParent(parent.transform);

        storedItem = item;
        itemParentNetworkObjectId.Value = parent.GetComponent<NetworkObject>().NetworkObjectId;
        // Debug.Log(item.tag);
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].Value == 0)
            {
                inventory[i].Value = itemNetworkObjectId;
                itemTagInventory = item.tag;
                if (itemPrefabs.TryGetValue(itemTagInventory, out _))
                {
                    // Equip item 
                    if (item.TryGetComponent<IItemParent>(out IItemParent itemParentScript))
                    {
                        itemParentScript.Equip(player);
                    }
                    Vector3 localTargetPosition = i switch
                    {
                        0 => new Vector3(4.2f, 3.7f, item.transform.position.z),
                        1 => new Vector3(6.05f, 3.7f, item.transform.position.z),
                        2 => new Vector3(7.8f, 3.7f, item.transform.position.z),
                        _ => Vector3.zero
                    };
                    item.transform.localPosition = localTargetPosition;
                    Debug.Log($"{item.tag} added to slot {i + 1}");
                    ItemVisibilityHandler();
                    return;
                }
            }
        }

        Debug.Log("Inventory is full");
    }

    public GameObject DropInventory()
    {
        return null;
        /*
        if (!IsOwner) return null;

        // int slot = currentItemSlot;
        if (!string.IsNullOrEmpty(inventory[slot].Value.ToString()))
        {
            string itemTag = inventory[slot].Value.ToString();
            inventory[slot].Value = 0;

            if (itemPrefabs.TryGetValue(itemTag, out GameObject prefab))
            {
                // Instantiate dropped item at player's position
                GameObject droppedItem = Instantiate(prefab, player.transform.position, Quaternion.identity);
                droppedItem.GetComponent<NetworkObject>().Spawn();
                Debug.Log($"{itemTag} dropped from slot {slot + 1}");
                return droppedItem;
            }
        }

        Debug.Log("No item to drop");
        return null;*/
    }

    public string InventoryCheck()
    {
        if (inventory[invSlot].Value != 0)
        {
            // Return the tag of the GameObject in the current slot
            return GetGameObjectByNetworkId(inventory[invSlot].Value).tag;
        }
        else
        {
            return null; // No item in the slot
        }
    }
}