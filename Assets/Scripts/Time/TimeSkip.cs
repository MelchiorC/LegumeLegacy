using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSkip : MonoBehaviour
{
    public Boolean onColl =false;
    public GameObject Player;
    public void TimeSkiper()
    {
        
        if (onColl == true)
        {
            TimeManager.Instance.skipTimeStamp();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            onColl = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        onColl = false;
    }
}

