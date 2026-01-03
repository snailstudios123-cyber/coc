using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixPauseMenuDuplicates : EditorWindow
{
    [MenuItem("Tools/🔧 Fix Duplicate Pause Menus")]
    public static void FixDuplicates()
    {
        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("FIXING DUPLICATE PAUSE MENU PANELS");
        Debug.Log("═══════════════════════════════════════════════════════");

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found!");
            return;
        }

        Transform canvasTransform = canvas.transform;
        
        GameObject[] pausePanels = new GameObject[2];
        GameObject[] spellsPanels = new GameObject[2];
        
        int pauseCount = 0;
        int spellsCount = 0;

        for (int i = 0; i < canvasTransform.childCount; i++)
        {
            Transform child = canvasTransform.GetChild(i);
            
            if (child.name == "PauseMenuPanel" && pauseCount < 2)
            {
                pausePanels[pauseCount] = child.gameObject;
                pauseCount++;
            }
            else if (child.name == "SpellsMenuPanel" && spellsCount < 2)
            {
                spellsPanels[spellsCount] = child.gameObject;
                spellsCount++;
            }
        }

        Debug.Log($"Found {pauseCount} PauseMenuPanel(s)");
        Debug.Log($"Found {spellsCount} SpellsMenuPanel(s)");

        if (pauseCount > 1)
        {
            Debug.Log("Removing duplicate PauseMenuPanel...");
            DestroyImmediate(pausePanels[0]);
            Debug.Log("✓ First PauseMenuPanel removed, keeping the second one");
        }

        if (spellsCount > 1)
        {
            Debug.Log("Removing duplicate SpellsMenuPanel...");
            DestroyImmediate(spellsPanels[0]);
            Debug.Log("✓ First SpellsMenuPanel removed, keeping the second one");
        }

        PauseMenuManager manager = FindObjectOfType<PauseMenuManager>();
        if (manager != null)
        {
            SerializedObject so = new SerializedObject(manager);
            
            GameObject remainingPausePanel = GameObject.Find("PauseMenuPanel");
            GameObject remainingSpellsPanel = GameObject.Find("SpellsMenuPanel");
            
            if (remainingPausePanel != null)
            {
                so.FindProperty("pauseMenuPanel").objectReferenceValue = remainingPausePanel;
                Debug.Log("✓ Updated PauseMenuManager.pauseMenuPanel reference");
                
                Transform content = remainingPausePanel.transform.Find("Content");
                if (content != null)
                {
                    AssignButtonIfFound(so, "resumeButton", content.Find("ResumeButton"));
                    AssignButtonIfFound(so, "spellsButton", content.Find("SpellsButton"));
                    AssignButtonIfFound(so, "inventoryButton", content.Find("InventoryButton"));
                    AssignButtonIfFound(so, "optionsButton", content.Find("OptionsButton"));
                    AssignButtonIfFound(so, "quitButton", content.Find("QuitButton"));
                }
            }

            if (remainingSpellsPanel != null)
            {
                so.FindProperty("spellsMenuPanel").objectReferenceValue = remainingSpellsPanel;
                Debug.Log("✓ Updated PauseMenuManager.spellsMenuPanel reference");
                
                Transform spellDetail = remainingSpellsPanel.transform.Find("SpellDetailPanel");
                if (spellDetail != null)
                {
                    so.FindProperty("spellDetailPanel").objectReferenceValue = spellDetail.gameObject;
                }
                
                Transform backBtn = remainingSpellsPanel.transform.Find("BackButton");
                if (backBtn != null)
                {
                    AssignButtonIfFound(so, "backButton", backBtn);
                }
                
                Transform container = remainingSpellsPanel.transform.Find("SpellsListPanel/ScrollView/Viewport/SpellsListContainer");
                if (container != null)
                {
                    so.FindProperty("spellsListContainer").objectReferenceValue = container;
                }
            }

            so.ApplyModifiedProperties();
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("<color=green>✓ CLEANUP COMPLETE!</color>");
        Debug.Log("Now enter Play mode and press ESC to test the pause menu.");
        Debug.Log("═══════════════════════════════════════════════════════");
    }

    private static void AssignButtonIfFound(SerializedObject so, string propertyName, Transform transform)
    {
        if (transform != null)
        {
            Button button = transform.GetComponent<Button>();
            if (button != null)
            {
                so.FindProperty(propertyName).objectReferenceValue = button;
                Debug.Log($"  ✓ Assigned {propertyName}");
            }
        }
    }
}
