using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    ItemData itemToDisplay;
    int quantity;

    public Image itemDisplayImage;
    public Text quantityText;

    public enum InventoryType
    {
        Item, Tool
    }
    //Determines which inventory section this slot apart of
    public InventoryType inventoryType;

    int slotIndex;

    public void Display(ItemSlotData itemSlot)
    {
        //Set the variables accordingly
        itemToDisplay = itemSlot.itemData;
        quantity = itemSlot.quantity;

        //By deafult, the quantity text should not show
        quantityText.text = "";

        //Check if there is an item to display
        if(itemToDisplay != null)
        {
            //Switch the thumbnail over
            itemDisplayImage.sprite = itemToDisplay.thumbnail;

            //Display the stack quantity if there is more than 1 in the stack
            if (quantity > 1)
            {
                quantityText.text = quantity.ToString();
            }

            itemDisplayImage.gameObject.SetActive(true);

            return;
        }

        itemDisplayImage.gameObject.SetActive(false);


    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //Move item from inventory to hand
        InventoryManager.Instance.InventoryToHand(slotIndex, inventoryType);
    }

    //Set the slot index
    public void AssignIndex(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }

    //Display the item info on the item info box when the mouse hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.DisplayItemInfo(itemToDisplay);
    }

    //Reset the item info on the item info box when the mouse leave
    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.DisplayItemInfo(null);
    }
    //Addition
    public ItemData GetItemSlotData()
    {
        return itemToDisplay;
    }
}
