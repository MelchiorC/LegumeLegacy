using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ITimeTracker
{
    public static UIManager Instance { get; private set; }
    [Header("Status Bar")]
    //Tool equip slot on status bar
    public Image toolEquipSlot;
    //Tool quantity text on the status bar
    public Text toolQuantityText;
    //Time UI
    public Text timeText;
    public Text dateText;

    [Header("Inventory System")]
    //The inventory panel
    public GameObject inventoryPanel;

    //The tool equip slot UI on the inventory panel
    public HandInventorySlot toolHandSlot;

    //The tool slot UI's
    public InventorySlot[] toolSlots;

    //The item equip slot UI on the inventory panel
    public HandInventorySlot itemHandSlot;

    //The item slot UI's
    public InventorySlot[] itemSlots;

    //Item Info box
    public Text itemNameText;
    public Text itemDescriptionText;

    [Header("Yes No Prompt")]
    public YesNoPrompt yesNoPrompt;

    [Header("Player Stats")]
    public Text moneyText;

    [Header("Shop")]
    public ShopListingManager shopListingManager;


    [Header("TimeSkip")]
    public TimeSkip skip;

    [Header("Hara")]
    public GameObject Hara;

    public HaraImage change;
    public void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }

    private void Start()
    {
        RenderInventory();
        AssignSlotIndexes();
        RenderPlayerStats();

        //Add UIManager to the list of objects TimeManager will notify when the time updates
        TimeManager.Instance.RegisterTracker(this);
    }

    public void TriggerYesNoPrompt(string message, System.Action onYesCallback)
    {
        //Set active the gameobject of the Yes No Prompt
        yesNoPrompt.gameObject.SetActive(true);

        yesNoPrompt.CreatePrompt(message, onYesCallback);
    }

    #region Inventory
    //Iterate through the slot UI elements and assign its reference slot index
    public void AssignSlotIndexes()
    {
        for (int i = 0; i < toolSlots.Length; i++)
        {
            toolSlots[i].AssignIndex(i);
            itemSlots[i].AssignIndex(i);
        }
    }

    //Render the inventory screen to reflect the player's inventory
    public void RenderInventory()
    {
        //Get the respective slots to process
        ItemSlotData[] inventoryToolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        ItemSlotData[] inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);
        //Render the tool section
        RenderInventoryPanel(inventoryToolSlots, toolSlots);

        //Render the item section
        RenderInventoryPanel(inventoryItemSlots, itemSlots);

        //Render the equipped slots
        toolHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool));
        itemHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item));

        //Get tool equip from Inventory manager
        ItemData equippedTool = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);

        //Text should be empty by default
        toolQuantityText.text = "";

        //Check if there is an item to display
        if (equippedTool != null)
        {
            //Switch the thumbnail over
            toolEquipSlot.sprite = equippedTool.thumbnail;

            toolEquipSlot.gameObject.SetActive(true);

            //Get quantity
            int quantity = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool).quantity;
            if(quantity > 1)
            {
                toolQuantityText.text = quantity.ToString();
            }

            return;
        }

        toolEquipSlot.gameObject.SetActive(false);
    }

    //Iterate through a slot in a section and display the in the UI
    void RenderInventoryPanel(ItemSlotData[] slots, InventorySlot[] uiSlots)
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            //Display item accordingly
            uiSlots[i].Display(slots[i]);
        }
    }

    public void ToggleInventoryPanel()
    {
        //If the panel is hidden, show it and vice versa
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        RenderInventory();
    }

    //Display item info on the item infobox
    public void DisplayItemInfo(ItemData data)
    {
        //If data is null, reset
        if(data == null)
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";

            return;
        }


        itemNameText.text = data.name;
        itemDescriptionText.text = data.description;
    }
    #endregion

    #region Time
    //Callback to handle the UI for time
    public void ClockUpdate(GameTimestamp timestamp)
    {
        //Handles the time
        //Get the hours and minutes
        int hours = timestamp.hour;
        int minutes = timestamp.minute;

        //AM or PM
        string prefix = "AM ";

        //Convert hours to 12 hour clock
        if(hours > 12)
        {
            //Time becomes PM
            prefix = "PM ";
            hours -= 12;
        }

        //Format it for the time display
        timeText.text = prefix + hours + ":" + minutes.ToString("00");

        //Handles the date
        int day = timestamp.day;
        string season = timestamp.season.ToString();
        string dayOfTheWeek = timestamp.GetDayOfTheWeek().ToString();

        //Format it for the date text diplay
        dateText.text = season + " " + day + " (" + dayOfTheWeek + ")";
    }
    #endregion

    //Render the UI of the player stats in the HUD
    public void RenderPlayerStats()
    {
        moneyText.text = PlayerStats.Money + PlayerStats.CURRENCY;
    }

    //Open the shop window with the shop items listed
    public void OpenShop(List<ItemData> shopItems)
    {
        //Set active the shop window
        shopListingManager.gameObject.SetActive(true);
        shopListingManager.RenderShop(shopItems);
    }

    public void OpenUI(bool water, bool compost, bool stick, bool treli)
    {
        change.IsItWatered(water);
        change.CompostYes(compost);
        change.StickYes(stick, treli);
        Hara.gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        // Hide the UI elements and deactivate the Hara GameObject
        Hara.gameObject.SetActive(false);
    }

}
