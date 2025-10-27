using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestData> questList;
    public TMP_Text questUIText;

    private Quest currentQuest;
    private int currentQuestIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (questList.Count > 0)
        {
            currentQuest = new Quest(questList[0]);
            UpdateUI();
        }
    }

    public void ReportAction(QuestData.QuestType actionType, string cropType, int quantity = 1)
    {
        if (currentQuest == null || currentQuest.isCompleted) return;

        if (currentQuest.data.questType == actionType)
        {
            currentQuest.AddProgress(cropType, quantity);

            if (currentQuest.isCompleted)
            {
                questUIText.text = "Selesai";
                Invoke(nameof(AdvanceToNextQuest), 1.5f);
            }
            else
            {
                UpdateUI();
            }
        }
    }

    private void AdvanceToNextQuest()
    {
        currentQuestIndex++;

        if (currentQuestIndex < questList.Count)
        {
            currentQuest = new Quest(questList[currentQuestIndex]);
            UpdateUI();
        }
        else
        {
            questUIText.text = "Semua quest telah selesai!";
        }
    }

    private void UpdateUI()
    {
        if (currentQuest != null && !currentQuest.isCompleted)
        {
            questUIText.text = $"{currentQuest.data.questName}: {currentQuest.currentAmount}/{currentQuest.data.requiredAmount}";
        }
    }
}
