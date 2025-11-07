    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Globalization;

public class KnifeScript : NetworkBehaviour, IItemParent
{
    // Variables

    // GameObjects
    public GameObject item;
    public GameObject sprite;

    // Network variables
    private readonly NetworkVariable<ulong> playerNetworkObjectId = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public readonly NetworkVariable<bool> isEquipped = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    private readonly NetworkVariable<bool> isItemVisible = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    // GameObject accessors
    public Collider2D playerCollider;
    // Other files
    public Knife itemScript;
    public KnifeSprite spriteScript;

    public bool IsEquipped
    {
        get => isEquipped.Value;
        set => isEquipped.Value = value;
    }

    private void Awake()
    {
        Debug.Log("Subscribing to OnPlayerNetworkObjectIdChanged");
        playerNetworkObjectId.OnValueChanged += OnPlayerNetworkObjectIdChanged;
        isItemVisible.OnValueChanged += OnIsItemVisibleChanged;
    }   

    private void OnPlayerNetworkObjectIdChanged(ulong previousValue, ulong newValue)
    {
        Debug.Log("make it");
        if (newValue == 0)
        {
            playerCollider = null;
        }
        else
        {
            playerCollider = GetGameObjectByNetworkId(newValue).GetComponentInChildren<Collider2D>();
        }
    }

    private void OnIsItemVisibleChanged(bool previousValue, bool newValue)
    {
        if (item != null)
        {
            if (newValue)
            {
                ShowItem();
            }
            if (!newValue)
            {
                HideItem();
            }
            item.SetActive(newValue);
        }
    }

    public void Equip(GameObject tempPlayer)
    {
        if (!IsOwner) return;

        playerNetworkObjectId.Value = tempPlayer.GetComponentInParent<NetworkObject>().NetworkObjectId;
        isEquipped.Value = true;
    }

    public void Drop()
    {
        if (!IsOwner) return;

        playerNetworkObjectId.Value = 0;
        isEquipped.Value = false;
        isItemVisible.Value = false;
        spriteScript.isPressEtoEquipVisible = false;
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

    public void ShowItem()
    {
        if (IsOwner) isItemVisible.Value = true;
    }

    public void HideItem()
    {
        if (itemScript.stabCheck != null)
        {
            StopCoroutine(itemScript.Stab());
            itemScript.stabCheck = null;
            itemScript.isStabbing = false;
            transform.rotation = Quaternion.identity;
        }

        transform.rotation = Quaternion.identity;
        if (IsOwner)
        {
            isItemVisible.Value = false;
        }
    }

    public void SetEquippable()
    {
        spriteScript.showingEquip = 0;
        spriteScript.spawnPosition = sprite.transform.position + new Vector3(0, .7f, 0);
    }

    public GameObject GetItem()
    {
        return item;
    }
}