using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string description;

    //Icon to be displayed in UI
    public Sprite thumbnail;

    //GameObject to be shown in the scence
    public GameObject gameModel;

    //The item costs
    public int cost;

    public int compostmaterial;

}
