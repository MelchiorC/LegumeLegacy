using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MechanicsUI : MonoBehaviour
{
    [Header("Categories")]
    [SerializeField] private List<MechanicCategory> mechanicCategories;

    [Header("UI References")]
    [SerializeField] private GameObject tabButtonPrefab;
    [SerializeField] private Transform tabContainer;

    [SerializeField] private GameObject entryButtonPrefab;
    [SerializeField] private Transform entryButtonContainer;

    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private Image mechanicImage;
    [SerializeField] private TMP_Text mechanicTitleText;
    [SerializeField] private TMP_Text mechanicDescriptionText;

    // Colors for active and inactive tab states
    [SerializeField] private Color activeTabColor   = new Color(0.91f, 0.75f, 0.38f);   // #E8C060
    [SerializeField] private Color inactiveTabColor = new Color(0.23f, 0.42f, 0.13f);   // #3A6A20

    [SerializeField] private Color activeTabTextColor = new Color(0.23f, 0.13f, 0f);   // #3A2000
    [SerializeField] private Color inactiveTabTextColor = new Color(0.83f, 0.94f, 0.63f); // #D4F0A0

    private MechanicCategory currentCategory;
    private Button activeTabButton;

    private void Start()
    {
        PopulateCategoryTabs();
        if (mechanicCategories.Count > 0)
        {
            Button firstTab = tabContainer.GetChild(0).GetComponent<Button>();
            SetActiveTab(firstTab);
            SwitchCategory(mechanicCategories[0]);
        }
    }

    private void PopulateCategoryTabs()
    {
        foreach (Transform child in tabContainer)
            Destroy(child.gameObject);

        foreach (var category in mechanicCategories)
        {
            GameObject tab = Instantiate(tabButtonPrefab, tabContainer);
            TMP_Text tabText = tab.GetComponentInChildren<TMP_Text>();
            if (tabText != null)
                tabText.text = category.categoryName;

            Button button = tab.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                MechanicCategory localCategory = category;
                Button localButton = button;
                button.onClick.AddListener(() =>
                {
                    SetActiveTab(localButton);
                    SwitchCategory(localCategory);
                });
            }
        }
    }

    // Highlights the selected tab and resets all others
    private void SetActiveTab(Button selectedButton)
    {
    foreach (Transform child in tabContainer)
    {
        Button btn = child.GetComponent<Button>();
        if (btn != null)
        {
            btn.image.color = inactiveTabColor;
            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.color = inactiveTabTextColor;
        }
    }

    selectedButton.image.color = activeTabColor;
    TMP_Text activeText = selectedButton.GetComponentInChildren<TMP_Text>();
    if (activeText != null) activeText.color = activeTabTextColor;
    activeTabButton = selectedButton;
    }

    private void SwitchCategory(MechanicCategory category)
    {
        currentCategory = category;
        PopulateEntryButtons();
        if (category.entries.Count > 0)
            ShowMechanic(category.entries[0]);
    }

    private void PopulateEntryButtons()
    {
        scrollRect.enabled = false;

        List<GameObject> toDelete = new List<GameObject>();
        foreach (Transform child in entryButtonContainer)
            toDelete.Add(child.gameObject);
        foreach (GameObject obj in toDelete)
            DestroyImmediate(obj);

        foreach (var entry in currentCategory.entries)
        {
            GameObject btn = Instantiate(entryButtonPrefab, entryButtonContainer);
            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = entry.mechanicName;

            Button button = btn.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                MechanicEntry localEntry = entry;
                button.onClick.AddListener(() => ShowMechanic(localEntry));
            }
        }

        scrollRect.enabled = true;
    }

    private void ShowMechanic(MechanicEntry entry)
    {
        mechanicImage.sprite = entry.mechanicImage;
        mechanicTitleText.text = entry.mechanicName;
        mechanicDescriptionText.text = entry.mechanicDescription;
    }

    // Detects clicks on TMP link tags in the description text and navigates to the linked entry
    // Links are written like: <link="AssetFileName"><color=color>LinkText</color></link>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(
                mechanicDescriptionText, Input.mousePosition, null);

            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = mechanicDescriptionText.textInfo.linkInfo[linkIndex];
                string linkedName = linkInfo.GetLinkID();
                NavigateToEntry(linkedName);
            }
        }
    }

    // Searches all categories for an entry matching the asset filename and navigates to it
    private void NavigateToEntry(string entryName)
    {
        foreach (var category in mechanicCategories)
        {
            foreach (var entry in category.entries)
            {
                if (entry.name == entryName)
                {
                    // update tab highlight if navigating to a different category
                    if (category != currentCategory)
                    {
                        int categoryIndex = mechanicCategories.IndexOf(category);
                        Button tabButton = tabContainer.GetChild(categoryIndex).GetComponent<Button>();
                        SetActiveTab(tabButton);
                    }

                    SwitchCategory(category);
                    ShowMechanic(entry);
                    return;
                }
            }
        }
    }

    // Public method for external scripts (e.g. UIManager) to open a specific entry by asset filename
    public void OpenEntry(string entryFileName)
    {
        NavigateToEntry(entryFileName);
    }
}