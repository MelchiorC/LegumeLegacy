using System.Collections.Generic;
using UnityEngine;

public class ItemPickupBanner : MonoBehaviour
{
    public static ItemPickupBanner Instance { get; private set; }

    [Header("Banner Settings")]
    [Tooltip("Parent container for banner entries (should have a VerticalLayoutGroup).")]
    public Transform entryContainer;

    [Tooltip("Prefab for a single banner entry. Must have ItemPickupBannerEntry component.")]
    public GameObject entryPrefab;

    [Tooltip("Maximum number of visible entries at once.")]
    public int maxVisibleEntries = 5;

    // Track active entries so we can stack same-item pickups
    private List<ItemPickupBannerEntry> activeEntries = new List<ItemPickupBannerEntry>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Hide the prefab template so the placeholder is not visible in the scene
        if (entryPrefab != null)
        {
            entryPrefab.SetActive(false);
        }
    }

    public void ShowPickup(ItemData item, int quantity = 1)
    {
        if (item == null) return;

        // Clean up destroyed/null entries first
        activeEntries.RemoveAll(e => e == null);

        // Check if there's already an active entry for this item — stack it
        for (int i = 0; i < activeEntries.Count; i++)
        {
            if (activeEntries[i] != null && activeEntries[i].GetItemData() == item)
            {
                activeEntries[i].AddQuantity(quantity);
                return;
            }
        }

        // Enforce max visible limit — remove the oldest entry if needed
        if (activeEntries.Count >= maxVisibleEntries)
        {
            RemoveEntry(activeEntries[0]);
            if (activeEntries.Count > 0 && activeEntries[0] != null)
            {
                Destroy(activeEntries[0].gameObject);
                activeEntries.RemoveAt(0);
            }
        }

        // Spawn a new entry
        if (entryPrefab == null || entryContainer == null)
        {
            Debug.LogWarning("[ItemPickupBanner] Entry prefab or container is not assigned!");
            return;
        }

        // Ensure the container is active so entries are visible
        if (!entryContainer.gameObject.activeSelf)
        {
            entryContainer.gameObject.SetActive(true);
        }

        GameObject entryObj = Instantiate(entryPrefab, entryContainer);
        entryObj.SetActive(true);

        ItemPickupBannerEntry entry = entryObj.GetComponent<ItemPickupBannerEntry>();

        if (entry != null)
        {
            entry.Setup(item, quantity);
            activeEntries.Add(entry);
        }
    }

    public void RemoveEntry(ItemPickupBannerEntry entry)
    {
        if (activeEntries.Contains(entry))
        {
            activeEntries.Remove(entry);
        }
    }
}
