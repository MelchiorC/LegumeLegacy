using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventorySlot;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public void Awake()
    {
        // If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            // Set the static instance to this instance
            Instance = this;
        }
    }

    [Header("Tools")]
    // Tool slots
    [SerializeField]
    private List<ItemSlotData> toolSlots = new List<ItemSlotData>(); // Changed from array to List
    // Tool in the player's hand
    [SerializeField]
    private ItemSlotData equippedToolSlot = null;

    [Header("Items")]
    // Item slots
    [SerializeField]
    private List<ItemSlotData> itemSlots = new List<ItemSlotData>(); // Changed from array to List
    // Item in the player's hand
    [SerializeField]
    private ItemSlotData equippedItemSlot = null;

    // The transform for the player to hold items in the scene
    public Transform handPoint;

    // Equipping

    // Handles movement of item from inventory to hand
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        // The slot to equip (Tool by Default)
        ItemSlotData handToEquip = equippedToolSlot;
        // The list to change
        List<ItemSlotData> inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            // Change the slot to item
            handToEquip = equippedItemSlot;
            inventoryToAlter = itemSlots;
        }

        // Check if stackable
        if (handToEquip.Stackable(inventoryToAlter[slotIndex]))
        {
            ItemSlotData slotToAlter = inventoryToAlter[slotIndex];

            // Add to the hand slot
            handToEquip.AddQuantity(slotToAlter.quantity);

            // Empty the inventory slot
            slotToAlter.Empty();
        }
        else
        {
            // Not stackable
            // Cache the inventory ItemSlotData
            ItemSlotData slotToEquip = new ItemSlotData(inventoryToAlter[slotIndex]);

            // Change the inventory slot to the hand
            inventoryToAlter[slotIndex] = new ItemSlotData(handToEquip);

            EquipHandSlot(slotToEquip);
        }

        // Update the changes in the scene
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }

        // Update the changes to the UI
        UIManager.Instance.RenderInventory();
    }

    // Handles movement of item from hand to inventory
    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {
        // The slot to move (Tool by default)
        ItemSlotData handSlot = equippedToolSlot;
        // The list to change
        List<ItemSlotData> inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            handSlot = equippedItemSlot;
            inventoryToAlter = itemSlots;
        }

        // Try stacking the hand slot
        // Check if the operation failed
        if (!StackItemToInventory(handSlot, inventoryToAlter))
        {
            // Find an empty slot to put the item in
            for (int i = 0; i < inventoryToAlter.Count; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    // Send the equipped item over to its new slot
                    inventoryToAlter[i] = new ItemSlotData(handSlot);

                    // Remove the item from the hand
                    handSlot.Empty();
                    break;
                }
            }
        }

        // Update the changes in the scene
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }

        // Update the changes to the UI
        UIManager.Instance.RenderInventory();
    }

    
    public bool StackItemToInventory(ItemSlotData itemSlot, List<ItemSlotData> inventoryList)
    {
        int remainingAmount = itemSlot.quantity;

        // Try stacking into existing slots
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].Stackable(itemSlot))
            {
                int spaceAvailable = ItemSlotData.maxStackSize - inventoryList[i].quantity;
                int amountToAdd = Mathf.Min(spaceAvailable, remainingAmount);
                inventoryList[i].AddQuantity(amountToAdd);

                remainingAmount -= amountToAdd;

                if (remainingAmount <= 0)
                {
                    itemSlot.Empty();
                    return true; // Return true if everything is stacked
                }
            }
        }

        // If no existing slot can stack, add a single new slot
        if (remainingAmount > 0)
        {
            InventorySlot.InventoryType inventoryType = inventoryList == itemSlots ? InventorySlot.InventoryType.Item : InventorySlot.InventoryType.Tool;

            // Avoid adding multiple slots in one call by ensuring this happens only once
            if (inventoryList.Count == UIManager.Instance.GetUIInventorySlotCount(inventoryType))
            {
                inventoryList.Add(new ItemSlotData(null, 0));
                UIManager.Instance.AddInventorySlotUI(inventoryType);
            }

            int newSlotIndex = inventoryList.Count - 1;
            inventoryList[newSlotIndex] = new ItemSlotData(itemSlot.itemData, Mathf.Min(ItemSlotData.maxStackSize, remainingAmount));
            remainingAmount -= Mathf.Min(ItemSlotData.maxStackSize, remainingAmount);

            if (remainingAmount > 0)
            {
                Debug.LogWarning("Not enough space to stack all items in the inventory.");
            }

            itemSlot.quantity = remainingAmount;
            return true; // Return true after adding the new slot
        }

        return false; // Return false if no stacking or slot addition could occur
}

    // Handles movement of item from shop to inventory
    public void ShopToInventory(ItemSlotData itemSlotToMove)
    {
        // The inventory list to change
        List<ItemSlotData> inventoryToAlter = IsTool(itemSlotToMove.itemData) ? toolSlots : itemSlots;

        // Try stacking the hand slot
        // Check if the operation failed
        if (!StackItemToInventory(itemSlotToMove, inventoryToAlter))
        {
            // Find an empty slot to put the item in
            for (int i = 0; i < inventoryToAlter.Count; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    Debug.Log($"Added item {itemSlotToMove.itemData.name} to empty slot at index {i}.");
                    inventoryToAlter[i] = new ItemSlotData(itemSlotToMove);
                    UIManager.Instance.RenderInventory(); // Update the UI
                    return;
                }
            }
        }
        // Update the changes to the UI
        UIManager.Instance.RenderInventory();
        RenderHand();
    }

    // Render the player's equipped item in the scene
    public void RenderHand()
    {
        // Reset objects on the hand
        if (handPoint.childCount > 0)
        {
            Destroy(handPoint.GetChild(0).gameObject);
        }

        // Check if the player has anything equipped
        if (SlotEquipped(InventorySlot.InventoryType.Item))
        {
            // Instantiate the game model on the player's hand and put it on the scene
            Instantiate(GetEquippedSlotItem(InventorySlot.InventoryType.Item).gameModel, handPoint);
        }
    }

    // Inventory slot data
    #region Gets and Checks
    // Get the slot item (ItemData)
    public ItemData GetEquippedSlotItem(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot.itemData;
        }
        return equippedToolSlot.itemData;
    }
    public ItemData GetEquippedSlotItem()
    {
        return equippedToolSlot.itemData;
    }
    public List<ItemSlotData> GetAllInventoryItems()
    {
        List<ItemSlotData> ret = new List<ItemSlotData>();
        if (equippedToolSlot.itemData != null)
            ret.Add(equippedToolSlot);
        if (equippedItemSlot.itemData != null)
            ret.Add(equippedItemSlot);

        ret.AddRange(toolSlots);
        ret.AddRange(itemSlots);
        return ret;
    }
    // Get function for the inventory slots
    public List<ItemSlotData> GetInventorySlots(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return itemSlots;
        }
        return toolSlots;
    }

    // Check if a hand slot has an item
    public bool SlotEquipped(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return !equippedItemSlot.IsEmpty();
        }
        return !equippedToolSlot.IsEmpty();
    }

    // Check if the item is a tool
    public bool IsTool(ItemData item)
    {
        // Is it equipment?
        // Try to cast it as equipment first
        EquipmentData equipment = item as EquipmentData;
        if (equipment != null)
        {
            return true;
        }

        // Is it seed?
        // Try to cast it as a seed
        SeedData seed = item as SeedData;
        // If the seed is not null, it is a seed
        return seed != null;
    }
    #endregion

    public void EquipHandSlot(ItemSlotData itemSlot)
    {
        // Get the item data from the slot
        ItemData item = itemSlot.itemData;
        if (IsTool(item))
        {
            equippedToolSlot = new ItemSlotData(itemSlot);
        }
        else
        {
            equippedItemSlot = new ItemSlotData(itemSlot);
        }
    }

    // Overload for ItemData type
    public void EquipHandSlot(ItemData item)
    {
        EquipHandSlot(new ItemSlotData(item));
    }

    public void ConsumeItem(ItemSlotData itemSlot)
    {
        if (itemSlot.IsEmpty())
        {
            Debug.LogError("There is nothing to consume!");
            return;
        }

        // Use up one of the item slots
        itemSlot.Remove();
        // Refresh Inventory
        RenderHand();
        UIManager.Instance.RenderInventory();
    }

    public ItemSlotData GetEquippedSlot(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot;
        }
        return equippedToolSlot;
    }

}
