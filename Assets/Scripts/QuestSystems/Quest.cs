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

        if (data.targetCrop == cropType)
        {
            currentAmount += quantity;
        }
    }
}
