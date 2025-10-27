using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopListingManager : MonoBehaviour
{
    //The shop Listing entry prefab to instantiate
    public GameObject shopListing;
    //The transform of the grid to instantiate the entries on
    public Transform listingGrid;

    //Variables to keep track of what the player is trying to purchase (selection)
    ItemData itemToBuy;
    int quantity;

    [Header("Confirmation Screen")]
    public GameObject confirmationScreen;
    public Text confirmationPrompt;
    public Text quantityText;
    public Text costCalculationText;
    public Button purchaseButton;
    public GameObject ListingGrid;

    public void RenderShop(List<ItemData> shopItems)
    {
        ListingGrid.SetActive(true);
        confirmationScreen.SetActive(false);
        //Reset the listings if there was a previous one
        if(listingGrid.childCount > 0)
        {
            foreach(Transform child in listingGrid)
            {
                Destroy(child.gameObject);
            }
        }

        //Create a new listing for every item
        foreach(ItemData shopItem in shopItems)
        {
            //Instantiate a shop listing prefab for the item
            GameObject listingGameObject = Instantiate(shopListing, listingGrid);

            //Assign it the shop item and display listing
            listingGameObject.GetComponent<ShopListing>().Display(shopItem);
        }
    }

    public void OpenConfirmationScreen(ItemData item)
    {
        itemToBuy = item;
        quantity = 1;
        RenderConfirmationScreen();
    }

    public void RenderConfirmationScreen()
    {
        confirmationScreen.SetActive(true);

        ListingGrid.SetActive(false);

        confirmationPrompt.text = $"Beli {itemToBuy.name}?";

        quantityText.text = "x" + quantity;

        int cost = itemToBuy.cost * quantity;

        int playerMoneyLeft = PlayerStats.Money - cost;

        //Stop the player from purchasing the item if the player does not have enough money
        if(playerMoneyLeft < 0)
        {
            costCalculationText.text = "Uang Tidak Cukup!";
            purchaseButton.interactable = false;
            return;
        }

        purchaseButton.interactable = true; 

        costCalculationText.text = $"{PlayerStats.Money} > {playerMoneyLeft} ";
    }

    public void AddQuantity()
    {
        quantity++;
        RenderConfirmationScreen();
    }

    public void SubstractQuantity()
    {
        if(quantity > 1)
        {
            quantity--;
        }
        RenderConfirmationScreen();
    }
    
    //Purchase the item and close the confirmation screen
    public void ConfirmPurchase()
    {
        Shop.Purchase(itemToBuy, quantity);
        confirmationScreen.SetActive(false);
    }

    public void CancelPurchase()
    {
        confirmationScreen.SetActive(false);
        ListingGrid.SetActive(true);
    }
}
