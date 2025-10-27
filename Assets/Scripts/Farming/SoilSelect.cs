using UnityEngine;
using UnityEngine.EventSystems;

public class SoilSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    void ApplyToAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Add SoilSelect script to the child
            SoilSelect soilSelect = child.gameObject.AddComponent<SoilSelect>();
            soilSelect.enabled = true;
            PopUp popUp = child.gameObject.AddComponent<PopUp>();

            // Recursively apply to grandchildren
            ApplyToAllChildren(child);
        }
    }
    
    void Start()
    {
        // Apply the SoilSelect script to all children recursively
        ApplyToAllChildren(transform);
        
    }

    private void Update()
    {
        // Check if the highlighted soil object exists
        if (highlightedSoil != null)
        {
            // Highlight the soil
            

            // Get the position of the highlighted soil and update the PopUp component
            PopUp popupScript = GetComponent<PopUp>();
            popupScript.ReceiveHighlightedPosition(highlightedSoil.position);
        }
    }

    private Transform highlightedSoil;

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            GameObject enteredObject = eventData.pointerCurrentRaycast.gameObject;
            if (enteredObject.CompareTag("Soil") && enteredObject != highlightedSoil)
            {
                highlightedSoil = enteredObject.transform;
                HighlightSoil(highlightedSoil);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        if (highlightedSoil != null)
        {
            UnhighlightSoil(highlightedSoil);
            highlightedSoil = null;
        }
    }

    private void HighlightSoil(Transform soil)
    {
        if (soil.GetComponent<Outline>() == null)
        {
            Outline outline = soil.gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.green;
            outline.OutlineWidth = 7.0f;

            
        }
        else
        {
            soil.GetComponent<Outline>().enabled = true;
        }
    }

    private void UnhighlightSoil(Transform soil)
    {
        if (soil.GetComponent<Outline>() != null)
        {
            soil.GetComponent<Outline>().enabled = false;
        }
    }

   
}