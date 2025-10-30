using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestData data;
    public int currentAmount = 0;
    public bool isCompleted => currentAmount >= data.requiredAmount;

    public Quest(QuestData questData)
    {
        data = questData;
        currentAmount = 0;
    }

    public void AddProgress(string cropType, int quantity = 1)
    {
        if (isCompleted) return;

        Debug.Log($"[Quest] AddProgress: Checking '{data.targetCrop}' == '{cropType}' ? {data.targetCrop == cropType}");

        if (data.targetCrop == cropType)
        {
            currentAmount += quantity;
            Debug.Log($"[Quest] Progress added! New amount: {currentAmount}/{data.requiredAmount}");
        }
        else
        {
            Debug.Log($"[Quest] No match - expected '{data.targetCrop}', got '{cropType}'");
        }
    }
}
