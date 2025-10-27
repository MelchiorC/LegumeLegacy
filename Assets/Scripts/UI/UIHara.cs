using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHara : MonoBehaviour
{
    public GameObject UI;

    public void Show()
    {
        UI.SetActive(true);
    }

    public void Hide() 
    {
        UI.SetActive(false);
    }

}
