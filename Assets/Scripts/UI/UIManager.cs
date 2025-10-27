using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ITimeTracker
{
    public static UIManager Instance { get; private set; }

    [Header("Status Bar")]
    public Image toolEquipSlot;
    public Text toolQuantityText;
    public Text timeText;
    public Text dateText;

    [Header("Inventory System")]
    public GameObject inventoryPanel;
    public HandInventorySlot toolHandSlot;
    public HandInventorySlot itemHandSlot;

    [Header("Inventory UI")]
    public Transform itemContentTransform;
    public Transform toolContentTransform;
    public GameObject slotPrefab;

    [Header("Item Info Box")]
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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        RenderInventory();
        RenderPlayerStats();

        // Add UIManager to TimeManager trackers
        TimeManager.Instance.RegisterTracker(this);
    }

    public void TriggerYesNoPrompt(string message, System.Action onYesCallback)
    {
        yesNoPrompt.gameObject.SetActive(true);
        yesNoPrompt.CreatePrompt(message, onYesCallback);
    }

    #region Inventory Management

    public void RenderInventory()
    {
        // Get inventory data
        List<ItemSlotData> inventoryToolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        List<ItemSlotData> inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);

        // Render tool and item sections dynamically
        RenderInventoryPanel(inventoryToolSlots, toolContentTransform, InventorySlot.InventoryType.Tool);
        RenderInventoryPanel(inventoryItemSlots, itemContentTransform, InventorySlot.InventoryType.Item);

        // Render equipped slots
        toolHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool));
        itemHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item));

        // Update tool equip UI
        UpdateToolEquipUI();
    }

    private void RenderInventoryPanel(List<ItemSlotData> slots, Transform contentTransform, InventorySlot.InventoryType inventoryType)
    {
        // Ensure the number of UI slots matches the inventory data
        for (int i = contentTransform.childCount; i < slots.Count; i++)
        {
            AddInventorySlotUI(inventoryType);
        }

        for (int i = 0; i < contentTransform.childCount; i++)
        {
            InventorySlot slotUI = contentTransform.GetChild(i).GetComponent<InventorySlot>();
            if (slotUI != null)
            {
                if (i < slots.Count)
                {
                    // Display the actual inventory data
                    slotUI.Display(slots[i]);
                }
                else
                {
                    // Clear the slot if there's no corresponding data
                    slotUI.Display(new ItemSlotData(null, 0));
                }
            }
        }
    }

    private void UpdateToolEquipUI()
    {
        ItemData equippedTool = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);
        toolQuantityText.text = "";

        if (equippedTool != null)
        {
            toolEquipSlot.sprite = equippedTool.thumbnail;
            toolEquipSlot.gameObject.SetActive(true);

            int quantity = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool).quantity;
            if (quantity > 1)
            {
                toolQuantityText.text = quantity.ToString();
            }
        }
        else
        {
            toolEquipSlot.gameObject.SetActive(false);
        }
    }

    public void ToggleInventoryPanel()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        RenderInventory();
    }

    public void DisplayItemInfo(ItemData data)
    {
        if (data == null)
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";
            return;
        }

        itemNameText.text = data.name;
        itemDescriptionText.text = data.description;
    }

    public void AddInventorySlotUI(InventorySlot.InventoryType inventoryType)
    {
        Transform contentTransform = inventoryType == InventorySlot.InventoryType.Item ? itemContentTransform : toolContentTransform;

        GameObject newSlotUI = Instantiate(slotPrefab, contentTransform);

        InventorySlot slotUI = newSlotUI.GetComponent<InventorySlot>();
        if (slotUI != null)
        {
            slotUI.inventoryType = inventoryType;
            int newIndex = contentTransform.childCount - 1; // Get the new slot's index
            slotUI.AssignIndex(newIndex);

            Debug.Log($"Added new UI slot at index {newIndex} for {inventoryType} inventory.");
        }
    }

    public int GetUIInventorySlotCount(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return itemContentTransform.childCount;
        }
        else if (inventoryType == InventorySlot.InventoryType.Tool)
        {
            return toolContentTransform.childCount;
        }

        return 0;
    }

    #endregion

    #region Time Management

    public void ClockUpdate(GameTimestamp timestamp)
    {
        int hours = timestamp.hour;
        int minutes = timestamp.minute;

        string prefix = hours >= 12 ? "PM " : "AM ";
        hours = hours > 12 ? hours - 12 : hours;

        timeText.text = $"{prefix}{hours}:{minutes:D2}";

        int day = timestamp.day;
        string season = timestamp.season.ToString();
        string dayOfTheWeek = timestamp.GetDayOfTheWeek().ToString();

        dateText.text = $"{season} {day} ({dayOfTheWeek})";
    }

    #endregion

    public void RenderPlayerStats()
    {
        moneyText.text = PlayerStats.Money + PlayerStats.CURRENCY;
    }

    public void OpenShop(List<ItemData> shopItems)
    {
        shopListingManager.gameObject.SetActive(true);
        shopListingManager.RenderShop(shopItems);
    }

    public void OpenUI(bool water, bool compost, bool stick, bool trellis)
    {
        change.IsItWatered(water);
        change.CompostYes(compost);
        change.StickYes(stick, trellis);
        Hara.gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        Hara.gameObject.SetActive(false);
    }
}