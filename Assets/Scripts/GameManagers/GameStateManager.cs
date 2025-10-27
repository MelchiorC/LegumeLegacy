using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour//, ITimeTracker
{
    public static GameStateManager Instance {  get; private set; }

    public void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Add this to TimeManager's Listener list
        //TimeManager.Instance.RegisterTracker(this);
    }

    //public void ClockUpdate(GameTimestamp timestamp)
    //{
    //    UpdateShippingState(timestamp);
    //}

    //void UpdateShippingState(GameTimestamp timestamp)
    //{
    //    ShippingBin.ShipItems();
    //    Debug.Log("Item has been shipped");  
    //}
}
