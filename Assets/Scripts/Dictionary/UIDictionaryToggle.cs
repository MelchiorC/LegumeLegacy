using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDictionaryToggle : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject uiControlsPanel;
    [SerializeField] private GameObject uiMechanicsPanel;
    [SerializeField] private GameObject backgroundPanel;

    [Header("Toggle Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.None;

    private int currentPanelIndex = -1;

    private void Start()
    {
        // both are hidden at start
        if (uiControlsPanel != null)
            uiControlsPanel.SetActive(false);

        if (uiMechanicsPanel != null)
            uiMechanicsPanel.SetActive(false);

        
        if (backgroundPanel != null)
            backgroundPanel.SetActive(false);
    }

    private void Update()
    {
        if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
        {
            ToggleDictionaryPanel();
        }
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
                HideAll();
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

    // Hides all
    public void HideAll()
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
            HideAll();
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
}