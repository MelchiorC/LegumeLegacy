using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopOrShippingBin : InteractableObject
{
    public List<ItemData> shopItems;

    public override void Pickup()
    {
        // Check if the player is holding an item
        if (IsPlayerHoldingItem())
        {
            // Player is holding an item, trigger ShippingBin interaction
            HandleShippingBin();
        }
        else
        {
            // Player is not holding an item, trigger Shop interaction
            HandleShop();
        }
    }

    private void HandleShop()
    {
        // Open the shop UI
        Debug.Log("Opening Shop");
        UIManager.Instance.OpenShop(shopItems);
    }

    private void HandleShippingBin()
    {
        // Trigger the sell confirmation for the item the player is holding
        ItemData handSlotItem = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Item);
        if (handSlotItem == null) return;

        // Prompt to sell the item immediately
        Debug.Log("Prompting to sell item");
        UIManager.Instance.TriggerYesNoPrompt($"Apakah kamu mau menjual {handSlotItem.name} untuk {handSlotItem.cost} per barang?", ConfirmSell);
    }

    private void ConfirmSell()
    {
        // Get the ItemSlotData of what the player is holding
        ItemSlotData handSlot = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item);
        if (handSlot == null || handSlot.itemData == null) return;

        // Calculate the money to be received
        int moneyReceived = handSlot.quantity * handSlot.itemData.cost;

        // Add money to player stats
        PlayerStats.Earn(moneyReceived);

        // Report quest progress for each item sold
        for (int i = 0; i < handSlot.quantity; i++)
        {
            QuestManager.Instance.ReportAction(QuestData.QuestType.SellCrop, handSlot.itemData.name);
        }

        // Clear the hand slot as the item is sold
        handSlot.Empty();
        InventoryManager.Instance.RenderHand();
    }
}

