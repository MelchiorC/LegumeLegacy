using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventorySlot;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {  get; private set; }

    public void Awake()
    {
        //If there is more than one instance, destroy the extra
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }

        //Initialize inventory with starting slots
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        //Initialize tool slots if empty
        if (toolSlots.Count == 0)
        {
            for (int i = 0; i < initialInventorySize; i++)
            {
                toolSlots.Add(new ItemSlotData());
            }
        }

        //Initialize item slots if empty
        if (itemSlots.Count == 0)
        {
            for (int i = 0; i < initialInventorySize; i++)
            {
                itemSlots.Add(new ItemSlotData());
            }
        }

        //Initialize equipped slots if null
        if (equippedToolSlot == null)
            equippedToolSlot = new ItemSlotData();
        if (equippedItemSlot == null)
            equippedItemSlot = new ItemSlotData();
    }

    [Header("Tools")]
    //Tool slots (dynamic)
    [SerializeField]
    private List<ItemSlotData> toolSlots = new List<ItemSlotData>();
    //Tool in the player's hand
    [SerializeField]
    private ItemSlotData equippedToolSlot = null;

    [Header("Items")]
    //item slots (dynamic)
    [SerializeField]
    private List<ItemSlotData> itemSlots = new List<ItemSlotData>();
    //Item in the player's hand
    [SerializeField]
    private ItemSlotData equippedItemSlot = null;

    [Header("Inventory Settings")]
    //Starting inventory size
    [SerializeField]
    private int initialInventorySize = 8;
    //How many slots to add when inventory is full
    [SerializeField]
    private int slotsToAddWhenFull = 4;
    //Maximum inventory size (0 = unlimited)
    [SerializeField]
    private int maxInventorySize = 0;

    //The transform for the player to hold items in the scene
    public Transform handPoint;

    //Equipping

    //Handles movement of item from inventory to hand
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        //The slot to equip (Tool by Default)
        ItemSlotData handToEquip = equippedToolSlot;
        //The array to change
        List<ItemSlotData> inventoryToAlter = toolSlots;

        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            handToEquip = equippedItemSlot;
            inventoryToAlter = itemSlots;  
        }

        //Check if stackable
        if (handToEquip.Stackable(inventoryToAlter[slotIndex]))
        {
            ItemSlotData slotToAlter = inventoryToAlter[slotIndex];

            //Add to the hand slot
            handToEquip.AddQuantity(slotToAlter.quantity);

            //Empty the inventory slot
            slotToAlter.Empty();
        }
        else
        {
            //Not stackable
            //Cache the inventory ItemSlotData
            ItemSlotData slotToEquip = new ItemSlotData(inventoryToAlter[slotIndex]);

            //Change the inventory slot to the hand
            inventoryToAlter[slotIndex] = new ItemSlotData(handToEquip);

            EquipHandSlot(slotToEquip);
        }

        //Update the changes in the scene
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }
        
        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
    }

    //Handles movement of item from hand to inventory
    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {
        //The slot to move (Tool by default)
        ItemSlotData handSlot = equippedToolSlot;
        //The array to change
        List<ItemSlotData> inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            handSlot = equippedItemSlot;
            inventoryToAlter = itemSlots;
        }

        //Try stacking the handslot
        //Check if the operation failed
        if(!StackItemToInventory(handSlot, inventoryToAlter))
        {
            //Find an empty slot to put the item in
            bool itemPlaced = false;
            
            //Iterate through each inventory slot and find an empty slot
            for (int i = 0; i < inventoryToAlter.Count; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    //Send the eqquiped item over to its new slot
                    inventoryToAlter[i] = new ItemSlotData(handSlot);

                    //Remove the item from the hand
                    handSlot.Empty();
                    itemPlaced = true;
                    break;
                }
            }

            //If no empty slot found, expand inventory
            if (!itemPlaced)
            {
                ExpandInventory(inventoryType);
                //Place item in the first new slot
                inventoryToAlter[inventoryToAlter.Count - slotsToAddWhenFull] = new ItemSlotData(handSlot);
                //Remove the item from the hand
                handSlot.Empty();
            }
        }

        //Update the changes in the scene
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
    }

    //Iterate through each of the items in the inventory to see if it can be stacked
    //Will perform the operation if found, return false id unsuccesful
    public bool StackItemToInventory(ItemSlotData itemSlot, List<ItemSlotData> inventoryList)
    {
        for(int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].Stackable(itemSlot))
            {
                //Add to the inventory's slot stack
                inventoryList[i].AddQuantity(itemSlot.quantity);
                //Empty the item slot
                itemSlot.Empty();
                return true;
            }
        }

        //Can't find any slot that can be stacked
        return false;
    }
    
    //Handles movement of item from shop to inventory
    public void ShopToInventory(ItemSlotData itemSlotToMove)
    {
        //The inventory array to change
        List<ItemSlotData> inventoryToAlter = IsTool(itemSlotToMove.itemData) ? toolSlots : itemSlots;
        InventorySlot.InventoryType targetType = IsTool(itemSlotToMove.itemData) ? InventorySlot.InventoryType.Tool : InventorySlot.InventoryType.Item;

        Debug.Log($"[ShopToInventory] Adding {itemSlotToMove.itemData.name} to {targetType} inventory");

        //Try stacking the handslot
        //Check if the operation failed
        if (!StackItemToInventory(itemSlotToMove, inventoryToAlter))
        {
            //Find an empty slot to put the item in
            bool itemPlaced = false;
            
            //Iterate through each inventory slot and find an empty slot
            for (int i = 0; i < inventoryToAlter.Count; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    //Send the eqquiped item over to its new slot
                    inventoryToAlter[i] = new ItemSlotData(itemSlotToMove);
                    itemPlaced = true;
                    break;
                }
            }

            //If no empty slot found, expand inventory
            if (!itemPlaced)
            {
                ExpandInventory(targetType);
                //Place item in the first new slot
                inventoryToAlter[inventoryToAlter.Count - slotsToAddWhenFull] = new ItemSlotData(itemSlotToMove);
            }
        }
        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        RenderHand();
    }

    //Expand inventory by adding new slots
    public void ExpandInventory(InventorySlot.InventoryType inventoryType)
    {
        List<ItemSlotData> inventoryToExpand = inventoryType == InventorySlot.InventoryType.Tool ? toolSlots : itemSlots;

        //Check if we've reached max capacity
        if (maxInventorySize > 0 && inventoryToExpand.Count >= maxInventorySize)
        {
            Debug.LogWarning($"Inventory is at maximum capacity ({maxInventorySize})!");
            return;
        }

        int slotsToAdd = slotsToAddWhenFull;
        
        //Make sure we don't exceed max size
        if (maxInventorySize > 0 && inventoryToExpand.Count + slotsToAdd > maxInventorySize)
        {
            slotsToAdd = maxInventorySize - inventoryToExpand.Count;
        }

        //Add new empty slots
        for (int i = 0; i < slotsToAdd; i++)
        {
            inventoryToExpand.Add(new ItemSlotData());
        }

        Debug.Log($"{inventoryType} inventory expanded! New size: {inventoryToExpand.Count}");
        
        //Notify UI to refresh
        UIManager.Instance.RenderInventory();
    }
    //Render the player's equipped item in the scene
    public void RenderHand()
    {
        //Reset objectss on the hand
        if(handPoint.childCount > 0)
        {
            Destroy(handPoint.GetChild(0).gameObject);
        }

        //Check if the player has anything equipped
        if(SlotEquipped(InventorySlot.InventoryType.Item))
        {
            //Instantiate the game model on the player's hand and put it on the scene
            Instantiate(GetEquippedSlotItem(InventorySlot.InventoryType.Item).gameModel, handPoint);
        }
    }

    //Inventory slot data
    #region Gets and Checks
    //Get the slot item (ItemData)
    public ItemData GetEquippedSlotItem(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
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
    { List<ItemSlotData> ret;
        ret = new List<ItemSlotData>();
        if(equippedToolSlot.itemData != null)
            ret.Add(equippedToolSlot);
        if (equippedItemSlot.itemData != null)
            ret.Add(equippedItemSlot);

        for(int i = 0; i < toolSlots.Count; i++)
        {
            if (toolSlots[i].itemData != null)
            {
                ret.Add(toolSlots[i]);
            }
        }
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].itemData != null)
            {
                ret.Add(itemSlots[i]);
            }
        }
        return ret;
    }
    //Get function for the slots (ItemSlotData)
    public ItemSlotData GetEquippedSlot(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot;
        }
        return equippedToolSlot;
    }
    public ItemSlotData GetEquippedSlot()
    {
        return equippedItemSlot;
    }

    //Get function for the inventory slots
    public ItemSlotData[] GetInventorySlots(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            return itemSlots.ToArray();
        }
        return toolSlots.ToArray();
    }
    public ItemSlotData[] GetInventorySlots()
    {
        return itemSlots.ToArray();
    }

    //Check if a hand slot has a item
    public bool SlotEquipped(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return !equippedItemSlot.IsEmpty();
        }
        return !equippedToolSlot.IsEmpty();
    }

    //Check if the item is a tool
    public bool IsTool(ItemData item)
    {
        //Is it equipment?
        //Try to cast it as equipment first
        EquipmentData equipment = item as EquipmentData;
        if(equipment != null)
        {
            return true;
        }

        //Is it seed?
        //Try to cast it as a seed
        SeedData seed = item as SeedData;
        //If the seed not null it is a seed
        return seed != null;
    }
    #endregion

    //Equip the hand slot with an ItemData (Will overwrite the slot)
    public void EquipHandSlot(ItemData item)
    {
        if(IsTool(item))
        {
            equippedToolSlot = new ItemSlotData(item);
        }
        else
        {
            equippedItemSlot = new ItemSlotData(item);
        }
    }

    //Equip the hand slot with an ItemSlotData (Will overwrite the slot)
    public void EquipHandSlot(ItemSlotData itemSlot)
    {
        //Get the item data from the slot
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

    public void ConsumeItem(ItemSlotData itemSlot)
    {
        if (itemSlot.IsEmpty())
        {
            Debug.LogError("There is nothing to cunsume!");
            return;
        }

        //Use up one of the item slots
        itemSlot.Remove();
        //Refresh Inventory
        RenderHand();
        UIManager.Instance.RenderInventory();
    }

    #region Inventory Slot Validation
    private void OnValidate()
    {
        //Validate the hand slot
        ValidateInventorySlot(equippedToolSlot);
        ValidateInventorySlot(equippedItemSlot);

        //Validate the slots in the inventory
        ValidateInventorySlots(itemSlots);
        ValidateInventorySlots(toolSlots);
    }

    //When giving the itemData value in the inspector, automatically set the quantity to 1
    void ValidateInventorySlot(ItemSlotData slot)
    {
        if (slot != null && slot.itemData != null && slot.quantity == 0)
        {
            slot.quantity = 1;
        }
    }

    //Validate lists
    void ValidateInventorySlots(List<ItemSlotData> list)
    {
        foreach(ItemSlotData slot in list)
        {
            ValidateInventorySlot(slot);
        }
    }
    #endregion
}
