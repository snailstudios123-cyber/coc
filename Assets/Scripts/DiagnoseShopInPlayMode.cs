using UnityEngine;
using UnityEngine.UI;

public class DiagnoseShopInPlayMode : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("=== SHOP DIAGNOSIS (F9) ===");
            
            Transform content = GameObject.Find("Canvas/ShopPanel/ItemsList/Viewport/Content")?.transform;
            if (content != null)
            {
                Debug.Log($"Content found. Active: {content.gameObject.activeSelf}");
                Debug.Log($"Content child count: {content.childCount}");
                
                RectTransform contentRect = content.GetComponent<RectTransform>();
                Debug.Log($"Content RectTransform:");
                Debug.Log($"  Position: {contentRect.anchoredPosition}");
                Debug.Log($"  Size: {contentRect.sizeDelta}");
                Debug.Log($"  Anchors: Min={contentRect.anchorMin}, Max={contentRect.anchorMax}");
                Debug.Log($"  Pivot: {contentRect.pivot}");
                Debug.Log($"  LocalScale: {contentRect.localScale}");
                
                for (int i = 0; i < content.childCount; i++)
                {
                    Transform child = content.GetChild(i);
                    RectTransform childRect = child.GetComponent<RectTransform>();
                    Debug.Log($"\n  Child {i}: {child.name}");
                    Debug.Log($"    Active: {child.gameObject.activeSelf}");
                    Debug.Log($"    Position: {childRect.anchoredPosition}");
                    Debug.Log($"    Size: {childRect.sizeDelta}");
                    Debug.Log($"    Scale: {childRect.localScale}");
                    
                    Image[] images = child.GetComponentsInChildren<Image>(true);
                    Debug.Log($"    Images found: {images.Length}");
                    foreach (Image img in images)
                    {
                        Debug.Log($"      - {img.gameObject.name}: enabled={img.enabled}, sprite={img.sprite?.name ?? "NULL"}, color={img.color}");
                    }
                }
            }
            else
            {
                Debug.LogError("Content not found!");
            }
            
            Transform viewport = GameObject.Find("Canvas/ShopPanel/ItemsList/Viewport")?.transform;
            if (viewport != null)
            {
                RectTransform vpRect = viewport.GetComponent<RectTransform>();
                Debug.Log($"\nViewport RectTransform:");
                Debug.Log($"  Size: {vpRect.sizeDelta}");
                Debug.Log($"  Rect: {vpRect.rect}");
                
                Mask mask = viewport.GetComponent<Mask>();
                if (mask != null)
                {
                    Debug.Log($"  Mask: enabled={mask.enabled}, showGraphic={mask.showMaskGraphic}");
                }
            }
            
            Transform itemsList = GameObject.Find("Canvas/ShopPanel/ItemsList")?.transform;
            if (itemsList != null)
            {
                RectTransform listRect = itemsList.GetComponent<RectTransform>();
                Debug.Log($"\nItemsList RectTransform:");
                Debug.Log($"  Size: {listRect.sizeDelta}");
                Debug.Log($"  Rect: {listRect.rect}");
                Debug.Log($"  Actual width: {listRect.rect.width}, height: {listRect.rect.height}");
            }
        }
    }
}
