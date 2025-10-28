using System;
using System.Collections.Generic;
using UnityEngine;

public class ShippingBin : InteractableObject
{

    public override void Pickup()
    {
        ItemData handSlotItem = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Item);
        if (handSlotItem == null) return;

        // Prompt to sell the item immediately
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

        // Log the transaction
        Debug.Log($"Sold {handSlot.itemData.name} x {handSlot.quantity} for {moneyReceived}");

        // Clear the hand slot as the item is sold
        handSlot.Empty();
        InventoryManager.Instance.RenderHand();
    }
}
