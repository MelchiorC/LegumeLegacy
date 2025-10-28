using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    PlayerController playerController;


    //Land the player is currently selecting
    Soil selectedSoil = null;

    //The interactable object the player is currently selecting
    InteractableObject selectedInteractable = null;
   
    // Start is called before the first frame update
    void Start()
    {
        //Get access to our PlayerController component
        playerController = transform.parent.GetComponent<PlayerController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5))
        {
            OnInteractableHit(hit);
        }
    }
    
    //Handles what happens when the interaction raycast hits something interactable
    void OnInteractableHit(RaycastHit hit)
    {
        Collider other = hit.collider;
        
        //Check if the player is going to interact with soil
        if(other.tag == "Soil")
        {
            //Get the soil component
            Soil soil = other.GetComponent<Soil>();
            SelectSoil(soil);
            return;
        }

        //Check if the player is going to interact with an item
        if(other.tag == "Item")
        {
            //Set the interactable to the currently selected interactable
            selectedInteractable = other.GetComponent<InteractableObject>();
            return;
        }

        //Deselect the interactable if ther player is not standing on anything at the moment
        if(selectedInteractable != null)
        {
            selectedInteractable = null;
        }

        //Deselect the soil if the player is no standing on any soil at the moment
        if(selectedSoil != null)
        {
            selectedSoil.Select(false);
            selectedSoil = null;
        }
    }
    
    //Handles the selection process
    void SelectSoil(Soil soil)
    {
        //Set the previously selected land to false (if any)
        if (selectedSoil != null)
        {
            selectedSoil.Select(false);
        }

        //Set the new selected land to the land we're selecting now
        selectedSoil = soil;
        soil.Select(true);
    }

    //Triggered when the player presses the tool button
    public void Interact()
    {
        //The player shouldn't be able to use his tool when he has his hand full with an item
        if (InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
        {
            return;
        }

        //Check if the player selecting any land
        if(selectedSoil != null)
        {
            selectedSoil.Interact();
            return;
        }
    }

    //Triggered when the player presses the item interaction button
    public void ItemInteract()
    {
        
        //If the player isn't holding anything, pick up an item
        //Check if there is an interactable selected
        if(selectedInteractable != null)
        {
            //Pick it up
            selectedInteractable.Pickup();
        }
    }

    public void ItemKeep()
    {
        //If the player is holding something, keep it in his inventory
        if (InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
        {
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);
            return;
        }
    }
}
