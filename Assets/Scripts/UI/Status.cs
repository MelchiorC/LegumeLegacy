using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public string message;

    private void OnMouseEnter()
    {
        StatsShower._instance.SetAnsshow(message);
    }

    private void OnMouseExit()
    {
        StatsShower._instance.HideTool();
    }
}
