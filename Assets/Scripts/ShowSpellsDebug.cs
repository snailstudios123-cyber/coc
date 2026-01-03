using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowSpellsDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("=== F1 PRESSED: SPELL LIST DEBUG ===");
            
            GameObject container = GameObject.Find("SpellsListContainer");
            if (container == null)
            {
                Debug.LogError("Container not found!");
                return;
            }

            Debug.Log($"Container active: {container.activeInHierarchy}");
            Debug.Log($"Container child count: {container.transform.childCount}");

            RectTransform containerRT = container.GetComponent<RectTransform>();
            Debug.Log($"Container rect: {containerRT.rect.width} x {containerRT.rect.height}");
            Debug.Log($"Container position: {containerRT.anchoredPosition}");

            VerticalLayoutGroup vlg = container.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                Debug.Log($"VLG childControlHeight: {vlg.childControlHeight}");
                Debug.Log($"VLG childControlWidth: {vlg.childControlWidth}");
            }

            for (int i = 0; i < container.transform.childCount; i++)
            {
                Transform child = container.transform.GetChild(i);
                RectTransform rt = child.GetComponent<RectTransform>();
                
                Debug.Log($"\n--- Child {i}: {child.name} ---");
                Debug.Log($"  Active: {child.gameObject.activeSelf} / {child.gameObject.activeInHierarchy}");
                Debug.Log($"  Rect: {rt.rect.width} x {rt.rect.height}");
                Debug.Log($"  Position: {rt.anchoredPosition}");
                Debug.Log($"  Local position: {rt.localPosition}");
                
                LayoutElement le = child.GetComponent<LayoutElement>();
                if (le != null)
                {
                    Debug.Log($"  LayoutElement minHeight: {le.minHeight}, preferredHeight: {le.preferredHeight}");
                }

                TextMeshProUGUI[] texts = child.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var t in texts)
                {
                    Debug.Log($"  Text '{t.name}': '{t.text}' (active: {t.gameObject.activeInHierarchy}, enabled: {t.enabled})");
                }

                Image[] images = child.GetComponentsInChildren<Image>();
                foreach (var img in images)
                {
                    string spriteName = "null";
                    if (img.sprite != null)
                    {
                        spriteName = img.sprite.name;
                    }
                    Debug.Log($"  Image '{img.name}': sprite={spriteName}, enabled={img.enabled}, color={img.color}");
                }
            }

            GameObject viewport = GameObject.Find("Viewport");
            if (viewport != null)
            {
                RectTransform vrt = viewport.GetComponent<RectTransform>();
                Debug.Log($"\nViewport rect: {vrt.rect.width} x {vrt.rect.height}");
                
                Mask mask = viewport.GetComponent<Mask>();
                RectMask2D mask2d = viewport.GetComponent<RectMask2D>();
                Debug.Log($"Viewport has Mask: {mask != null}, RectMask2D: {mask2d != null}");
            }

            GameObject spellsPanel = GameObject.Find("SpellsMenuPanel");
            if (spellsPanel != null)
            {
                Debug.Log($"\nSpellsMenuPanel active: {spellsPanel.activeInHierarchy}");
            }

            Canvas.ForceUpdateCanvases();
            Debug.Log("\nForced canvas update!");
            
            Debug.Log("=== END DEBUG ===");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("=== F2: FORCE SHOW SPELLS ===");
            
            GameObject container = GameObject.Find("SpellsListContainer");
            if (container == null)
            {
                Debug.LogError("Container not found!");
                return;
            }

            VerticalLayoutGroup vlg = container.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                vlg.enabled = false;
                Debug.Log("Disabled VerticalLayoutGroup");
            }

            float yPos = 0;
            for (int i = 0; i < container.transform.childCount; i++)
            {
                Transform child = container.transform.GetChild(i);
                RectTransform rt = child.GetComponent<RectTransform>();
                
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(0.5f, 1);
                rt.anchoredPosition = new Vector2(0, yPos);
                rt.sizeDelta = new Vector2(0, 60);
                
                child.gameObject.SetActive(true);
                
                Debug.Log($"Positioned child {i} at Y={yPos}, size=60");
                
                yPos -= 70;
            }

            RectTransform containerRT = container.GetComponent<RectTransform>();
            float neededHeight = Mathf.Abs(yPos) + 10;
            containerRT.sizeDelta = new Vector2(containerRT.sizeDelta.x, neededHeight);
            
            ContentSizeFitter csf = container.GetComponent<ContentSizeFitter>();
            if (csf != null)
            {
                csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                Debug.Log("Set ContentSizeFitter to Unconstrained");
            }
            
            Canvas.ForceUpdateCanvases();
            
            Debug.Log($"Force positioned {container.transform.childCount} children");
            Debug.Log($"Container height set to: {containerRT.sizeDelta.y} (needed: {neededHeight})");
            Debug.Log("=== DONE - CHECK IF VISIBLE NOW ===");
        }
    }
}
