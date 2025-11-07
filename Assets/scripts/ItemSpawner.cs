using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSpawner : NetworkBehaviour
{
    // Variables

    // Have a bunch of new Vector3 showing spawnable spaces


    // GameObjects
    public GameObject gun;
    public GameObject knife;
    // public GameObject itemname;
    // GameObject accessors

    // Other files   
    private ItemAssigner itemAssigner;

    void Awake()
    {
        itemAssigner = GameObject.FindGameObjectWithTag("NEALogic").GetComponent<ItemAssigner>();
    }

    public void SpawnItems()
    {
        // when lobby is made, spawn a knife on each player, make host or whatever do it
        // make sure it networking and shit
        // Change to code which randomly spawns items or however they will spawn on map
    }

    void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.P)) // TEMPORARY
        {
            itemAssigner.SpawnKnives();
        }
    }
}
