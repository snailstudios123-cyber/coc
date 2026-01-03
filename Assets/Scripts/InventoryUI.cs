using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory Menu Panel")]
    [SerializeField] private GameObject inventoryMenuPanel;
    [SerializeField] private GameObject itemDetailPanel;

    [Header("Item List")]
    [SerializeField] private Transform itemsListContainer;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Button backButton;

    [Header("Item Detail Panel")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;

    [Header("Filter Buttons")]
    [SerializeField] private Button filterAllButton;
    [SerializeField] private Button filterConsumableButton;
    [SerializeField] private Button filterKeyItemButton;
    [SerializeField] private Button filterEquipmentButton;

    private ItemData selectedItem;
    private int selectedItemQuantity;
    private List<GameObject> spawnedItemSlots = new List<GameObject>();
    private ItemType currentFilter = ItemType.Consumable;
    private bool showAllItems = true;

    private void Start()
    {
        SetupButtons();
        
        gameObject.SetActive(false);
        
        if (itemDetailPanel != null)
        {
            itemDetailPanel.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToPauseMenu);
        }

        if (useButton != null)
        {
            useButton.onClick.AddListener(UseSelectedItem);
        }

        if (dropButton != null)
        {
            dropButton.onClick.AddListener(DropSelectedItem);
        }

        if (filterAllButton != null)
        {
            filterAllButton.onClick.AddListener(() => SetFilter(true));
        }

        if (filterConsumableButton != null)
        {
            filterConsumableButton.onClick.AddListener(() => SetFilter(false, ItemType.Consumable));
        }

        if (filterKeyItemButton != null)
        {
            filterKeyItemButton.onClick.AddListener(() => SetFilter(false, ItemType.KeyItem));
        }

        if (filterEquipmentButton != null)
        {
            filterEquipmentButton.onClick.AddListener(() => SetFilter(false, ItemType.Equipment));
        }
    }

    private void SetFilter(bool all, ItemType type = ItemType.Consumable)
    {
        showAllItems = all;
        currentFilter = type;
        PopulateItemsList();
    }

    public void OpenInventory()
    {
        Debug.Log("[InventoryUI] OpenInventory called!");
        
        gameObject.SetActive(true);
        Debug.Log("[InventoryUI] Set InventoryPanel to active");

        if (itemDetailPanel != null)
        {
            itemDetailPanel.SetActive(false);
        }

        FixItemListVisibility();
        PopulateItemsList();
        
        Debug.Log("[InventoryUI] OpenInventory complete");
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
        
        if (itemDetailPanel != null)
        {
            itemDetailPanel.SetActive(false);
        }

        ClearItemsList();
    }

    private void FixItemListVisibility()
    {
        if (itemsListContainer == null)
        {
            return;
        }

        VerticalLayoutGroup vlg = itemsListContainer.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.childControlHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;
        }

        ContentSizeFitter csf = itemsListContainer.GetComponent<ContentSizeFitter>();
        if (csf != null)
        {
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        Transform viewport = itemsListContainer.parent;
        if (viewport != null)
        {
            Mask mask = viewport.GetComponent<Mask>();
            if (mask != null)
            {
                mask.enabled = false;
            }
        }
    }

    private void PopulateItemsList()
    {
        ClearItemsList();

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[InventoryUI] InventoryManager.Instance is null! Cannot populate inventory list.");
            return;
        }

        if (itemSlotPrefab == null)
        {
            Debug.LogError("[InventoryUI] itemSlotPrefab is null! Assign the ItemSlot prefab.");
            return;
        }

        if (itemsListContainer == null)
        {
            Debug.LogError("[InventoryUI] itemsListContainer is null! Cannot populate inventory list.");
            return;
        }

        Dictionary<ItemData, int> inventory = InventoryManager.Instance.GetInventory();

        if (inventory == null || inventory.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<ItemData, int> entry in inventory)
        {
            ItemData item = entry.Key;
            int quantity = entry.Value;

            if (item == null)
            {
                Debug.LogWarning("[InventoryUI] Skipping null item in inventory");
                continue;
            }

            if (!showAllItems && item.itemType != currentFilter)
            {
                continue;
            }

            GameObject itemSlot = Instantiate(itemSlotPrefab, itemsListContainer);
            ItemSlotUI slotUI = itemSlot.GetComponent<ItemSlotUI>();

            if (slotUI != null)
            {
                slotUI.Initialize(item, quantity, this);
            }

            spawnedItemSlots.Add(itemSlot);
        }
    }

    private void ClearItemsList()
    {
        foreach (GameObject slot in spawnedItemSlots)
        {
            Destroy(slot);
        }
        spawnedItemSlots.Clear();
    }

    public void DisplayItemDetails(ItemData item, int quantity)
    {
        Debug.Log($"[InventoryUI] DisplayItemDetails called for {item.itemName}");
        selectedItem = item;
        selectedItemQuantity = quantity;

        if (itemDetailPanel != null)
        {
            itemDetailPanel.SetActive(true);
            Debug.Log("[InventoryUI] Item detail panel activated");
        }
        else
        {
            Debug.LogError("[InventoryUI] itemDetailPanel is NULL!");
        }

        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = item.description;
        }

        if (itemTypeText != null)
        {
            itemTypeText.text = $"Type: {item.itemType}";
        }

        if (quantityText != null)
        {
            quantityText.text = $"Quantity: {quantity}";
        }

        if (itemIconImage != null && item.icon != null)
        {
            itemIconImage.sprite = item.icon;
            itemIconImage.enabled = true;
            itemIconImage.color = Color.white;
        }
        else if (itemIconImage != null)
        {
            itemIconImage.enabled = false;
        }

        UpdateActionButtons();
    }

    private void UpdateActionButtons()
    {
        if (selectedItem == null)
        {
            return;
        }

        if (useButton != null)
        {
            useButton.gameObject.SetActive(selectedItem.isConsumable);
        }

        if (dropButton != null)
        {
            dropButton.gameObject.SetActive(!selectedItem.isKeyItem);
        }
    }

    private void UseSelectedItem()
    {
        if (InventoryManager.Instance == null || selectedItem == null)
        {
            return;
        }

        if (!selectedItem.isConsumable)
        {
            Debug.Log("This item cannot be used!");
            return;
        }

        bool success = InventoryManager.Instance.UseItem(selectedItem);

        if (success)
        {
            PopulateItemsList();

            if (!InventoryManager.Instance.HasItem(selectedItem))
            {
                if (itemDetailPanel != null)
                {
                    itemDetailPanel.SetActive(false);
                }
            }
            else
            {
                DisplayItemDetails(selectedItem, InventoryManager.Instance.GetItemCount(selectedItem));
            }
        }
    }

    private void DropSelectedItem()
    {
        if (InventoryManager.Instance == null || selectedItem == null)
        {
            return;
        }

        if (selectedItem.isKeyItem)
        {
            Debug.Log("Cannot drop key items!");
            return;
        }

        bool success = InventoryManager.Instance.RemoveItem(selectedItem, 1);

        if (success)
        {
            PopulateItemsList();

            if (!InventoryManager.Instance.HasItem(selectedItem))
            {
                if (itemDetailPanel != null)
                {
                    itemDetailPanel.SetActive(false);
                }
            }
            else
            {
                DisplayItemDetails(selectedItem, InventoryManager.Instance.GetItemCount(selectedItem));
            }
        }
    }

    private void BackToPauseMenu()
    {
        CloseInventory();

        if (PauseMenuManager.Instance != null)
        {
            PauseMenuManager.Instance.ShowPauseMenu();
        }
    }
}
