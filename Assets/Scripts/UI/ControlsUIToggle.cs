using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsUIToggle : MonoBehaviour
{
    public GameObject controlsPanel;  
    public KeyCode toggleKey = KeyCode.H;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
    }
}
