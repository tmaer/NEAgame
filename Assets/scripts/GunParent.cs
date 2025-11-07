using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Globalization;
using JetBrains.Annotations;

public class GunScript : MonoBehaviour, IItemParent
{
    // Variables

    // GameObjects
    public GameObject item { get; set; }
    public GameObject sprite { get; set; }
    public GameObject gun;
    public GameObject gunSprite;

    public GameObject player;
    // GameObject accessors
    // Network variables 
    public readonly NetworkVariable<bool> isEquipped = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Other files
    private GunSprite gunSpriteScript;

    public bool IsEquipped
    {
        get => isEquipped.Value; // Get the current value of the NetworkVariable
        set
        {
            isEquipped.Value = value;
        }
    }

    private void Awake()
    {
        gunSpriteScript = gunSprite.GetComponent<GunSprite>();
    }

    public void Equip(GameObject playerFR)
    {
        player = playerFR;
    }

    public void Drop()
    {

    }

    public void ShowItem()
    {
        gun.SetActive(true);
    }

    public void HideItem()
    {
        gun.SetActive(false);
    }

    public void SetEquippable()
    {
        gunSpriteScript.showingEquip = 0;
        gunSpriteScript.spawnPosition = gunSprite.transform.position + new Vector3(0, .7f, 0);
    }

    public GameObject GetItem()
    {
        return gun;
    }
}
