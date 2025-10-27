using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poopmekanism : MonoBehaviour
{
    public ItemData Poop;
    public GameTimestamp Hari1;
    private bool canCollectPoop = false; // Flag to check if the player is in the trigger zone

    void Update()
    {
        // Check if player is in the trigger zone and presses the 'E' key
        if (canCollectPoop && Input.GetKeyDown(KeyCode.E))
        {
            CollectPoop();
        }
    }

    private void CollectPoop()
    {
        if (Hari1 == null || GameTimestamp.CompareTimestamps(Hari1, TimeManager.Instance.GetGameTimestamp()) >= 1)
        {
            Hari1 = TimeManager.Instance.GetGameTimestamp();
            InventoryManager.Instance.EquipHandSlot(Poop);
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger zone
        if (other.CompareTag("Player"))
        {
            canCollectPoop = true;
            ShowPoopCollectionPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player has exited the trigger zone
        if (other.CompareTag("Player"))
        {
            canCollectPoop = false;
        }
    }

    private void ShowPoopCollectionPrompt()
    {
        Debug.Log("Press 'E' to collect poop.");
    }
}