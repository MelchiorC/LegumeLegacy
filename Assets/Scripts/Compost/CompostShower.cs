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
    }
    public Boolean CompostUI()
    {
        recipe = new List<ItemData>();
        spawnedDraggable = new List<GameObject>();
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

            int slots = UI.transform.Find("Inven").childCount;
             k = 0;
            for (int i = 0; i < playerInventory.Count; i++)
            {
            
                if(k < UI.transform.Find("Inven").childCount)
                {
                   // UI.transform.Find("Inven").GetChild(k).GetComponent<Image>().sprite = playerInventory[i].thumbnail;
                   
                    GameObject g = Instantiate(draggablePrefab,UI.transform.Find("Inven"));
                    g.transform.position = UI.transform.Find("Inven").GetChild(k).transform.position;
                    g.GetComponent<InventorySlot>().Display(playerInventory[i]);
                    g.GetComponent<DraggableInventoryCompost>().originSnappingPosition = UI.transform.Find("Inven").GetChild(k).transform.position;
                    spawnedDraggable.Add(g);
                    k++;
                }
              
               /* bool found = false;
                for (int j = 0; j < ints.Count; j++)
                {
                    if (i == ints[j])
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                   
                }*/
            }
        foreach (ItemSlotData item in playerInventory)
        {
            if (item.itemData.compostmaterial > 0)
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
        if (data.compostmaterial == 1)
            return true;
        else
            return false;
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
        GameObject g  = Instantiate(draggablePrefab);
        g.transform.parent = UI.transform.Find("Inven");
        g.transform.position = resultSlots[0].transform.position;
        if(k < UI.transform.Find("Inven").childCount)
        {
            g.GetComponent<DraggableInventoryCompost>().originSnappingPosition = UI.transform.Find("Inven").GetChild(k).transform.position;

        }
        g.GetComponent<InventorySlot>().Display(new ItemSlotData(compost));
        spawnedDraggable.Add(g);
        resultSlots[0].GetComponent<Image>().sprite = defaultSprite;
        InventoryManager.Instance.ShopToInventory(new ItemSlotData(compost));
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
