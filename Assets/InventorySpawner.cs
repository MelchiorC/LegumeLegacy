using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform parentContainer;

    void Start()
    {
        SpawnItems(); 
    }

    public void SpawnItems()
    {
        if (prefabToSpawn == null || parentContainer == null)
        {
            Debug.LogWarning("Prefab or Parent Container is not assigned!");
            return;
        }

        // Loop 10 times
        for (int i = 0; i < 10; i++)
        {
            
            GameObject newItem = Instantiate(prefabToSpawn, parentContainer);

            Debug.Log($"Spawned item {i + 1}");
        }
    }
}