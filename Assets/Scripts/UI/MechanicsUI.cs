using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MechanicsUI : MonoBehaviour
{
    [Header("Mechanic Entries")]
    [SerializeField] private List<MechanicEntry> mechanicEntries;

    [Header("UI References")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    [SerializeField] private Image mechanicImage;
    [SerializeField] private TMP_Text mechanicTitleText;
    [SerializeField] private TMP_Text mechanicDescriptionText;

    private void Start()
    {
        PopulateMechanicButtons();
        if (mechanicEntries.Count > 0)
            ShowMechanic(mechanicEntries[0]);
    }

    private void PopulateMechanicButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var entry in mechanicEntries)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);
            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = entry.mechanicName;

            Button button = btn.GetComponent<Button>();
            if (button != null)
            {
                MechanicEntry localEntry = entry;
                button.onClick.AddListener(() => ShowMechanic(localEntry));
            }
        }
    }

    private void ShowMechanic(MechanicEntry entry)
    {
        mechanicImage.sprite = entry.mechanicImage;
        mechanicTitleText.text = entry.mechanicName;
        mechanicDescriptionText.text = entry.mechanicDescription;
    }
}
