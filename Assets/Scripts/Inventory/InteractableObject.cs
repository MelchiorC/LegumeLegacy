using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    //The item information the GameObject is supposed to represent
    public ItemData item, waste;
    public int boost = 0;
    public string cropType;
    public virtual void Pickup()
    {
        
        for (int i = 0; i <= boost; i++) 
        {
            InventoryManager.Instance.EquipHandSlot(item);
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);

            InventoryManager.Instance.EquipHandSlot(waste);
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);
        }

        // Show pickup banner notifications
        int totalPickedUp = boost + 1;
        if (item != null && ItemPickupBanner.Instance != null)
            ItemPickupBanner.Instance.ShowPickup(item, totalPickedUp);
        if (waste != null && ItemPickupBanner.Instance != null)
            ItemPickupBanner.Instance.ShowPickup(waste, totalPickedUp);

        // Report Harvest Quest progress here
        QuestManager.Instance.ReportAction(QuestData.QuestType.Harvest, cropType);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPickupSFX();
        }

        //Set the player's inventory to the item
        //InventoryManager.Instance.EquipHandSlot(item);
        // InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);

        //InventoryManager.Instance.EquipHandSlot(waste);
        //InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);

        //Update the changes to the scene


        //Destroy this instance so as to not have multiple copies
        Destroy(gameObject);
    }

    public bool IsPlayerHoldingItem()
    {
        ItemData handSlotItem = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Item);
        return handSlotItem != null;
    }
}
