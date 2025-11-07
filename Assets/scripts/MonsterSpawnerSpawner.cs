using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterSpawnerSpawner : NetworkBehaviour
{
    // Variables
    private readonly List<Vector3> monsterSpawnerSpawnCoordinates = new()
    {
    new(-28, 25.6f, 0),
    new(-34, 57.1f, 0),
    new(8.5f, 57.1f, 0),
    new(63, -47.1f, 0),
    new(-20.5f, -47.1f, 0),
    new(19.2f, 25.6f, 0),
    new(-77.3f, -29.3f,0),
    new(-82.2f, 26.1f,0),
    new(62.2f,23.9f,0),
    };

    // GameObjects
    public GameObject monsterSpawner;
    // GameObject accessors

    // Other files

    public void SpawnSpawners()
    {
        /*
        for (int i = 0; i < monsterSpawnerSpawnCoordinates.Count; i++)
        {
            Instantiate(monsterSpawner, monsterSpawnerSpawnCoordinates[i], transform.rotation);
        }   */
        // Instantiate(monsterSpawner, monsterSpawnerSpawnCoordinates[0], transform.rotation);
    }
}
