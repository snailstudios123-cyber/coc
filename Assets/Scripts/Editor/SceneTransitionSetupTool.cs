using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SceneTransitionSetupTool : EditorWindow
{
    [MenuItem("Tools/Setup Scene Transition System")]
    public static void SetupSceneTransition()
    {
        if (GameObject.Find("SceneTransitionCanvas") != null)
        {
            bool proceed = EditorUtility.DisplayDialog(
                "Scene Transition Already Exists",
                "A SceneTransitionCanvas already exists in the scene. Do you want to replace it?",
                "Yes, Replace",
                "Cancel"
            );

            if (proceed)
            {
                DestroyImmediate(GameObject.Find("SceneTransitionCanvas"));
            }
            else
            {
                return;
            }
        }

        GameObject canvasObj = new GameObject("SceneTransitionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject fadeImageObj = new GameObject("FadeImage");
        fadeImageObj.transform.SetParent(canvasObj.transform, false);

        Image fadeImage = fadeImageObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        RectTransform rectTransform = fadeImageObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        SceneTransitionManager transitionManager = canvasObj.AddComponent<SceneTransitionManager>();

        SerializedObject serializedManager = new SerializedObject(transitionManager);
        serializedManager.FindProperty("fadeImage").objectReferenceValue = fadeImage;
        serializedManager.FindProperty("fadeDuration").floatValue = 1f;
        serializedManager.FindProperty("fadeColor").colorValue = Color.black;
        serializedManager.ApplyModifiedProperties();

        EditorUtility.SetDirty(canvasObj);
        Selection.activeGameObject = canvasObj;

        EditorUtility.DisplayDialog(
            "Setup Complete!",
            "Scene Transition System has been set up successfully!\n\n" +
            "The SceneTransitionCanvas will persist between scenes.\n\n" +
            "To create a transition trigger:\n" +
            "1. Create an empty GameObject\n" +
            "2. Add BoxCollider2D (set to Trigger)\n" +
            "3. Add SceneTransitionTrigger component\n" +
            "4. Configure target scene and spawn position",
            "OK"
        );

        Debug.Log("Scene Transition System setup complete!");
    }

    [MenuItem("GameObject/Scene Transition/Create Transition Trigger", false, 10)]
    public static void CreateTransitionTrigger()
    {
        GameObject triggerObj = new GameObject("SceneTransitionTrigger");

        BoxCollider2D collider = triggerObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(2f, 4f);

        SceneTransitionTrigger trigger = triggerObj.AddComponent<SceneTransitionTrigger>();

        SerializedObject serializedTrigger = new SerializedObject(trigger);
        serializedTrigger.FindProperty("targetSceneName").stringValue = "New Scene";
        serializedTrigger.FindProperty("spawnPosition").vector2Value = Vector2.zero;
        serializedTrigger.FindProperty("useSpawnPosition").boolValue = true;
        serializedTrigger.FindProperty("showGizmo").boolValue = true;
        serializedTrigger.FindProperty("gizmoColor").colorValue = new Color(0f, 1f, 0f, 0.3f);
        serializedTrigger.ApplyModifiedProperties();

        if (Selection.activeTransform != null)
        {
            triggerObj.transform.SetParent(Selection.activeTransform);
        }

        triggerObj.transform.localPosition = Vector3.zero;

        EditorUtility.SetDirty(triggerObj);
        Selection.activeGameObject = triggerObj;

        Debug.Log("Scene Transition Trigger created! Configure the target scene name and spawn position in the Inspector.");
    }
}
