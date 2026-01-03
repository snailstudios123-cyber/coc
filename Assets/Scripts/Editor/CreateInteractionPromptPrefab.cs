using UnityEngine;
using UnityEditor;
using TMPro;

public class CreateInteractionPromptPrefab : EditorWindow
{
    [MenuItem("Tools/Create E Prompt Prefab")]
    public static void CreatePromptPrefab()
    {
        GameObject prompt = new GameObject("E_Prompt");
        
        Canvas canvas = prompt.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = prompt.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2f, 1f);
        canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(prompt.transform);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Press E";
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(prompt.transform);
        bgObj.transform.SetAsFirstSibling();
        
        UnityEngine.UI.Image bg = bgObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        string prefabPath = "Assets/Prefabs/E_Prompt.prefab";
        PrefabUtility.SaveAsPrefabAsset(prompt, prefabPath);
        DestroyImmediate(prompt);
        
        EditorUtility.DisplayDialog("Success", "Created E_Prompt prefab at:\nAssets/Prefabs/E_Prompt.prefab\n\nYou can now assign this to the ChoreInteractable components.", "OK");
        
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }
}
