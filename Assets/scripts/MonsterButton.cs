using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterButton : NetworkBehaviour
{
    public GameObject[] spawners;

    public void AssignSpawners()
    {
        spawners = GameObject.FindGameObjectsWithTag("MonsterSpawner");
        foreach (GameObject spawner in spawners)
        {
            var spawnerScript = spawner.GetComponent<MonsterSpawner>();
            spawnerScript.Spawn();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (GameObject spawner in spawners)
            {
                var spawnerScript = spawner.GetComponent<MonsterSpawner>();
                spawnerScript.Spawn();
            }
        }
    }
}
