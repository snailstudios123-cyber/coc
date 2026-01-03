using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupRuinsProgression : EditorWindow
{
    [MenuItem("Tools/Story Progression/Setup Ruins Sequence")]
    public static void ShowWindow()
    {
        SetupRuins();
    }

    private static void SetupRuins()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Setup Ruins Progression",
            "This will:\n\n" +
            "1. Create OuterRuins scene (if missing)\n" +
            "2. Create UndergroundRuins scene (if missing)\n" +
            "3. Add glowing goddess statue with interact trigger\n" +
            "4. Set up scene transitions\n\n" +
            "Continue?",
            "Yes",
            "Cancel"
        );

        if (!confirm) return;

        CreateOuterRuinsScene();
        CreateUndergroundRuinsScene();

        EditorUtility.DisplayDialog(
            "Setup Complete!",
            "Ruins scenes created!\n\n" +
            "Next steps:\n" +
            "1. Open OuterRuins scene\n" +
            "2. Select 'Goddess Statue' in hierarchy\n" +
            "3. Assign glow and ground break particle effects\n" +
            "4. The statue already glows - player presses E to interact!\n\n" +
            "Test by going to the Outdoors scene and walking to the right exit.",
            "OK"
        );
    }

    private static void CreateOuterRuinsScene()
    {
        string scenePath = "Assets/Scenes/OuterRuins.unity";
        
        if (System.IO.File.Exists(scenePath))
        {
            Debug.Log("[RuinsSetup] OuterRuins scene already exists");
            return;
        }

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject mainCam = GameObject.Find("Main Camera");
        if (mainCam != null)
        {
            Camera cam = mainCam.GetComponent<Camera>();
            if (cam != null)
            {
                cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
                cam.orthographic = true;
                cam.orthographicSize = 5;
            }
        }

        GameObject statue = new GameObject("Goddess Statue");
        statue.transform.position = new Vector3(8, 0, 0);
        
        SpriteRenderer statueRenderer = statue.AddComponent<SpriteRenderer>();
        statueRenderer.color = new Color(0.9f, 0.9f, 1f);
        statueRenderer.sortingOrder = 1;
        
        BoxCollider2D statueCollider = statue.AddComponent<BoxCollider2D>();
        statueCollider.size = new Vector2(1.5f, 3f);
        statueCollider.isTrigger = false;
        
        GameObject interactPrompt = new GameObject("Interact Prompt");
        interactPrompt.transform.SetParent(statue.transform);
        interactPrompt.transform.localPosition = new Vector3(0, 2f, 0);
        
        TextMesh promptText = interactPrompt.AddComponent<TextMesh>();
        promptText.text = "Press E";
        promptText.fontSize = 20;
        promptText.anchor = TextAnchor.MiddleCenter;
        promptText.alignment = TextAlignment.Center;
        promptText.color = Color.yellow;
        
        MeshRenderer promptRenderer = interactPrompt.GetComponent<MeshRenderer>();
        promptRenderer.sortingOrder = 100;
        
        StatueCutsceneTrigger cutsceneTrigger = statue.AddComponent<StatueCutsceneTrigger>();
        SerializedObject so = new SerializedObject(cutsceneTrigger);
        so.FindProperty("statueObject").objectReferenceValue = statue;
        so.FindProperty("interactPrompt").objectReferenceValue = interactPrompt;
        so.FindProperty("undergroundSceneName").stringValue = "UndergroundRuins";
        so.FindProperty("undergroundSpawnPosition").vector2Value = new Vector2(0, 3);
        so.FindProperty("interactionRange").floatValue = 2f;
        so.ApplyModifiedProperties();
        
        interactPrompt.SetActive(false);

        GameObject ground = new GameObject("Ground");
        ground.transform.position = new Vector3(0, -2, 0);
        
        BoxCollider2D groundCollider = ground.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(20, 1);
        
        SpriteRenderer groundRenderer = ground.AddComponent<SpriteRenderer>();
        groundRenderer.color = new Color(0.3f, 0.25f, 0.2f);
        groundRenderer.drawMode = SpriteDrawMode.Tiled;
        groundRenderer.size = new Vector2(20, 1);

        GameObject spawnPoint = new GameObject("Player Spawn Point");
        spawnPoint.transform.position = new Vector3(-8, 0, 0);
        
        GameObject boundaryLeft = new GameObject("Boundary Left");
        boundaryLeft.transform.position = new Vector3(-10, 0, 0);
        BoxCollider2D leftCollider = boundaryLeft.AddComponent<BoxCollider2D>();
        leftCollider.size = new Vector2(1, 10);
        
        GameObject boundaryRight = new GameObject("Boundary Right");
        boundaryRight.transform.position = new Vector3(10, 0, 0);
        BoxCollider2D rightCollider = boundaryRight.AddComponent<BoxCollider2D>();
        rightCollider.size = new Vector2(1, 10);

        EditorSceneManager.SaveScene(newScene, scenePath);
        Debug.Log($"[RuinsSetup] ✓ Created OuterRuins scene at {scenePath}");
        
        AddSceneToBuildSettings(scenePath);
    }

    private static void CreateUndergroundRuinsScene()
    {
        string scenePath = "Assets/Scenes/UndergroundRuins.unity";
        
        if (System.IO.File.Exists(scenePath))
        {
            Debug.Log("[RuinsSetup] UndergroundRuins scene already exists");
            return;
        }

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject mainCam = GameObject.Find("Main Camera");
        if (mainCam != null)
        {
            Camera cam = mainCam.GetComponent<Camera>();
            if (cam != null)
            {
                cam.backgroundColor = new Color(0.05f, 0.05f, 0.1f);
                cam.orthographic = true;
                cam.orthographicSize = 5;
            }
        }

        GameObject spawnPoint = new GameObject("Player Spawn Point");
        spawnPoint.transform.position = new Vector3(0, 3, 0);

        GameObject ground = new GameObject("Ground");
        ground.transform.position = new Vector3(0, 0, 0);
        
        BoxCollider2D groundCollider = ground.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(20, 1);
        
        SpriteRenderer groundRenderer = ground.AddComponent<SpriteRenderer>();
        groundRenderer.color = new Color(0.2f, 0.15f, 0.15f);
        groundRenderer.drawMode = SpriteDrawMode.Tiled;
        groundRenderer.size = new Vector2(20, 1);

        GameObject platformLeft = CreatePlatform("Platform Left", new Vector3(-5, 2, 0), new Vector2(4, 0.5f));
        GameObject platformRight = CreatePlatform("Platform Right", new Vector3(5, 2, 0), new Vector2(4, 0.5f));

        GameObject wallLeft = new GameObject("Wall Left");
        wallLeft.transform.position = new Vector3(-10, 5, 0);
        BoxCollider2D leftCollider = wallLeft.AddComponent<BoxCollider2D>();
        leftCollider.size = new Vector2(1, 10);
        
        GameObject wallRight = new GameObject("Wall Right");
        wallRight.transform.position = new Vector3(10, 5, 0);
        BoxCollider2D rightCollider = wallRight.AddComponent<BoxCollider2D>();
        rightCollider.size = new Vector2(1, 10);

        EditorSceneManager.SaveScene(newScene, scenePath);
        Debug.Log($"[RuinsSetup] ✓ Created UndergroundRuins scene at {scenePath}");
        
        AddSceneToBuildSettings(scenePath);
    }

    private static GameObject CreatePlatform(string name, Vector3 position, Vector2 size)
    {
        GameObject platform = new GameObject(name);
        platform.transform.position = position;
        
        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = size;
        
        SpriteRenderer renderer = platform.AddComponent<SpriteRenderer>();
        renderer.color = new Color(0.4f, 0.3f, 0.25f);
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = size;
        
        return platform;
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        EditorBuildSettingsScene[] originalScenes = EditorBuildSettings.scenes;
        
        foreach (EditorBuildSettingsScene scene in originalScenes)
        {
            if (scene.path == scenePath)
            {
                Debug.Log($"[RuinsSetup] Scene already in build settings: {scenePath}");
                return;
            }
        }
        
        EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[originalScenes.Length + 1];
        System.Array.Copy(originalScenes, newScenes, originalScenes.Length);
        newScenes[originalScenes.Length] = new EditorBuildSettingsScene(scenePath, true);
        
        EditorBuildSettings.scenes = newScenes;
        Debug.Log($"[RuinsSetup] ✓ Added {scenePath} to build settings");
    }
}
