using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemDataHolder itemHolder = other.GetComponent<ItemDataHolder>();
        if (itemHolder != null && itemHolder.itemData != null)
        {
            ItemSlotData itemSlot = new ItemSlotData(itemHolder.itemData);

            if (inventoryManager.IsTool(itemHolder.itemData))
            {
                inventoryManager.ShopToInventory(itemSlot); 
            }
            else
            {
                inventoryManager.ShopToInventory(itemSlot); 
            }

            Debug.Log($"Collected item: {itemHolder.itemData.description}");
        }
    }
}

