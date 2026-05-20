using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopListingManager : MonoBehaviour
{
    const float LISTING_VIEWPORT_HEIGHT_FALLBACK = 400f;
    const float SCROLL_SENSITIVITY = 80f;
    const float DRAG_CLICK_THRESHOLD = 6f;

    //The shop Listing entry prefab to instantiate
    public GameObject shopListing;
    //The transform of the grid to instantiate the entries on
    public Transform listingGrid;

    //Variables to keep track of what the player is trying to purchase (selection)
    ItemData itemToBuy;
    int quantity;

    [Header("Confirmation Screen")]
    public GameObject confirmationScreen;
    public Image confirmationThumbnail;
    public Text confirmationPrompt;
    public Text quantityText;
    public Text costCalculationText;
    public Button purchaseButton;
    public GameObject ListingGrid;

    RectTransform listingRect;
    RectTransform listingViewportRect;
    float listingTopY;
    float listingViewportHeight = LISTING_VIEWPORT_HEIGHT_FALLBACK;
    float maxScrollOffset;
    bool isPointerDownOnListing;
    bool isDraggingListing;
    bool suppressNextListingClick;
    Vector2 dragStartPointerPosition;
    Vector2 lastPointerPosition;

    void Awake()
    {
        InitializeScrollableListingGrid();
        CacheConfirmationThumbnail();
    }

    void Update()
    {
        if (listingRect == null || ListingGrid == null || !ListingGrid.activeInHierarchy)
        {
            return;
        }

        HandleMouseDragScroll();
        HandleMouseWheelScroll();
    }

    void HandleMouseWheelScroll()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scrollDelta, 0f))
        {
            return;
        }

        if (!IsPointerOverListingViewport())
        {
            return;
        }

        float targetY = listingRect.anchoredPosition.y - scrollDelta * SCROLL_SENSITIVITY;
        SetListingScrollPosition(targetY);
    }

    void HandleMouseDragScroll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPointerDownOnListing = IsPointerOverListingViewport();
            isDraggingListing = false;
            suppressNextListingClick = false;
            dragStartPointerPosition = Input.mousePosition;
            lastPointerPosition = dragStartPointerPosition;
        }

        if (Input.GetMouseButton(0) && isPointerDownOnListing)
        {
            Vector2 currentPointerPosition = Input.mousePosition;
            Vector2 totalDrag = currentPointerPosition - dragStartPointerPosition;
            Vector2 frameDrag = currentPointerPosition - lastPointerPosition;

            if (!isDraggingListing && totalDrag.magnitude >= DRAG_CLICK_THRESHOLD)
            {
                isDraggingListing = true;
                suppressNextListingClick = true;
            }

            if (isDraggingListing)
            {
                SetListingScrollPosition(listingRect.anchoredPosition.y + frameDrag.y);
            }

            lastPointerPosition = currentPointerPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPointerDownOnListing = false;
            isDraggingListing = false;
        }
    }

    public bool ConsumeListingClickSuppression()
    {
        if (!suppressNextListingClick)
        {
            return false;
        }

        suppressNextListingClick = false;
        return true;
    }

    public void RenderShop(List<ItemData> shopItems)
    {
        InitializeScrollableListingGrid();

        if (shopItems == null)
        {
            shopItems = new List<ItemData>();
        }

        if (ListingGrid != null)
        {
            ListingGrid.SetActive(true);
        }

        confirmationScreen.SetActive(false);
        if (listingGrid == null)
        {
            return;
        }

        //Reset the listings if there was a previous one
        if(listingGrid.childCount > 0)
        {
            foreach(Transform child in listingGrid)
            {
                Destroy(child.gameObject);
            }
        }

        //Create a new listing for every item
        foreach(ItemData shopItem in shopItems)
        {
            //Instantiate a shop listing prefab for the item
            GameObject listingGameObject = Instantiate(shopListing, listingGrid);

            //Assign it the shop item and display listing
            listingGameObject.GetComponent<ShopListing>().Display(shopItem);
        }

        ResizeListingContent(shopItems.Count);
        ResetListingScroll();
    }

    public void OpenConfirmationScreen(ItemData item)
    {
        itemToBuy = item;
        quantity = 1;
        RenderConfirmationScreen();
    }

    public void RenderConfirmationScreen()
    {
        confirmationScreen.SetActive(true);

        if (ListingGrid != null)
        {
            ListingGrid.SetActive(false);
        }

        confirmationPrompt.text = $"Beli {itemToBuy.name}?";
        RenderConfirmationThumbnail();

        quantityText.text = "x" + quantity;

        int cost = itemToBuy.cost * quantity;

        int playerMoneyLeft = PlayerStats.Money - cost;

        //Stop the player from purchasing the item if the player does not have enough money
        if(playerMoneyLeft < 0)
        {
            costCalculationText.text = "Uang Tidak Cukup!";
            purchaseButton.interactable = false;
            return;
        }

        purchaseButton.interactable = true; 

        costCalculationText.text = $"{PlayerStats.Money} > {playerMoneyLeft} ";
    }

    void RenderConfirmationThumbnail()
    {
        CacheConfirmationThumbnail();

        if (confirmationThumbnail == null)
        {
            return;
        }

        Sprite thumbnail = itemToBuy != null ? itemToBuy.thumbnail : null;
        confirmationThumbnail.sprite = thumbnail;
        confirmationThumbnail.preserveAspect = true;
        confirmationThumbnail.enabled = thumbnail != null;
    }

    void CacheConfirmationThumbnail()
    {
        if (confirmationThumbnail != null || confirmationScreen == null)
        {
            return;
        }

        Image[] images = confirmationScreen.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            string imageName = image.gameObject.name.ToLowerInvariant();
            if (imageName.Contains("thumbnail") || imageName.Contains("item icon") || imageName.Contains("itemimage") || imageName.Contains("item image"))
            {
                confirmationThumbnail = image;
                return;
            }
        }
    }

    public void AddQuantity()
    {
        quantity++;
        RenderConfirmationScreen();
    }

    public void SubstractQuantity()
    {
        if(quantity > 1)
        {
            quantity--;
        }
        RenderConfirmationScreen();
    }
    
    //Purchase the item and close the confirmation screen
    public void ConfirmPurchase()
    {
        Shop.Purchase(itemToBuy, quantity);
        confirmationScreen.SetActive(false);
    }

    public void CancelPurchase()
    {
        confirmationScreen.SetActive(false);
        if (ListingGrid != null)
        {
            ListingGrid.SetActive(true);
        }
    }

    void InitializeScrollableListingGrid()
    {
        if (listingGrid == null || listingRect != null)
        {
            return;
        }

        listingRect = listingGrid as RectTransform;
        if (listingRect == null)
        {
            return;
        }

        RectTransform parentRect = listingRect.parent as RectTransform;
        if (parentRect == null)
        {
            return;
        }

        listingViewportRect = parentRect;
        listingViewportHeight = Mathf.Max(listingRect.rect.height, LISTING_VIEWPORT_HEIGHT_FALLBACK);
        listingTopY = listingRect.anchoredPosition.y + listingRect.rect.height * (1f - listingRect.pivot.y);
        listingRect.pivot = new Vector2(listingRect.pivot.x, 1f);
        listingRect.anchoredPosition = new Vector2(listingRect.anchoredPosition.x, listingTopY);

        if (parentRect.GetComponent<RectMask2D>() == null)
        {
            parentRect.gameObject.AddComponent<RectMask2D>();
        }

        GridLayoutGroup gridLayout = listingRect.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.childAlignment = TextAnchor.UpperCenter;
        }
    }

    void ResizeListingContent(int itemCount)
    {
        if (listingRect == null)
        {
            return;
        }

        GridLayoutGroup gridLayout = listingRect.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            return;
        }

        int columns = GetColumnCount(gridLayout);
        int rows = Mathf.CeilToInt(itemCount / (float)columns);
        float contentHeight = gridLayout.padding.top + gridLayout.padding.bottom;

        if (rows > 0)
        {
            contentHeight += rows * gridLayout.cellSize.y;
            contentHeight += (rows - 1) * gridLayout.spacing.y;
        }

        float finalHeight = Mathf.Max(contentHeight, listingViewportHeight);
        maxScrollOffset = Mathf.Max(0f, finalHeight - listingViewportHeight);
        listingRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalHeight);
        LayoutRebuilder.ForceRebuildLayoutImmediate(listingRect);
    }

    void ResetListingScroll()
    {
        if (listingRect == null)
        {
            return;
        }

        listingRect.anchoredPosition = new Vector2(listingRect.anchoredPosition.x, listingTopY);
    }

    void SetListingScrollPosition(float targetY)
    {
        targetY = Mathf.Clamp(targetY, listingTopY, listingTopY + maxScrollOffset);
        listingRect.anchoredPosition = new Vector2(listingRect.anchoredPosition.x, targetY);
    }

    bool IsPointerOverListingViewport()
    {
        if (listingViewportRect == null)
        {
            return false;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(listingViewportRect, Input.mousePosition);
    }

    int GetColumnCount(GridLayoutGroup gridLayout)
    {
        if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            return Mathf.Max(1, gridLayout.constraintCount);
        }

        if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            return Mathf.Max(1, Mathf.CeilToInt(gridLayout.transform.childCount / (float)gridLayout.constraintCount));
        }

        float contentWidth = listingRect != null ? listingRect.rect.width : gridLayout.cellSize.x;
        float cellWidth = gridLayout.cellSize.x + gridLayout.spacing.x;
        return Mathf.Max(1, Mathf.FloorToInt((contentWidth + gridLayout.spacing.x) / cellWidth));
    }
}
