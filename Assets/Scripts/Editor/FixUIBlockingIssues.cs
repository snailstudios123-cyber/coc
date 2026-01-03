using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixUIBlockingIssues : EditorWindow
{
    [MenuItem("Tools/⚡ Fix UI Blocking Issues")]
    public static void FixUIBlocking()
    {
        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("FIXING UI BLOCKING ISSUES");
        Debug.Log("═══════════════════════════════════════════════════════");

        bool fixedAnything = false;

        GameObject fadeImage = GameObject.Find("FadeImage");
        if (fadeImage != null)
        {
            Image img = fadeImage.GetComponent<Image>();
            if (img != null && img.raycastTarget)
            {
                img.raycastTarget = false;
                Debug.Log($"✓ Fixed FadeImage - disabled raycastTarget");
                Debug.Log($"  Path: {GetGameObjectPath(fadeImage.transform)}");
                fixedAnything = true;
            }
        }

        Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);
        foreach (Canvas canvas in allCanvases)
        {
            Image[] images = canvas.GetComponentsInChildren<Image>(true);
            foreach (Image img in images)
            {
                if (img.raycastTarget && img.color.a < 0.01f && !img.GetComponent<Button>())
                {
                    string path = GetGameObjectPath(img.transform);
                    
                    if (!path.Contains("PauseMenuPanel") && 
                        !path.Contains("Button") && 
                        !path.Contains("SpellsMenuPanel") &&
                        !path.Contains("InventoryPanel"))
                    {
                        img.raycastTarget = false;
                        Debug.Log($"✓ Fixed transparent blocking image: {path}");
                        fixedAnything = true;
                    }
                }
            }
        }

        if (!fixedAnything)
        {
            Debug.Log("No blocking UI elements found!");
        }

        EditorUtility.SetDirty(FindObjectOfType<Canvas>().gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("<color=green>✓ UI BLOCKING ISSUES FIXED!</color>");
        Debug.Log("Now test the pause menu in Play mode.");
        Debug.Log("═══════════════════════════════════════════════════════");
    }

    private static string GetGameObjectPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}
