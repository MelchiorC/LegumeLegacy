using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSkip : MonoBehaviour
{
    public Boolean onColl = false;
    public GameObject Player;
    public void TimeSkiper()
    {

        if (onColl == true)
        {
            // Skip to next day
            TimeManager.Instance.skipTimeStamp();

            // Manually trigger update (weather, soil, crop growth)
            TimeManager.Instance.Tick();

            Debug.Log("Day skipped. Time updated.");
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            onColl = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        onColl = false;
    }

}

