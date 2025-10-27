using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopShower : MonoBehaviour
{
    public Boolean onColl = false;
    public GameObject Player;
    public GameObject ShopUI;
    public Boolean isON = false;
    public int val = 0;

    // Update is called once per frame
    public void ShopShow()
    {
        
            if(onColl == true)
            {
                if (ShopUI.activeSelf)
                {
                    ShopUI.SetActive(false);
                    isON = false;
                }
                else
                {
                    isON = true;
                    ShopUI.SetActive(true);
                }
            }
        

    }

    public int UIActive()
    {
        if (isON == true)
        {
            val = 1;
        }
        if(isON == false)
        {
            val = 0;
        }
        return val;
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
