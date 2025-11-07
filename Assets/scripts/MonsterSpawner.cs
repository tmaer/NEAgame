using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterSpawner : NetworkBehaviour
{
    public GameObject monster;

    public void Spawn()
    {
        // Instantiate(monster, transform.position, transform.rotation);
    }

    /*
     * create a queue 
     * when spawning a wave create all monsters
     * add all monsters to queue
     * systematically spawn monster at the front of queue
     * move monsters along in the queue
     * useful when more monsters added to game
     */
}
