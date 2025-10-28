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
    }

    [Header("Tools")]
    //Tool slots
    [SerializeField]
    private ItemSlotData[] toolSlots = new ItemSlotData[8];
    //Tool in the player's hand
    [SerializeField]
    private ItemSlotData equippedToolSlot = null;

    [Header("Items")]
    //item slots
    [SerializeField]
    private ItemSlotData[] itemSlots = new ItemSlotData[8];
    //Item in the player's hand
    [SerializeField]
    private ItemSlotData equippedItemSlot = null;

    //The transform for the player to hold items in the scene
    public Transform handPoint;

    //Equipping

    //Handles movement of item from inventory to hand
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        //The slot to equip (Tool by Default)
        ItemSlotData handToEquip = equippedToolSlot;
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

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
        ItemSlotData[] inventoryToAlter = toolSlots;

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
            //Iterate through each inventory slot and find an empty slot
            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    //Send the eqquiped item over to its new slot
                    inventoryToAlter[i] = new ItemSlotData(handSlot);

                    //Remove the item from the hand
                    handSlot.Empty();
                    break;
                }
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
    public bool StackItemToInventory(ItemSlotData itemSlot, ItemSlotData[] inventoryArray)
    {
        for(int i = 0; i < inventoryArray.Length; i++)
        {
            if (inventoryArray[i].Stackable(itemSlot))
            {
                //Add to the inventory's slot stack
                inventoryArray[i].AddQuantity(itemSlot.quantity);
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
        ItemSlotData[] inventoryToAlter = IsTool(itemSlotToMove.itemData) ? toolSlots : itemSlots;

        //Try stacking the handslot
        //Check if the operation failed
        if (!StackItemToInventory(itemSlotToMove, inventoryToAlter))
        {
            //Find an empty slot to put the item in
            //Iterate through each inventory slot and find an empty slot
            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    //Send the eqquiped item over to its new slot
                    inventoryToAlter[i] = new ItemSlotData(itemSlotToMove);
                    break;
                }
            }
        }
        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        RenderHand();
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

        for(int i = 0; i < toolSlots.Length; i++)
        {
            if (toolSlots[i].itemData != null)
            {
                ret.Add(toolSlots[i]);
            }
        }
        for (int i = 0; i < itemSlots.Length; i++)
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
            return itemSlots;
        }
        return toolSlots;
    }
    public ItemSlotData[] GetInventorySlots()
    {
        return itemSlots;
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
        if (slot.itemData != null && slot.quantity == 0)
        {
            slot.quantity = 1;
        }
    }

    //Validate arrays
    void ValidateInventorySlots(ItemSlotData[] array)
    {
        foreach(ItemSlotData slot in array)
        {
            ValidateInventorySlot(slot);
        }
    }
    #endregion
    // Start is called before the first frame update

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
