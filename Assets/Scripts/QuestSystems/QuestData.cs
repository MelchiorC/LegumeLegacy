using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
public class QuestData : ScriptableObject
{
    public string questName;
    public string description;

    public enum QuestType { Plant, Water, Harvest }
    public QuestType questType;

    public string targetCrop; // e.g., "Potato"
    public int requiredAmount = 1;
}
