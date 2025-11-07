using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorSpawner : NetworkBehaviour
{
    // Variables
    private readonly List<Vector3> rotatedDoorSpawnCoordinates = new()
    {
    //rotation 90
    new(-.25f,28,0),
    new(-2.6f,43.1f,0),
    new(-26.7f,43.1f,0),
    new(-67.8f,28,0),
    new(-70.55f,-5,0),
    new(29.1f,14.8f,0),
    new(53.6f,-11.7f,0),
    new(53.6f,-27.3f,0),
    new(-11.5f,-34.35f,0),
    new(-15.4f,-15,0),
    new(8.25f,-27.25f,0),
    new(29.1f,-27.25f,0),
    };

    private readonly List<Vector3> doorSpawnCoordinates = new()
    {
    //rotation 0
    new(-30.15f,7.4f,0),
    new(-40.1f,35.3f,0),
    new(-16.9f,35.35f,0),
    new(-30.1f,-23.25f,0),
    new(17.1f,-.5f,0),
    new(65.15f,11.2f,0),
    new(41.65f,20.2f,0),
    };

    // GameObjects
    public GameObject door;
    // GameObject accessors

    // Other files

    public void SpawnDoors()
    {
        foreach (var position in rotatedDoorSpawnCoordinates)
        {
            SpawnDoor(position, Quaternion.Euler(0, 0, 90));
        }
        foreach (var position in doorSpawnCoordinates)
        {
            SpawnDoor(position, Quaternion.identity);
        }
    }

    private void SpawnDoor(Vector3 position, Quaternion rotation)
    {
        if (IsServer)
        {
            GameObject doorInstance = Instantiate(door, position, rotation);
            
            if (doorInstance.TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.Spawn();
            }
        }
    }
}
