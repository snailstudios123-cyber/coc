using UnityEngine;
using UnityEngine.UI;

public class CleanupSpellsUI : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.Log("=== F4: CLEANUP - RESTORE NORMAL APPEARANCE ===");
            
            GameObject container = GameObject.Find("SpellsListContainer");
            if (container != null)
            {
                for (int i = 0; i < container.transform.childCount; i++)
                {
                    Transform child = container.transform.GetChild(i);
                    RectTransform rt = child.GetComponent<RectTransform>();
                    rt.localScale = Vector3.one;
                    
                    CanvasGroup cg = child.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        Destroy(cg);
                    }
                }
                Debug.Log("Restored spell slots to normal scale");
            }

            GameObject listPanel = GameObject.Find("SpellsListPanel");
            if (listPanel != null)
            {
                Image img = listPanel.GetComponent<Image>();
                if (img != null)
                {
                    img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                    Debug.Log("Restored SpellsListPanel background");
                }
            }

            GameObject viewport = GameObject.Find("Viewport");
            if (viewport != null)
            {
                Image img = viewport.GetComponent<Image>();
                if (img != null)
                {
                    img.color = new Color(1f, 1f, 1f, 0.01f);
                    Debug.Log("Restored Viewport background");
                }
                
                Mask mask = viewport.GetComponent<Mask>();
                if (mask != null)
                {
                    mask.enabled = true;
                    Debug.Log("Re-enabled Viewport mask");
                }
            }

            Canvas.ForceUpdateCanvases();
            
            Debug.Log("=== CLEANUP COMPLETE - NORMAL APPEARANCE RESTORED ===");
        }
    }
}
