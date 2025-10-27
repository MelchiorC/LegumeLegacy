using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SoilStat
{
    [SerializeField]

    private int BaseVal;
    public int GetValue()
    {
        return BaseVal;
    }
}
