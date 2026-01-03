using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DiagnosePauseMenuButtons : EditorWindow
{
    [MenuItem("Tools/🔍 Diagnose Pause Menu Buttons")]
    public static void DiagnoseButtons()
    {
        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("PAUSE MENU BUTTON DIAGNOSTICS");
        Debug.Log("═══════════════════════════════════════════════════════");

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found in scene!");
            return;
        }
        Debug.Log($"✓ Canvas found: {canvas.name}");

        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("❌ Canvas missing GraphicRaycaster component!");
        }
        else
        {
            Debug.Log($"✓ GraphicRaycaster found, enabled: {raycaster.enabled}");
            Debug.Log($"  - Blocking Objects: {raycaster.blockingObjects}");
            Debug.Log($"  - Ignore Reversed Graphics: {raycaster.ignoreReversedGraphics}");
        }

        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ No EventSystem found in scene!");
        }
        else
        {
            Debug.Log($"✓ EventSystem found, active: {eventSystem.gameObject.activeInHierarchy}");
            Debug.Log($"  - GameObject: {eventSystem.name}");
            
            StandaloneInputModule inputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (inputModule == null)
            {
                Debug.LogWarning("⚠ EventSystem missing StandaloneInputModule!");
            }
            else
            {
                Debug.Log($"✓ StandaloneInputModule found, enabled: {inputModule.enabled}");
            }
        }

        GameObject pausePanel = GameObject.Find("PauseMenuPanel");
        if (pausePanel == null)
        {
            Debug.LogError("❌ PauseMenuPanel not found!");
            return;
        }
        Debug.Log($"✓ PauseMenuPanel found, active: {pausePanel.activeSelf}");

        Transform content = pausePanel.transform.Find("Content");
        if (content == null)
        {
            Debug.LogError("❌ Content panel not found!");
            return;
        }

        string[] buttonNames = { "ResumeButton", "SpellsButton", "InventoryButton", "OptionsButton", "QuitButton" };
        
        Debug.Log("\n--- BUTTON DETAILS ---");
        foreach (string btnName in buttonNames)
        {
            Transform btnTransform = content.Find(btnName);
            if (btnTransform == null)
            {
                Debug.LogError($"❌ {btnName} not found!");
                continue;
            }

            Button btn = btnTransform.GetComponent<Button>();
            Image img = btnTransform.GetComponent<Image>();

            Debug.Log($"\n{btnName}:");
            Debug.Log($"  - GameObject active: {btnTransform.gameObject.activeSelf}");
            Debug.Log($"  - Layer: {LayerMask.LayerToName(btnTransform.gameObject.layer)}");
            
            if (btn == null)
            {
                Debug.LogError($"  ❌ Missing Button component!");
            }
            else
            {
                Debug.Log($"  ✓ Button component found");
                Debug.Log($"    - Interactable: {btn.interactable}");
                Debug.Log($"    - Transition: {btn.transition}");
                Debug.Log($"    - Target Graphic: {(btn.targetGraphic != null ? btn.targetGraphic.name : "NULL")}");
                Debug.Log($"    - OnClick listeners: {btn.onClick.GetPersistentEventCount()}");
            }

            if (img == null)
            {
                Debug.LogError($"  ❌ Missing Image component!");
            }
            else
            {
                Debug.Log($"  ✓ Image component found");
                Debug.Log($"    - Enabled: {img.enabled}");
                Debug.Log($"    - Raycast Target: {img.raycastTarget}");
                Debug.Log($"    - Color: {img.color}");
            }
        }

        PauseMenuManager manager = FindObjectOfType<PauseMenuManager>();
        if (manager == null)
        {
            Debug.LogError("\n❌ PauseMenuManager not found in scene!");
        }
        else
        {
            Debug.Log($"\n✓ PauseMenuManager found on: {manager.gameObject.name}");
            
            SerializedObject so = new SerializedObject(manager);
            Debug.Log("\n--- MANAGER BUTTON REFERENCES ---");
            Debug.Log($"  Resume Button: {(so.FindProperty("resumeButton").objectReferenceValue != null ? "✓ Assigned" : "❌ NULL")}");
            Debug.Log($"  Spells Button: {(so.FindProperty("spellsButton").objectReferenceValue != null ? "✓ Assigned" : "❌ NULL")}");
            Debug.Log($"  Inventory Button: {(so.FindProperty("inventoryButton").objectReferenceValue != null ? "✓ Assigned" : "❌ NULL")}");
            Debug.Log($"  Options Button: {(so.FindProperty("optionsButton").objectReferenceValue != null ? "✓ Assigned" : "❌ NULL")}");
            Debug.Log($"  Quit Button: {(so.FindProperty("quitButton").objectReferenceValue != null ? "✓ Assigned" : "❌ NULL")}");
        }

        Debug.Log("\n═══════════════════════════════════════════════════════");
        Debug.Log("DIAGNOSIS COMPLETE");
        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("\nNOTE: Buttons only work in PLAY MODE when the pause menu is VISIBLE!");
        Debug.Log("Press ESC in Play mode to open the pause menu, then try clicking buttons.");
        Debug.Log("═══════════════════════════════════════════════════════\n");
    }
}
