using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafMechanism : MonoBehaviour
{
    public ItemData Leaf;
    public GameTimestamp FirstDay;
    private bool canGatherLeaf = false; // Flag to check if the player is in the trigger zone

    void Update()
    {
        // Check if the player is in the trigger zone and presses the 'E' key
        if (canGatherLeaf && Input.GetKeyDown(KeyCode.E))
        {
            GatherLeaf();
        }
    }

    private void GatherLeaf()
    {
        // Ensure timestamp comparison and item collection conditions
        if (FirstDay == null || GameTimestamp.CompareTimestamps(FirstDay, TimeManager.Instance.GetGameTimestamp()) >= 1)
        {
            FirstDay = TimeManager.Instance.GetGameTimestamp();
            InventoryManager.Instance.EquipHandSlot(Leaf);
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger zone
        if (other.CompareTag("Player"))
        {
            canGatherLeaf = true;
            DisplayLeafCollectionPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player has exited the trigger zone
        if (other.CompareTag("Player"))
        {
            canGatherLeaf = false;
        }
    }

    private void DisplayLeafCollectionPrompt()
    {
        Debug.Log("Press 'E' to gather the leaf.");
    }
}