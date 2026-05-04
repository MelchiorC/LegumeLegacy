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

    private MechanicCategory currentCategory; //Track Current active category

    private void Start()
    {
        PopulateCategoryTabs();
        if (mechanicCategories.Count > 0)
            SwitchCategory(mechanicCategories[0]);
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
            button.onClick.RemoveAllListeners(); // ← add this
            MechanicCategory localCategory = category;
            button.onClick.AddListener(() => SwitchCategory(localCategory));
        }
    }
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

    // collect first, then destroy
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

//Detects Links from TMP 
//Link are written like: <link="AssetFileName"><color=color>LinkText</color></link>
private void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        //find if the click is on a TMP link from the text
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
        mechanicDescriptionText, Input.mousePosition, null);

        Debug.Log("Link index: " + linkIndex); 

        if (linkIndex != -1)
        {
            // Get the link info and extract the linked entry name, then navigate to it
            TMP_LinkInfo linkInfo = mechanicDescriptionText.textInfo.linkInfo[linkIndex];
            string linkedName = linkInfo.GetLinkID();
            Debug.Log("Linked to: " + linkedName); 
            NavigateToEntry(linkedName);
        }
    }
}

    private void NavigateToEntry(string entryName)
{
    Debug.Log("Looking for: '" + entryName + "'"); 
    foreach (var category in mechanicCategories)
    {
        foreach (var entry in category.entries)
        {
            //Search all categories and entries for a match and then switch to it.
            Debug.Log("Checking: '" + entry.mechanicName + "'"); 
            if (entry.name == entryName)
            {
                SwitchCategory(category);
                ShowMechanic(entry);
                return;
            }
        }
    }
    Debug.Log("Entry not found!"); 
}

    public void OpenEntry(string entryFileName) //For external calls, e.g. from HaraImage
    {
        NavigateToEntry(entryFileName);
    }
    }
