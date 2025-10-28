using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestData> questList;
    public TMP_Text questUIText;

    private List<Quest> activeQuests = new List<Quest>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (QuestData data in questList)
        {
            activeQuests.Add(new Quest(data));
        }
        UpdateUI();
    }

    public void ReportAction(QuestData.QuestType actionType, string cropType)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.data.questType == actionType && !quest.isCompleted)
            {
                quest.AddProgress(cropType);
            }
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        string display = "";
        foreach (Quest quest in activeQuests)
        {
            display += $"{quest.data.questName}: {quest.currentAmount}/{quest.data.requiredAmount}\n";
        }
        questUIText.text = display;
    }
}
