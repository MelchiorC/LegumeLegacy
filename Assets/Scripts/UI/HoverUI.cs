using UnityEngine;

public class HoverUI : MonoBehaviour
{
    // Assign your UI element (like a Text or Panel) in the Inspector
    public GameObject uiPopup;

    // Assign your camera if it's not the main camera
    public Camera mainCamera;

    void Start()
    {
        // Ensure the UI is hidden at the start
        if (uiPopup != null)
        {
            uiPopup.SetActive(false);
        }

        // Use the main camera by default if none is assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Check if the player is holding PH and hovering over soil
        if (PlayerIsHoldingPH() && IsHoveringOverSoil())
        {
            // Show the UI
            if (uiPopup != null && !uiPopup.activeSelf)
            {
                uiPopup.SetActive(true);
            }
        }
        else
        {
            // Hide the UI if not hovering or not holding PH
            if (uiPopup != null && uiPopup.activeSelf)
            {
                uiPopup.SetActive(false);
            }
        }
    }

    // Check if the player is holding the PH tool
    bool PlayerIsHoldingPH()
    {
        // Get the player's equipped item from the inventory (assumed method)
        ItemData equippedItem = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);

        // Check if the equipped item is EquipmentData and specifically a PH tool
        if (equippedItem is EquipmentData equipment && equipment.toolType == EquipmentData.ToolType.PH)
        {
            return true; // Player is holding the PH tool
        }
        return false; // Player is not holding the PH tool
    }

    // Check if the object being hovered over is tagged as "Soil"
    bool IsHoveringOverSoil()
    {
        // Raycast from the mouse position into the scene
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform raycast to detect objects
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the object hit by the raycast has the "Soil" tag
            if (hit.collider != null && hit.collider.CompareTag("Soil"))
            {
                return true; // Player is hovering over a soil object
            }
        }
        return false; // Not hovering over soil
    }
}
