using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupBannerEntry : MonoBehaviour
{
    [Header("UI References")]
    public Image thumbnailImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;

    [Header("Animation Settings")]
    public float slideDuration = 0.3f;
    public float displayDuration = 3f;
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ItemData currentItem;
    private int currentQuantity;
    private Coroutine lifetimeCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Hide thumbnail on start so no white placeholder shows
        if (thumbnailImage != null)
            thumbnailImage.gameObject.SetActive(false);
    }

    public void Setup(ItemData item, int quantity)
    {
        currentItem = item;
        currentQuantity = quantity;

        // Set thumbnail
        if (thumbnailImage != null)
        {
            if (item.thumbnail != null)
            {
                thumbnailImage.sprite = item.thumbnail;
                thumbnailImage.gameObject.SetActive(true);
            }
            else
            {
                thumbnailImage.gameObject.SetActive(false);
            }
        }

        // Set item name
        if (itemNameText != null)
        {
            itemNameText.text = item.name;
        }

        // Set quantity
        UpdateQuantityDisplay();

        // Start the slide-in and auto-dismiss
        if (lifetimeCoroutine != null)
            StopCoroutine(lifetimeCoroutine);

        lifetimeCoroutine = StartCoroutine(BannerLifetime());
    }

    public void AddQuantity(int amount)
    {
        currentQuantity += amount;
        UpdateQuantityDisplay();

        // Reset the lifetime timer
        if (lifetimeCoroutine != null)
            StopCoroutine(lifetimeCoroutine);

        lifetimeCoroutine = StartCoroutine(BannerLifetime());
    }

    public ItemData GetItemData()
    {
        return currentItem;
    }

    private void UpdateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = "x" + currentQuantity;
        }
    }

    private IEnumerator BannerLifetime()
    {
        // Ensure visible immediately
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;

        // Stay visible
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        canvasGroup.alpha = 0f;

        // Notify the banner manager to remove this entry
        ItemPickupBanner.Instance?.RemoveEntry(this);

        Destroy(gameObject);
    }
}
