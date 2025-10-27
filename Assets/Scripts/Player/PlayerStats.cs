using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public static int Money {  get; private set; }

    public const string CURRENCY = ".";
    
    public static void Spend(int cost)
    {
        //Check if the player has enough to spend
        if(cost > Money)
        {
            Debug.LogError("Uang Tidak Cukup!");
            return;
        }
        Money -= cost;
        UIManager.Instance.RenderPlayerStats();
    }

    public static void Earn(int income)
    {
        Money += income;
        UIManager.Instance.RenderPlayerStats();
    }
}
