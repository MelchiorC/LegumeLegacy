using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mesin : MonoBehaviour
{
    public float currentcapacity;
    public float maxcapacity;
    public float dispenseamount;
    public Boolean onUI = false;
    public Boolean onCall = false;
    public GameObject ui;
    public GameTimestamp time = null;

    private void Update()
    {
        
    }


    void Start()
    {
        currentcapacity = maxcapacity;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentcapacity >= dispenseamount)
        {
            onCall = true;
            currentcapacity -= dispenseamount;
        }
        else if (other.CompareTag("RefillTrigger"))
        {
            currentcapacity = maxcapacity;
        }

        
    }

    private void OnCollisionExit(Collision collision)
    {
        onCall = false;
    }
    public void addpupuk(float amount)
    {
        currentcapacity = Mathf.Clamp(currentcapacity + amount, 0, maxcapacity);
        
        if (time == null) 
        {
            time = TimeManager.Instance.GetGameTimestamp();
        }
        
    }   

    public bool Dispensepupuk(float amount)
    {
        if (time != null)
        {
            GameTimestamp curr = TimeManager.Instance.GetGameTimestamp();
            int hoursElapsed = GameTimestamp.CompareTimestamps(time, curr);
            if (hoursElapsed > 0)
            {
                time = null;
            }
        }
        if (currentcapacity >= amount)
        {
            currentcapacity -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

}  
    


