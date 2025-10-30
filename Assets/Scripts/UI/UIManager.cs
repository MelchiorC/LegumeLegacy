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

    //Slot prefabs for dynamic creation
    public GameObject inventorySlotPrefab;
    public Transform toolSlotContainer;
    public Transform itemSlotContainer;

    //The tool equip slot UI on the inventory panel
    public HandInventorySlot toolHandSlot;

    //The tool slot UI's (dynamic)
    private List<InventorySlot> toolSlots = new List<InventorySlot>();

    //The item equip slot UI on the inventory panel
    public HandInventorySlot itemHandSlot;

    //The item slot UI's (dynamic)
    private List<InventorySlot> itemSlots = new List<InventorySlot>();

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

    [Header("Dictionary Panels")]
    [SerializeField] private GameObject uiControlsPanel;
    [SerializeField] private GameObject uiMechanicsPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private KeyCode dictionaryToggleKey = KeyCode.None;
    private int currentPanelIndex = -1;

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
        //Initialize UI slots to match inventory size
        InitializeInventoryUI();
        
        RenderInventory();
        RenderPlayerStats();

        //Add UIManager to the list of objects TimeManager will notify when the time updates
        TimeManager.Instance.RegisterTracker(this);

        // Initialize dictionary panels as hidden
        InitializeDictionaryPanels();
    }

    private void InitializeInventoryUI()
    {
        //Find and add existing tool slots from the container
        if (toolSlotContainer != null)
        {
            InventorySlot[] existingToolSlots = toolSlotContainer.GetComponentsInChildren<InventorySlot>();
            foreach (var slot in existingToolSlots)
            {
                if (slot.inventoryType == InventorySlot.InventoryType.Tool)
                {
                    toolSlots.Add(slot);
                }
            }
            //Reassign indices
            for (int i = 0; i < toolSlots.Count; i++)
            {
                toolSlots[i].AssignIndex(i);
            }
        }

        //Find and add existing item slots from the container
        if (itemSlotContainer != null)
        {
            InventorySlot[] existingItemSlots = itemSlotContainer.GetComponentsInChildren<InventorySlot>();
            foreach (var slot in existingItemSlots)
            {
                if (slot.inventoryType == InventorySlot.InventoryType.Item)
                {
                    itemSlots.Add(slot);
                }
            }
            //Reassign indices
            for (int i = 0; i < itemSlots.Count; i++)
            {
                itemSlots[i].AssignIndex(i);
            }
        }

        //Get inventory sizes from InventoryManager
        ItemSlotData[] inventoryToolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        ItemSlotData[] inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);

        //Create additional tool slot UI elements if needed
        if (inventoryToolSlots.Length > toolSlots.Count)
        {
            CreateInventorySlots(inventoryToolSlots.Length, InventorySlot.InventoryType.Tool);
        }
        
        //Create additional item slot UI elements if needed
        if (inventoryItemSlots.Length > itemSlots.Count)
        {
            CreateInventorySlots(inventoryItemSlots.Length, InventorySlot.InventoryType.Item);
        }
    }

    private void CreateInventorySlots(int count, InventorySlot.InventoryType type)
    {
        List<InventorySlot> slotList = type == InventorySlot.InventoryType.Tool ? toolSlots : itemSlots;
        Transform container = type == InventorySlot.InventoryType.Tool ? toolSlotContainer : itemSlotContainer;

        //Create only the slots that don't exist yet
        int currentCount = slotList.Count;
        
        for (int i = currentCount; i < count; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, container);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            slot.inventoryType = type;
            slot.AssignIndex(i);
            slotList.Add(slot);
        }

        //Force layout rebuild for proper positioning
        if (container != null)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());
        }
    }

    private void Update()
    {
        // Handle dictionary panel toggle
        if (dictionaryToggleKey != KeyCode.None && Input.GetKeyDown(dictionaryToggleKey))
        {
            ToggleDictionaryPanel();
        }
    }

    public void TriggerYesNoPrompt(string message, System.Action onYesCallback)
    {
        //Set active the gameobject of the Yes No Prompt
        yesNoPrompt.gameObject.SetActive(true);

        yesNoPrompt.CreatePrompt(message, onYesCallback);
    }

    #region Inventory
    //Render the inventory screen to reflect the player's inventory
    public void RenderInventory()
    {
        //Get the respective slots to process
        ItemSlotData[] inventoryToolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        ItemSlotData[] inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);
        
        //Check if we need to create more UI slots (inventory was expanded)
        if (inventoryToolSlots.Length > toolSlots.Count)
        {
            CreateInventorySlots(inventoryToolSlots.Length, InventorySlot.InventoryType.Tool);
        }
        if (inventoryItemSlots.Length > itemSlots.Count)
        {
            CreateInventorySlots(inventoryItemSlots.Length, InventorySlot.InventoryType.Item);
        }

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
    void RenderInventoryPanel(ItemSlotData[] slots, List<InventorySlot> uiSlots)
    {
        for (int i = 0; i < slots.Length && i < uiSlots.Count; i++)
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

    #region Dictionary Panels
    private void InitializeDictionaryPanels()
    {
        // Both are hidden at start
        if (uiControlsPanel != null)
            uiControlsPanel.SetActive(false);

        if (uiMechanicsPanel != null)
            uiMechanicsPanel.SetActive(false);

        if (backgroundPanel != null)
            backgroundPanel.SetActive(false);
    }

    public void ToggleDictionaryPanel()
    {
        currentPanelIndex = (currentPanelIndex + 1) % 3;

        switch (currentPanelIndex)
        {
            case 0: // Show Controls
                ShowControls();
                break;
            case 1: // Show Mechanics
                ShowMechanics();
                break;
            case 2: // Hide All
                HideDictionaryPanels();
                currentPanelIndex = -1;
                break;
        }
    }

    // Shows the Controls panel
    public void ShowControls()
    {
        if (backgroundPanel != null)
            backgroundPanel.SetActive(true);

        if (uiControlsPanel != null)
            uiControlsPanel.SetActive(true);

        if (uiMechanicsPanel != null)
            uiMechanicsPanel.SetActive(false);

        currentPanelIndex = 0;
    }

    // Shows the Mechanics panel
    public void ShowMechanics()
    {
        if (backgroundPanel != null)
            backgroundPanel.SetActive(true);

        if (uiControlsPanel != null)
            uiControlsPanel.SetActive(false);

        if (uiMechanicsPanel != null)
            uiMechanicsPanel.SetActive(true);

        currentPanelIndex = 1;
    }

    // Hides all dictionary panels
    public void HideDictionaryPanels()
    {
        if (backgroundPanel != null)
            backgroundPanel.SetActive(false);

        if (uiControlsPanel != null)
            uiControlsPanel.SetActive(false);

        if (uiMechanicsPanel != null)
            uiMechanicsPanel.SetActive(false);

        currentPanelIndex = -1;
    }

    public void ToggleDictionaryVisibility()
    {
        if (backgroundPanel != null && backgroundPanel.activeSelf)
        {
            HideDictionaryPanels();
        }
        else
        {
            // Show the last active panel, or Controls if none was active
            if (currentPanelIndex == 0)
                ShowControls();
            else if (currentPanelIndex == 1)
                ShowMechanics();
            else
                ShowControls();
        }
    }
    #endregion

}
