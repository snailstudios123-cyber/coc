using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button selectButton;

    private ItemData currentItem;
    private int currentQuantity;
    private InventoryUI inventoryUI;
    private Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    private Color highlightColor = Color.white;

    public void Initialize(ItemData item, int quantity, InventoryUI ui)
    {
        currentItem = item;
        currentQuantity = quantity;
        inventoryUI = ui;

        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }

        if (quantityText != null)
        {
            if (quantity > 1)
            {
                quantityText.text = $"x{quantity}";
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }

        if (itemIcon != null && item.icon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            itemIcon.color = normalColor;
        }
        else if (itemIcon != null)
        {
            itemIcon.enabled = false;
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        Debug.Log($"[ItemSlotUI] Item clicked: {(currentItem != null ? currentItem.itemName : "NULL")}");
        if (inventoryUI != null && currentItem != null)
        {
            Debug.Log($"[ItemSlotUI] Calling DisplayItemDetails for {currentItem.itemName}");
            inventoryUI.DisplayItemDetails(currentItem, currentQuantity);
        }
        else
        {
            Debug.LogError($"[ItemSlotUI] Cannot display details - inventoryUI: {inventoryUI != null}, currentItem: {currentItem != null}");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemIcon != null)
        {
            itemIcon.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemIcon != null)
        {
            itemIcon.color = normalColor;
        }
    }
}
