using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DraggableInventoryCompost : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public CompostShower compost;
    /* public int idOrigin;// -1 if it is not bag inventor, >=0 otherwise
     public int idDest;// -1 if it is not the crafting ingredient slot, >= 0 otherwise
     public bool isResult;//true if it is the result slot, false otherwise
     public int trueDest = -1;
     public bool occupied; 
     */ //Obsolete


    
    public Vector3 originSnappingPosition;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
        this.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        transform.Translate(eventData.delta);
        this.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //compost.gameObject.GetComponent<CompostShower>().itemMovement(); // handler for the UI to put item and process it
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        GameObject craftingTarget = null;
        GameObject bagTarget = null;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == null)
            {
                continue;
            }

            if (craftingTarget == null && result.gameObject.CompareTag("CraftingSlotCompost"))
            {
                craftingTarget = result.gameObject;
            }
            else if (bagTarget == null && result.gameObject.CompareTag("BagCraftingSlot"))
            {
                bagTarget = result.gameObject;
            }

            if (craftingTarget != null && bagTarget != null)
            {
                break;
            }
        }

        ItemData draggedItem = gameObject.GetComponent<InventorySlot>().GetItemSlotData();

        if (craftingTarget != null && CompostShower.instance.isCompostRecipe(draggedItem))
        {
            this.gameObject.transform.position = craftingTarget.transform.position;
            CompostShower.instance.AddItemToRecipe(draggedItem);
        }
        else if (bagTarget != null)
        {
            this.gameObject.transform.position = bagTarget.transform.position;
            CompostShower.instance.RemoveItemToRecipe(draggedItem);
        }
        else
        {
            this.gameObject.transform.position = originSnappingPosition;
            this.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            CompostShower.instance.RemoveItemToRecipe(draggedItem);
        }
    }

    // Start is called before the first frame update
    public void OnPointerClick(PointerEventData eventData)
    {
        /*int opId = -1;
        if (isResult)
        {
            opId = 2; 
        }else if(idOrigin <= -1)
        {
            opId = 1;
        }else if(idDest <= -1)
        {
            opId = 0;
        }
        compost.gameObject.GetComponent<CompostShower>().itemMovement(isResult,idDest,idOrigin,opId,trueDest);
        */
    }

}
