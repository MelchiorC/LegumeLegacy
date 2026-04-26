using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject panelToToggle;
    [SerializeField] private KeyCode toggleKey = KeyCode.None; 

    private void Update()
    {
        if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if (panelToToggle != null)
            panelToToggle.SetActive(!panelToToggle.activeSelf);
    }

    public void ShowPanel()
    {
        if (panelToToggle != null)
            panelToToggle.SetActive(true);
    }

    public void HidePanel()
    {
        if (panelToToggle != null)
            panelToToggle.SetActive(false);
    }
}


