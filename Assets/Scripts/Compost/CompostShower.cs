using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompostShower : MonoBehaviour
{

    public GameObject UI;
    public Boolean OnTrigger = false;
    public List<ItemSlotData> playerInventory;
    public List<ItemSlotData> craftable;
    
    public List<GameObject> craftingSlots;
    public List<GameObject> bagSlots;
    public List<GameObject> resultSlots;
    public int lastEmptyCraftingSlotId;
    public Sprite defaultSprite;
    [SerializeField]
    public ItemData compost;
    public ItemData pupukResult;
    public ItemData pestisidaNabatiResult;

    [Header("Compost Recipe")]
    public ItemData sisaTanaman;
    public ItemData kotoranHewan;

    [Header("Pestisida Nabati Recipe")]
    public ItemData cabai;
    public ItemData bawangPutih;
    public ItemData emptyBottle;

    public GameObject draggablePrefab;
    public List<ItemData> recipe;
    public static CompostShower instance;
    List<GameObject> spawnedDraggable;
    public int k;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        // Keep compatibility with old inspector setup that used only `compost`.
        if (pupukResult == null)
        {
            pupukResult = compost;
        }
    }
    public Boolean CompostUI()
    {
        recipe = new List<ItemData>();
        spawnedDraggable = new List<GameObject>();
        craftable = new List<ItemSlotData>();
        if (OnTrigger == true)
        {
            playerInventory = new List<ItemSlotData>();
            List<ItemSlotData> temp = new List<ItemSlotData>();
            resultSlots[0].GetComponent<Image>().sprite = null;
            foreach(GameObject g in craftingSlots)
            {
                
            }
            UI.SetActive(true);
            playerInventory = InventoryManager.Instance.GetAllInventoryItems();
            temp = InventoryManager.Instance.GetAllInventoryItems();
            List<int> ints = new List<int>();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i] != null)
                {
                    if (temp[i].itemData.compostmaterial == 0)
                    {
                        ints.Add(i);
                    }
                }
            }

            RebuildBagDraggables();

        foreach (ItemSlotData item in playerInventory)
        {
            if (item != null && item.itemData != null && item.itemData.compostmaterial > 0)
            {
                craftable.Add(item);
            }
        }
        return true;
        }

        return false;
    }
    public void HideUI()
    {
        UI.SetActive(false);
        foreach(GameObject g in spawnedDraggable)
        {
            Destroy(g);
        }
    }
    public bool isCompostRecipe(ItemData data)
    {
        if (data == null)
        {
            return false;
        }

        if (data == sisaTanaman || data == kotoranHewan || data == cabai || data == bawangPutih || data == emptyBottle)
        {
            return true;
        }

        // Fallback for older data setup.
        return data.compostmaterial == 1;
    }
    public void AddItemToRecipe(ItemData data)
    {
        bool found = false;
        foreach(ItemData item in recipe)
        {
            if(item == data)
            {
                found = true;
                break;
            }
        }
        if (!found)
        {
            recipe.Add(data);
        }
       
        
    }
    public void RemoveItemToRecipe(ItemData data)
    {
        bool found = false;
        foreach (ItemData item in recipe)
        {
            if (item == data)
            {
                found = true;
                break;
            }
        }
        if (found)
        {
            recipe.Remove(data);
        }
    }
    
    public void Craft()
    {
        ItemData craftedItem = GetCraftResult();
        if (craftedItem == null)
        {
            Debug.LogWarning("[CompostShower] Recipe tidak cocok. Gunakan Sisa Tanaman + Kotoran Hewan untuk pupuk, atau Cabai + Bawang Putih + Empty Bottle untuk pestisida nabati.");
            return;
        }

        List<ItemData> ingredientsToConsume = new List<ItemData>(recipe);
        if (!TryConsumeRecipeIngredients(ingredientsToConsume))
        {
            Debug.LogWarning("[CompostShower] Gagal crafting karena bahan tidak ditemukan di inventory.");
            return;
        }

        resultSlots[0].GetComponent<Image>().sprite = defaultSprite;
        InventoryManager.Instance.ShopToInventory(new ItemSlotData(craftedItem));
        recipe.Clear();
        RefreshCraftingUI();
    }

    private void RefreshCraftingUI()
    {
        playerInventory = InventoryManager.Instance.GetAllInventoryItems();

        foreach (GameObject slot in craftingSlots)
        {
            Image image = slot.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = defaultSprite;
            }
        }

        RebuildBagDraggables();
    }

    private void RebuildBagDraggables()
    {
        if (spawnedDraggable == null)
        {
            spawnedDraggable = new List<GameObject>();
        }

        foreach (GameObject spawned in spawnedDraggable)
        {
            if (spawned != null)
            {
                Destroy(spawned);
            }
        }

        spawnedDraggable.Clear();

        Transform invenTransform = UI.transform.Find("Inven");
        k = 0;
        for (int i = 0; i < playerInventory.Count; i++)
        {
            if (k < invenTransform.childCount)
            {
                GameObject g = Instantiate(draggablePrefab, invenTransform);
                g.transform.position = invenTransform.GetChild(k).transform.position;
                g.GetComponent<InventorySlot>().Display(playerInventory[i]);
                g.GetComponent<DraggableInventoryCompost>().originSnappingPosition = invenTransform.GetChild(k).transform.position;
                spawnedDraggable.Add(g);
                k++;
            }
        }
    }

    private bool TryConsumeRecipeIngredients(List<ItemData> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
        {
            return false;
        }

        List<ItemSlotData> inventoryItems = InventoryManager.Instance.GetAllInventoryItems();

        // Validate all ingredients first to avoid partial consumption.
        foreach (ItemData ingredient in ingredients)
        {
            bool found = false;
            foreach (ItemSlotData slot in inventoryItems)
            {
                if (slot != null && !slot.IsEmpty() && slot.itemData == ingredient && slot.quantity > 0)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        foreach (ItemData ingredient in ingredients)
        {
            ItemSlotData slotToConsume = null;
            foreach (ItemSlotData slot in inventoryItems)
            {
                if (slot != null && !slot.IsEmpty() && slot.itemData == ingredient && slot.quantity > 0)
                {
                    slotToConsume = slot;
                    break;
                }
            }

            if (slotToConsume == null)
            {
                return false;
            }

            InventoryManager.Instance.ConsumeItem(slotToConsume);
        }

        return true;
    }

    private ItemData GetCraftResult()
    {
        if (HasExactRecipe(sisaTanaman, kotoranHewan) || HasExactRecipeByName("Sisa Tanaman", "Kotoran Hewan"))
        {
            return pupukResult;
        }

        if (HasExactRecipe(cabai, bawangPutih, emptyBottle) || HasExactRecipeByName("Cabai", "Bawang Putih", "Empty Bottle") || HasExactRecipeByName("Cabai", "Bawang Putih", "Botol Kosong"))
        {
            return pestisidaNabatiResult;
        }

        return null;
    }

    private bool HasExactRecipe(params ItemData[] requiredItems)
    {
        if (recipe == null || requiredItems == null || recipe.Count != requiredItems.Length)
        {
            return false;
        }

        List<ItemData> remaining = new List<ItemData>(recipe);
        foreach (ItemData item in requiredItems)
        {
            if (item == null)
            {
                return false;
            }

            int index = remaining.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            remaining.RemoveAt(index);
        }

        return remaining.Count == 0;
    }

    private bool HasExactRecipeByName(params string[] requiredNames)
    {
        if (recipe == null || requiredNames == null || recipe.Count != requiredNames.Length)
        {
            return false;
        }

        List<string> remainingNames = new List<string>();
        foreach (ItemData item in recipe)
        {
            if (item == null)
            {
                return false;
            }

            remainingNames.Add(item.name.ToLowerInvariant());
        }

        foreach (string requiredName in requiredNames)
        {
            int index = remainingNames.IndexOf(requiredName.ToLowerInvariant());
            if (index < 0)
            {
                return false;
            }

            remainingNames.RemoveAt(index);
        }

        return remainingNames.Count == 0;
    }
    

    public void itemMovement(bool isResultSlot, int destId, int originId, int operation, int trueDest )
    {
        Debug.Log(originId);
        Debug.Log(destId);
        
        
        //
        switch (operation)
        {
            //0 from bag to crafting slot, 1 from crafting slot back to bag, 2 from result to bag
            case 0:
                if (playerInventory[originId].itemData.compostmaterial <= 0)
                    return;
                craftingSlots[lastEmptyCraftingSlotId].GetComponent<Image>().sprite = craftable[originId].itemData.thumbnail;
                //craftingSlots[lastEmptyCraftingSlotId].GetComponent<DraggableInventoryCompost>().trueDest = originId;
                bagSlots[originId].GetComponent<Image>().sprite = defaultSprite;
                
                    if(lastEmptyCraftingSlotId < craftingSlots.Count - 1)
                    {
                        lastEmptyCraftingSlotId++;
                    }
                   break;
            case 1:
                craftingSlots[destId].GetComponent<Image>().sprite = defaultSprite;
                bagSlots[trueDest].GetComponent<Image>().sprite = craftable[trueDest].itemData.thumbnail; ;
                if (lastEmptyCraftingSlotId > 0)
                {
                    lastEmptyCraftingSlotId--;
                }
                break;
            case 2:
                bagSlots[2].GetComponent<Image>().sprite = compost.thumbnail;
                resultSlots[0].GetComponent<Image>().sprite = defaultSprite;
                InventoryManager.Instance.ShopToInventory(new ItemSlotData(compost));
                //calculate last filledBag;
                break;
                
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    {
        OnTrigger = false;
    }
}
