using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleItemSlotUI : MonoBehaviour
{
    private Image iconImage;
    private TextMeshProUGUI nameText;
    
    void Awake()
    {
        iconImage = transform.Find("Icon")?.GetComponent<Image>();
        nameText = transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
    }
    
    public void SetItem(ItemData data, int quantity)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        
        if (iconImage != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
            iconImage.enabled = true;
        }
        
        if (nameText != null)
        {
            nameText.text = quantity > 1 ? $"{data.itemName} x{quantity}" : data.itemName;
        }
    }
}
