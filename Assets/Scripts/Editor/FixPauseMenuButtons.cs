using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class FixPauseMenuButtons : EditorWindow
{
    [MenuItem("Tools/🔧 Fix Pause Menu Buttons")]
    public static void Fix()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("   FIXING PAUSE MENU BUTTON ISSUE");
        Debug.Log("═══════════════════════════════════════════");
        
        string scenePath = "Assets/Scenes/New Scene.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        bool changesMade = false;
        
        GameObject inventoryPanel = GameObject.Find("Canvas/InventoryPanel");
        if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("✓ FIXED: Deactivated InventoryPanel");
            changesMade = true;
        }
        
        string[] uiElementsToFix = new string[]
        {
            "Canvas/Mana Container",
            "Canvas/Hearts Parent",
            "Canvas/CoinUI"
        };
        
        foreach (string path in uiElementsToFix)
        {
            GameObject obj = GameObject.Find(path);
            if (obj != null)
            {
                Image img = obj.GetComponent<Image>();
                if (img != null && img.raycastTarget)
                {
                    img.raycastTarget = false;
                    Debug.Log($"✓ FIXED: Disabled raycast target on {obj.name}");
                    changesMade = true;
                }
                
                Transform[] children = obj.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    Image childImg = child.GetComponent<Image>();
                    if (childImg != null && childImg.raycastTarget && 
                        !child.name.Contains("Button") && 
                        !child.GetComponent<Button>())
                    {
                        childImg.raycastTarget = false;
                        Debug.Log($"  ✓ Disabled raycast on child: {child.name}");
                        changesMade = true;
                    }
                }
            }
        }
        
        if (changesMade)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("✓ Scene saved successfully!");
        }
        else
        {
            Debug.Log("✓ All elements already properly configured!");
        }
        
        Debug.Log("");
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("   COMPLETE - TEST YOUR PAUSE MENU!");
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("1. Press Play");
        Debug.Log("2. Press Escape");
        Debug.Log("3. Click buttons - they should work now!");
        Debug.Log("═══════════════════════════════════════════");
    }
}
