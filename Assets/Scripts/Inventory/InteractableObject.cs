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

        // Report Harvest Quest progress here
        QuestManager.Instance.ReportAction(QuestData.QuestType.Harvest, cropType);

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