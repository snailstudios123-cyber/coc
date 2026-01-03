using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class OutsideSceneSetup : EditorWindow
{
    [MenuItem("Tools/Setup Outside Scene")]
    public static void SetupOutsideScene()
    {
        if (!EditorUtility.DisplayDialog("Setup Outside Scene", 
            "This will create a new 'OutsideScene' with:\n\n" +
            "• Main Camera with CameraFollow\n" +
            "• Player (Electus prefab)\n" +
            "• Canvas with UI (Health, Mana, Money)\n" +
            "• Managers (Spell, Quest, Dialogue, Pause, Scene Transition)\n" +
            "• 3 Chore objects (Telekinesis, Beam, Lora Vitis)\n" +
            "• Ground tilemap\n\n" +
            "Continue?", 
            "Yes", "Cancel"))
        {
            return;
        }

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        SetupCamera();
        SetupPlayer();
        SetupCanvas();
        SetupManagers();
        SetupChores();
        SetupGround();
        
        string scenePath = "Assets/Scenes/OutsideScene.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        EditorUtility.DisplayDialog("Success!", 
            $"Outside scene created at:\n{scenePath}\n\n" +
            "Next steps:\n" +
            "1. Add ground tiles to the Tilemap\n" +
            "2. Position the chore objects\n" +
            "3. Add sprites to the chores\n" +
            "4. Add this scene to Build Settings\n" +
            "5. Create scene transitions from DownstairsScene", 
            "OK");
    }

    private static void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5;
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0.53f, 0.81f, 0.92f);
            
            CameraFollow cameraFollow = mainCamera.gameObject.GetComponent<CameraFollow>();
            if (cameraFollow == null)
            {
                cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
            }
            
            Debug.Log("✓ Camera setup complete");
        }
    }

    private static GameObject SetupPlayer()
    {
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Electus.prefab");
        
        if (playerPrefab == null)
        {
            playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/Electus.prefab");
        }
        
        if (playerPrefab == null)
        {
            Debug.LogError("❌ Could not find Electus prefab! Create player manually.");
            return null;
        }
        
        GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        player.transform.position = new Vector3(0, 0, 0);
        player.name = "Electus";
        
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                SerializedObject so = new SerializedObject(cameraFollow);
                so.FindProperty("target").objectReferenceValue = player.transform;
                so.ApplyModifiedProperties();
            }
        }
        
        Debug.Log("✓ Player setup complete");
        return player;
    }

    private static void SetupCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        GameObject healthBar = CreateUIElement("HealthBar", canvasObj.transform, new Vector2(50, -50), new Vector2(200, 30));
        GameObject manaBar = CreateUIElement("ManaBar", canvasObj.transform, new Vector2(50, -90), new Vector2(200, 30));
        GameObject moneyText = CreateUIElement("MoneyText", canvasObj.transform, new Vector2(-50, -50), new Vector2(150, 50));
        
        Debug.Log("✓ Canvas setup complete");
    }

    private static GameObject CreateUIElement(string name, Transform parent, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = name.Contains("Money") ? new Vector2(1, 1) : new Vector2(0, 1);
        rect.anchorMax = name.Contains("Money") ? new Vector2(1, 1) : new Vector2(0, 1);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        
        return obj;
    }

    private static void SetupManagers()
    {
        GameObject managers = new GameObject("--- MANAGERS ---");
        
        GameObject spellManager = new GameObject("SpellManager");
        spellManager.transform.SetParent(managers.transform);
        spellManager.AddComponent<SpellManager>();
        
        GameObject questManager = new GameObject("QuestManager");
        questManager.transform.SetParent(managers.transform);
        questManager.AddComponent<QuestManager>();
        
        GameObject dialogueManager = new GameObject("DialogueManager");
        dialogueManager.transform.SetParent(managers.transform);
        dialogueManager.AddComponent<DialogueManager>();
        
        GameObject pauseManager = new GameObject("PauseMenuManager");
        pauseManager.transform.SetParent(managers.transform);
        pauseManager.AddComponent<PauseMenuManager>();
        
        GameObject sceneTransition = new GameObject("SceneTransitionManager");
        sceneTransition.transform.SetParent(managers.transform);
        sceneTransition.AddComponent<SceneTransitionManager>();
        
        Debug.Log("✓ Managers setup complete");
    }

    private static void SetupChores()
    {
        SpellData telekinesis = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/Telekinesis.asset");
        SpellData beamSpell = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/BeamSpell.asset");
        SpellData loraVitis = AssetDatabase.LoadAssetAtPath<SpellData>("Assets/ScriptableObjects/Spells/Lora Vitis.asset");

        GameObject choresParent = new GameObject("--- CHORES ---");
        
        if (telekinesis != null)
        {
            CreateChoreObject("Chore_Telekinesis", new Vector3(-8, 0, 0), telekinesis, "Wash the Dishes", choresParent.transform);
        }
        
        if (beamSpell != null)
        {
            CreateChoreObject("Chore_BeamSpell", new Vector3(0, 0, 0), beamSpell, "Water the Plants", choresParent.transform);
        }
        
        if (loraVitis != null)
        {
            CreateChoreObject("Chore_LoraVitis", new Vector3(8, 0, 0), loraVitis, "Clean the Yard", choresParent.transform);
        }
        
        Debug.Log("✓ Chores setup complete");
    }

    private static void CreateChoreObject(string name, Vector3 position, SpellData reward, string choreName, Transform parent)
    {
        GameObject chore = new GameObject(name);
        chore.transform.SetParent(parent);
        chore.transform.position = position;

        SpriteRenderer sr = chore.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 0.9f, 0.3f);
        sr.sortingOrder = 1;
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        BoxCollider2D collider = chore.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.5f, 1.5f);

        ChoreInteractable choreScript = chore.AddComponent<ChoreInteractable>();
        SerializedObject so = new SerializedObject(choreScript);
        so.FindProperty("choreName").stringValue = choreName;
        so.FindProperty("spellReward").objectReferenceValue = reward;
        so.FindProperty("interactionRange").floatValue = 3f;
        so.FindProperty("spriteRenderer").objectReferenceValue = sr;
        so.ApplyModifiedProperties();
    }

    private static void SetupGround()
    {
        GameObject gridObj = new GameObject("Grid");
        Grid grid = gridObj.AddComponent<Grid>();
        
        GameObject tilemapObj = new GameObject("Ground");
        tilemapObj.transform.SetParent(gridObj.transform);
        
        tilemapObj.AddComponent<UnityEngine.Tilemaps.Tilemap>();
        UnityEngine.Tilemaps.TilemapRenderer renderer = tilemapObj.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();
        renderer.sortingOrder = -1;
        
        BoxCollider2D groundCollider = tilemapObj.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(50, 1);
        groundCollider.offset = new Vector2(0, -5);
        
        tilemapObj.layer = LayerMask.NameToLayer("Ground");
        
        Debug.Log("✓ Ground tilemap setup complete");
    }
}
