using UnityEngine;
using UnityEngine.UI;

public class MakeSpellsVisible : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("=== F3: MAKE SPELLS ACTUALLY VISIBLE ===");
            
            GameObject container = GameObject.Find("SpellsListContainer");
            if (container == null)
            {
                Debug.LogError("Container not found!");
                return;
            }

            for (int i = 0; i < container.transform.childCount; i++)
            {
                Transform child = container.transform.GetChild(i);
                
                CanvasGroup cg = child.GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = child.gameObject.AddComponent<CanvasGroup>();
                }
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
                
                child.gameObject.SetActive(true);
                
                RectTransform rt = child.GetComponent<RectTransform>();
                rt.localScale = Vector3.one * 2f;
                
                Debug.Log($"Child {i} scaled to 2x and alpha = 1");
            }

            GameObject listPanel = GameObject.Find("SpellsListPanel");
            if (listPanel != null)
            {
                Image img = listPanel.GetComponent<Image>();
                if (img != null)
                {
                    img.color = Color.yellow;
                    Debug.Log("Set SpellsListPanel background to YELLOW");
                }
            }

            GameObject viewport = GameObject.Find("Viewport");
            if (viewport != null)
            {
                Image img = viewport.GetComponent<Image>();
                if (img != null)
                {
                    img.color = Color.red;
                    Debug.Log("Set Viewport background to RED");
                }
                
                Mask mask = viewport.GetComponent<Mask>();
                if (mask != null)
                {
                    mask.enabled = false;
                    Debug.Log("DISABLED VIEWPORT MASK!");
                }
            }

            RectTransform containerRT = container.GetComponent<RectTransform>();
            containerRT.localScale = Vector3.one;
            containerRT.anchoredPosition = Vector2.zero;
            
            Canvas.ForceUpdateCanvases();
            
            Debug.Log("=== EVERYTHING SCALED 2X, MASK DISABLED ===");
            Debug.Log("If you STILL can't see them, the problem is elsewhere!");
        }
    }
}
