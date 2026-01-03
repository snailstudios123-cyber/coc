using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupBotanicaAndBoss : EditorWindow
{
    [MenuItem("Tools/Story Progression/Setup Botanica & Boss Room")]
    public static void ShowWindow()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Setup Botanica & Boss",
            "This will:\n\n" +
            "1. Create BotanicaInitialDialogue and BotanicaFinalDialogue\n" +
            "2. Add Botanica NPC to UndergroundRuins scene\n" +
            "3. Create BossRoom scene with boss and rewards\n" +
            "4. Set up complete progression flow\n\n" +
            "Continue?",
            "Yes",
            "Cancel"
        );

        if (!confirm) return;

        CreateBotanicaDialogue.CreateDialogueAssets();
        SetupUndergroundRuinsScene();
        CreateBossRoomScene();

        EditorUtility.DisplayDialog(
            "Setup Complete!",
            "Botanica and Boss Room are ready!\n\n" +
            "Story Flow:\n" +
            "1. Player falls into UndergroundRuins\n" +
            "2. Botanica appears and speaks\n" +
            "3. Player explores dungeon\n" +
            "4. Player enters BossRoom\n" +
            "5. Defeat boss → Get rewards\n" +
            "6. Botanica returns and teleports player to village\n\n" +
            "Next: Customize dialogue and boss difficulty!",
            "OK"
        );
    }

    private static void SetupUndergroundRuinsScene()
    {
        string scenePath = "Assets/Scenes/UndergroundRuins.unity";

        if (!System.IO.File.Exists(scenePath))
        {
            Debug.LogWarning("[BotanicaSetup] UndergroundRuins scene doesn't exist! Run 'Setup Ruins Sequence' first.");
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject existingBotanica = GameObject.Find("Botanica");
        if (existingBotanica != null)
        {
            Debug.Log("[BotanicaSetup] Botanica already exists in UndergroundRuins");
            return;
        }

        GameObject botanica = new GameObject("Botanica");
        botanica.transform.position = new Vector3(0, 5, 0);

        SpriteRenderer renderer = botanica.AddComponent<SpriteRenderer>();
        renderer.color = new Color(0.6f, 1f, 0.6f);
        renderer.sortingOrder = 10;

        GameObject visualChild = new GameObject("Visual");
        visualChild.transform.SetParent(botanica.transform);
        visualChild.transform.localPosition = Vector3.zero;

        SpriteRenderer childRenderer = visualChild.AddComponent<SpriteRenderer>();
        childRenderer.color = new Color(0.6f, 1f, 0.6f, 1f);
        childRenderer.sortingOrder = 10;

        DialogueData initialDialogue = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/BotanicaInitialDialogue.asset");

        BotanicaEncounter encounter = botanica.AddComponent<BotanicaEncounter>();
        SerializedObject so = new SerializedObject(encounter);
        so.FindProperty("botanicaVisual").objectReferenceValue = visualChild;
        so.FindProperty("initialDialogue").objectReferenceValue = initialDialogue;
        so.FindProperty("appearDelay").floatValue = 1f;
        so.FindProperty("fadeInDuration").floatValue = 2f;
        so.FindProperty("autoTrigger").boolValue = true;
        so.ApplyModifiedProperties();

        GameObject bossRoomTrigger = new GameObject("Trigger - To Boss Room");
        bossRoomTrigger.transform.position = new Vector3(15, 2, 0);

        BoxCollider2D triggerCollider = bossRoomTrigger.AddComponent<BoxCollider2D>();
        triggerCollider.size = new Vector2(2, 5);
        triggerCollider.isTrigger = true;

        SceneTransitionTrigger transition = bossRoomTrigger.AddComponent<SceneTransitionTrigger>();
        SerializedObject transitionSO = new SerializedObject(transition);
        transitionSO.FindProperty("targetSceneName").stringValue = "BossRoom";
        transitionSO.FindProperty("spawnPosition").vector2Value = new Vector2(-8, 0);
        transitionSO.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[BotanicaSetup] ✓ Added Botanica to UndergroundRuins scene");
    }

    private static void CreateBossRoomScene()
    {
        string scenePath = "Assets/Scenes/BossRoom.unity";

        if (System.IO.File.Exists(scenePath))
        {
            Debug.Log("[BotanicaSetup] BossRoom scene already exists");
            return;
        }

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject mainCam = GameObject.Find("Main Camera");
        if (mainCam != null)
        {
            Camera cam = mainCam.GetComponent<Camera>();
            if (cam != null)
            {
                cam.backgroundColor = new Color(0.1f, 0.05f, 0.05f);
                cam.orthographic = true;
                cam.orthographicSize = 6;
            }
        }

        GameObject spawnPoint = new GameObject("Player Spawn Point");
        spawnPoint.transform.position = new Vector3(-8, 0, 0);

        GameObject arena = new GameObject("Arena");

        GameObject ground = new GameObject("Ground");
        ground.transform.SetParent(arena.transform);
        ground.transform.position = new Vector3(0, -3, 0);

        BoxCollider2D groundCollider = ground.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(25, 1);

        SpriteRenderer groundRenderer = ground.AddComponent<SpriteRenderer>();
        groundRenderer.color = new Color(0.25f, 0.2f, 0.2f);
        groundRenderer.drawMode = SpriteDrawMode.Tiled;
        groundRenderer.size = new Vector2(25, 1);

        GameObject wallLeft = new GameObject("Wall Left");
        wallLeft.transform.SetParent(arena.transform);
        wallLeft.transform.position = new Vector3(-12, 2, 0);
        BoxCollider2D leftCollider = wallLeft.AddComponent<BoxCollider2D>();
        leftCollider.size = new Vector2(1, 10);

        GameObject wallRight = new GameObject("Wall Right");
        wallRight.transform.SetParent(arena.transform);
        wallRight.transform.position = new Vector3(12, 2, 0);
        BoxCollider2D rightCollider = wallRight.AddComponent<BoxCollider2D>();
        rightCollider.size = new Vector2(1, 10);

        GameObject bossSpawn = new GameObject("Boss Spawn Point");
        bossSpawn.transform.position = new Vector3(5, 0, 0);

        GameObject rewardHandler = new GameObject("Boss Reward Handler");
        rewardHandler.transform.position = Vector3.zero;

        DialogueData finalDialogue = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/BotanicaFinalDialogue.asset");

        GameObject botanicaVisual = new GameObject("Botanica Visual");
        botanicaVisual.transform.SetParent(rewardHandler.transform);
        botanicaVisual.transform.localPosition = new Vector3(0, 3, 0);

        SpriteRenderer botanicaRenderer = botanicaVisual.AddComponent<SpriteRenderer>();
        botanicaRenderer.color = new Color(0.6f, 1f, 0.6f, 0f);
        botanicaRenderer.sortingOrder = 10;

        BossRewardHandler rewardScript = rewardHandler.AddComponent<BossRewardHandler>();
        SerializedObject rewardSO = new SerializedObject(rewardScript);
        rewardSO.FindProperty("healthIncrease").intValue = 1;
        rewardSO.FindProperty("giveFullMana").boolValue = true;
        rewardSO.FindProperty("botanicaVisual").objectReferenceValue = botanicaVisual;
        rewardSO.FindProperty("finalDialogue").objectReferenceValue = finalDialogue;
        rewardSO.FindProperty("appearDelay").floatValue = 2f;
        rewardSO.FindProperty("fadeInDuration").floatValue = 2f;
        rewardSO.FindProperty("villageSceneName").stringValue = "Outdoors";
        rewardSO.FindProperty("villageSpawnPosition").vector2Value = new Vector2(0, 0);
        rewardSO.FindProperty("teleportDelay").floatValue = 1f;
        rewardSO.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(newScene, scenePath);
        Debug.Log($"[BotanicaSetup] ✓ Created BossRoom scene at {scenePath}");

        AddSceneToBuildSettings(scenePath);
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        EditorBuildSettingsScene[] originalScenes = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene scene in originalScenes)
        {
            if (scene.path == scenePath)
            {
                Debug.Log($"[BotanicaSetup] Scene already in build settings: {scenePath}");
                return;
            }
        }

        EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[originalScenes.Length + 1];
        System.Array.Copy(originalScenes, newScenes, originalScenes.Length);
        newScenes[originalScenes.Length] = new EditorBuildSettingsScene(scenePath, true);

        EditorBuildSettings.scenes = newScenes;
        Debug.Log($"[BotanicaSetup] ✓ Added {scenePath} to build settings");
    }
}
