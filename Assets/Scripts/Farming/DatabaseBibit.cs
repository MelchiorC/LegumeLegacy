using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseBibit : MonoBehaviour
{
    public List<SeedData> TreliSeed = new List<SeedData>();
    public static DatabaseBibit Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public bool CheckTreli(SeedData data)
    {
        bool found = false;

        for (int i = 0; i < TreliSeed.Count; i++)
        {
            if (TreliSeed[i] == data)
            {

                found = true;
                break;
            }
        }
        return found;
    }
}

