using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

//using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    // Variables
    // GameObjects
    public GameObject map;
    public GameObject mainCamera;
    public GameObject monsterButtonton;
    private GameObject localPlayerParent;
    // GameObject accessors

    // Other files
    private ItemSpawner itemSpawner;
    private MonsterSpawnerSpawner monsterSpawnerSpawner;
    private DoorSpawner doorSpawner;
    private MonsterButton monsterButton;

    private void Awake()
    {
        itemSpawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<ItemSpawner>();
        monsterSpawnerSpawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<MonsterSpawnerSpawner>();
        doorSpawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<DoorSpawner>();
        monsterButton = monsterButtonton.GetComponent<MonsterButton>();
    }

    public void StartGame()
    {
        // Debug.Log("Start");
        SpawnMap(); SpawnItems();
        JoinGame();
    }

    public void JoinGame()
    {
        GameObject[] playerParents = GameObject.FindGameObjectsWithTag("Player Parent");
        foreach (var parent in playerParents)
        {
            if (parent.TryGetComponent(out NetworkObject networkObject) &&
                networkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log($"Local player found: {parent.name}, OwnerClientId: {networkObject.OwnerClientId}");

                localPlayerParent = parent;
                localPlayerParent.SetActive(true);
                localPlayerParent.transform.position = Vector3.zero;

                mainCamera.transform.SetParent(localPlayerParent.transform);
                mainCamera.transform.localPosition = new Vector3(0f, 0f, -10f);

                return;
            }
        }

        Debug.LogWarning("Local player parent not found!");
    }
    
    public void SpawnMap()
    {
        map.SetActive(true);
    }

    public void SpawnItems()
    {
        doorSpawner.SpawnDoors();
        // itemSpawner.SpawnItems();
        // monsterSpawnerSpawner.SpawnSpawners();
        // monsterButton.AssignSpawners();
    }
}
