using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuntimeInventoryDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DebugInventoryIcons();
        }
    }
    
    void DebugInventoryIcons()
    {
        Debug.Log("========== INVENTORY ICON DEBUG ==========");
        
        Transform content = GameObject.Find("/Canvas/InventoryPanel/ItemsScrollView/Viewport/Content")?.transform;
        if (content == null)
        {
            Debug.LogError("Content not found!");
            return;
        }
        
        Debug.Log("Children in inventory: " + content.childCount);
        
        for (int i = 0; i < content.childCount; i++)
        {
            Transform slot = content.GetChild(i);
            Debug.Log("\n--- SLOT " + i + ": " + slot.name + " ---");
            Debug.Log("Active: " + slot.gameObject.activeSelf);
            Debug.Log("Layer: " + LayerMask.LayerToName(slot.gameObject.layer));
            
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            Debug.Log("Slot Size: " + slotRect.sizeDelta);
            Debug.Log("Slot Position: " + slotRect.anchoredPosition);
            
            Image slotBg = slot.GetComponent<Image>();
            if (slotBg != null)
            {
                Debug.Log("BG Image enabled: " + slotBg.enabled + ", color: " + slotBg.color);
            }
            
            Transform icon = slot.Find("Icon");
            if (icon != null)
            {
                Debug.Log("ICON found!");
                Debug.Log("  Active: " + icon.gameObject.activeSelf);
                Debug.Log("  Layer: " + LayerMask.LayerToName(icon.gameObject.layer));
                
                RectTransform iconRect = icon.GetComponent<RectTransform>();
                Debug.Log("  Size: " + iconRect.sizeDelta);
                Debug.Log("  Position: " + iconRect.anchoredPosition);
                Debug.Log("  Local Scale: " + iconRect.localScale);
                
                Image iconImg = icon.GetComponent<Image>();
                if (iconImg != null)
                {
                    Debug.Log("  Image enabled: " + iconImg.enabled);
                    Debug.Log("  Sprite: " + (iconImg.sprite != null ? iconImg.sprite.name : "NULL"));
                    Debug.Log("  Color: " + iconImg.color);
                    Debug.Log("  Alpha: " + iconImg.color.a);
                    
                    CanvasRenderer cr = icon.GetComponent<CanvasRenderer>();
                    if (cr != null)
                    {
                        Debug.Log("  CanvasRenderer cull: " + cr.cull);
                        Debug.Log("  CanvasRenderer alpha: " + cr.GetAlpha());
                    }
                }
                else
                {
                    Debug.LogError("  NO IMAGE COMPONENT!");
                }
            }
            else
            {
                Debug.LogError("NO ICON CHILD!");
            }
        }
        
        Canvas canvas = content.GetComponentInParent<Canvas>();
        Debug.Log("\n--- CANVAS INFO ---");
        Debug.Log("Render Mode: " + canvas.renderMode);
        Debug.Log("Sorting Layer: " + canvas.sortingLayerName);
        Debug.Log("Order in Layer: " + canvas.sortingOrder);
        
        Camera cam = canvas.worldCamera;
        if (cam != null)
        {
            Debug.Log("Camera: " + cam.name);
            Debug.Log("Camera culling mask: " + cam.cullingMask);
        }
        else
        {
            Debug.Log("No camera (Screen Space Overlay)");
        }
        
        Debug.Log("========================================");
    }
}
