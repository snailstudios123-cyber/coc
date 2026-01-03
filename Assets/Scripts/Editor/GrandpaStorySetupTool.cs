using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

public class GrandpaStorySetupTool : EditorWindow
{
    private string bedroomSceneName = "BedroomScene";
    private string downstairsSceneName = "DownstairsScene";
    private Vector2 bedroomTransitionPos = new Vector2(-5, 0);
    private Vector2 downstairsSpawnPosition = new Vector2(-4, 0);
    private Vector2 grandpaNPCPosition = new Vector2(3, 0);

    [MenuItem("Tools/Setup Grandpa Story Progression")]
    public static void ShowWindow()
    {
        GetWindow<GrandpaStorySetupTool>("Grandpa Story Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Grandpa Story Progression - Full Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Scene Names:", EditorStyles.boldLabel);
        bedroomSceneName = EditorGUILayout.TextField("Bedroom Scene", bedroomSceneName);
        downstairsSceneName = EditorGUILayout.TextField("Downstairs Scene", downstairsSceneName);

        GUILayout.Space(10);
        GUILayout.Label("Positions:", EditorStyles.boldLabel);
        bedroomTransitionPos = EditorGUILayout.Vector2Field("Bedroom Exit Position", bedroomTransitionPos);
        downstairsSpawnPosition = EditorGUILayout.Vector2Field("Downstairs Spawn", downstairsSpawnPosition);
        grandpaNPCPosition = EditorGUILayout.Vector2Field("Grandpa Position", grandpaNPCPosition);

        GUILayout.Space(20);

        if (GUILayout.Button("CREATE COMPLETE STORY SCENES!", GUILayout.Height(40)))
        {
            SetupEverything();
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "This will create:\n" +
            "✓ Two complete scenes (Bedroom + Downstairs)\n" +
            "✓ All dialogue assets\n" +
            "✓ Scene transition system\n" +
            "✓ Grandpa NPC with quest system\n" +
            "✓ UI and interaction systems", 
            MessageType.Info);
    }

    private void SetupEverything()
    {
        if (!EditorUtility.DisplayDialog("Create Story Scenes?",
            $"This will create:\n\n" +
            $"• {bedroomSceneName}.unity\n" +
            $"• {downstairsSceneName}.unity\n" +
            $"• All dialogue and quest systems\n\n" +
            $"Continue?",
            "Yes, Create Everything!", "Cancel"))
        {
            return;
        }

        EditorUtility.DisplayProgressBar("Setup", "Creating dialogue assets...", 0.1f);
        CreateDialogueAssets();

        EditorUtility.DisplayProgressBar("Setup", "Creating bedroom scene...", 0.3f);
        CreateBedroomScene();

        EditorUtility.DisplayProgressBar("Setup", "Creating downstairs scene...", 0.6f);
        CreateDownstairsScene();

        EditorUtility.DisplayProgressBar("Setup", "Finalizing...", 0.9f);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Success!", 
            $"Story scenes created!\n\n" +
            $"✓ {bedroomSceneName}\n" +
            $"✓ {downstairsSceneName}\n\n" +
            $"Open {bedroomSceneName} and press Play to test!\n\n" +
            $"Note: Add your Player prefab to both scenes.", 
            "OK");
    }

    private void CreateDialogueAssets()
    {
        CreateGrandpaCallDialogue();
        CreateGrandpaChoresDialogue();
        CreateGrandpaAfterChoresDialogue();
    }

    private void CreateGrandpaCallDialogue()
    {
        string path = "Assets/GrandpaCallDialogue.asset";
        if (AssetDatabase.LoadAssetAtPath<DialogueData>(path) != null)
        {
            Debug.Log("[Setup] GrandpaCallDialogue already exists, skipping...");
            return;
        }

        DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
        dialogue.dialogueName = "Grandpa's Call";
        dialogue.canRepeat = false;

        dialogue.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speakerName = "Grandpa",
                text = "Hey! Wake up, sleepyhead!",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Grandpa",
                text = "Come downstairs, I need to talk to you!",
                typingSpeed = 0.05f
            }
        };

        AssetDatabase.CreateAsset(dialogue, path);
        Debug.Log($"[Setup] Created {path}");
    }

    private void CreateGrandpaChoresDialogue()
    {
        string path = "Assets/GrandpaChoresDialogue.asset";
        if (AssetDatabase.LoadAssetAtPath<DialogueData>(path) != null)
        {
            Debug.Log("[Setup] GrandpaChoresDialogue already exists, skipping...");
            return;
        }

        DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
        dialogue.dialogueName = "Grandpa's Chores";
        dialogue.canRepeat = false;

        dialogue.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speakerName = "Grandpa",
                text = "Ah, there you are! Good morning!",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Grandpa",
                text = "I've been meaning to talk to you about something important.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Grandpa",
                text = "There are some chores that need doing around here.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Grandpa",
                text = "Can you help your old grandpa out?",
                typingSpeed = 0.05f
            }
        };

        AssetDatabase.CreateAsset(dialogue, path);
        Debug.Log($"[Setup] Created {path}");
    }

    private void CreateGrandpaAfterChoresDialogue()
    {
        string path = "Assets/GrandpaAfterChoresDialogue.asset";
        if (AssetDatabase.LoadAssetAtPath<DialogueData>(path) != null)
        {
            Debug.Log("[Setup] GrandpaAfterChoresDialogue already exists, skipping...");
            return;
        }

        DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
        dialogue.dialogueName = "Grandpa After Chores";
        dialogue.canRepeat = true;

        dialogue.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speakerName = "Grandpa",
                text = "Thank you for helping out!",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Grandpa",
                text = "You're a good kid.",
                typingSpeed = 0.05f
            }
        };

        AssetDatabase.CreateAsset(dialogue, path);
        Debug.Log($"[Setup] Created {path}");
    }

    private void CreateBedroomScene()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        CreateCamera();
        CreateSceneTransitionManager();
        CreateDialogueManagerInScene();
        CreateGrandpaCallTriggerInScene();
        CreateSceneTransitionTrigger();

        string scenePath = $"Assets/Scenes/{bedroomSceneName}.unity";
        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"[Setup] Created {scenePath}");
    }

    private void CreateDownstairsScene()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        CreateCamera();
        CreateSceneTransitionManager();
        CreateDialogueManagerInScene();
        CreateQuestManager();
        CreateGrandpaNPC();
        CreateInteractionUI();

        string scenePath = $"Assets/Scenes/{downstairsSceneName}.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"[Setup] Created {scenePath}");
    }

    private void CreateCamera()
    {
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.backgroundColor = new Color(0.2f, 0.2f, 0.3f);
        cam.orthographic = true;
        cam.orthographicSize = 5;
        camObj.tag = "MainCamera";
        camObj.AddComponent<AudioListener>();
        
        CameraFollow cameraFollow = camObj.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            Debug.Log("[Setup] CameraFollow script not found, add it manually if needed");
        }
    }

    private void CreateSceneTransitionManager()
    {
        if (FindObjectOfType<SceneTransitionManager>() != null) return;

        GameObject transitionObj = new GameObject("SceneTransitionManager");
        SceneTransitionManager manager = transitionObj.AddComponent<SceneTransitionManager>();

        Canvas canvas = new GameObject("TransitionCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvas.transform.SetParent(transitionObj.transform);
        canvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();

        GameObject fadePanel = new GameObject("FadeImage");
        fadePanel.transform.SetParent(canvas.transform, false);
        UnityEngine.UI.Image fadeImage = fadePanel.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = Color.black;
        
        RectTransform rect = fadePanel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("fadeImage").objectReferenceValue = fadeImage;
        serializedManager.FindProperty("fadeDuration").floatValue = 1f;
        serializedManager.ApplyModifiedProperties();
    }

    private void CreateDialogueManagerInScene()
    {
        if (FindObjectOfType<DialogueManager>() != null) return;

        GameObject dialogueObj = new GameObject("DialogueManager");
        DialogueManager manager = dialogueObj.AddComponent<DialogueManager>();

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        GameObject dialoguePanel = new GameObject("DialoguePanel");
        dialoguePanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = dialoguePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.1f);
        panelRect.anchorMax = new Vector2(0.9f, 0.3f);
        panelRect.sizeDelta = Vector2.zero;

        UnityEngine.UI.Image panelImage = dialoguePanel.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        GameObject speakerText = new GameObject("SpeakerName");
        speakerText.transform.SetParent(dialoguePanel.transform, false);
        TMPro.TextMeshProUGUI speakerTMP = speakerText.AddComponent<TMPro.TextMeshProUGUI>();
        speakerTMP.text = "Speaker";
        speakerTMP.fontSize = 24;
        speakerTMP.color = Color.yellow;
        RectTransform speakerRect = speakerText.GetComponent<RectTransform>();
        speakerRect.anchorMin = new Vector2(0, 0.7f);
        speakerRect.anchorMax = new Vector2(1, 1);
        speakerRect.offsetMin = new Vector2(20, 0);
        speakerRect.offsetMax = new Vector2(-20, -10);

        GameObject dialogueText = new GameObject("DialogueText");
        dialogueText.transform.SetParent(dialoguePanel.transform, false);
        TMPro.TextMeshProUGUI dialogueTMP = dialogueText.AddComponent<TMPro.TextMeshProUGUI>();
        dialogueTMP.text = "Dialogue text appears here...";
        dialogueTMP.fontSize = 20;
        RectTransform dialogueRect = dialogueText.GetComponent<RectTransform>();
        dialogueRect.anchorMin = new Vector2(0, 0);
        dialogueRect.anchorMax = new Vector2(1, 0.7f);
        dialogueRect.offsetMin = new Vector2(20, 10);
        dialogueRect.offsetMax = new Vector2(-20, 0);

        GameObject continuePrompt = new GameObject("ContinuePrompt");
        continuePrompt.transform.SetParent(dialoguePanel.transform, false);
        TMPro.TextMeshProUGUI continueTMP = continuePrompt.AddComponent<TMPro.TextMeshProUGUI>();
        continueTMP.text = "Press E to continue...";
        continueTMP.fontSize = 16;
        continueTMP.alignment = TMPro.TextAlignmentOptions.Right;
        RectTransform continueRect = continuePrompt.GetComponent<RectTransform>();
        continueRect.anchorMin = new Vector2(0.5f, 0);
        continueRect.anchorMax = new Vector2(1, 0.2f);
        continueRect.offsetMin = new Vector2(0, 5);
        continueRect.offsetMax = new Vector2(-20, -5);

        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("dialoguePanel").objectReferenceValue = dialoguePanel;
        serializedManager.FindProperty("speakerNameText").objectReferenceValue = speakerTMP;
        serializedManager.FindProperty("dialogueText").objectReferenceValue = dialogueTMP;
        serializedManager.FindProperty("continuePrompt").objectReferenceValue = continuePrompt;
        serializedManager.ApplyModifiedProperties();

        dialoguePanel.SetActive(false);
    }

    private void CreateGrandpaCallTriggerInScene()
    {
        GameObject callTrigger = new GameObject("GrandpaCallTrigger");
        GrandpaCallTrigger trigger = callTrigger.AddComponent<GrandpaCallTrigger>();

        DialogueData callDialogue = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/GrandpaCallDialogue.asset");
        
        SerializedObject serializedTrigger = new SerializedObject(trigger);
        serializedTrigger.FindProperty("grandpaCallDialogue").objectReferenceValue = callDialogue;
        serializedTrigger.FindProperty("delayBeforeCall").floatValue = 2f;
        serializedTrigger.FindProperty("autoTriggerOnStart").boolValue = true;
        serializedTrigger.ApplyModifiedProperties();
    }

    private void CreateSceneTransitionTrigger()
    {
        GameObject triggerObj = new GameObject("StairsTransition");
        triggerObj.transform.position = new Vector3(bedroomTransitionPos.x, bedroomTransitionPos.y, 0);
        
        BoxCollider2D collider = triggerObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(2, 3);

        SceneTransitionTrigger trigger = triggerObj.AddComponent<SceneTransitionTrigger>();
        
        SerializedObject serializedTrigger = new SerializedObject(trigger);
        serializedTrigger.FindProperty("targetSceneName").stringValue = downstairsSceneName;
        serializedTrigger.FindProperty("spawnPosition").vector2Value = downstairsSpawnPosition;
        serializedTrigger.FindProperty("useSpawnPosition").boolValue = true;
        serializedTrigger.ApplyModifiedProperties();

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Quad);
        visual.name = "Visual";
        visual.transform.SetParent(triggerObj.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(2, 3, 1);
        visual.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0, 0.3f);
        DestroyImmediate(visual.GetComponent<MeshCollider>());
    }

    private void CreateQuestManager()
    {
        if (FindObjectOfType<QuestManager>() != null)
        {
            Debug.Log("[Setup] QuestManager already exists in scene");
            return;
        }

        GameObject questManagerObj = new GameObject("QuestManager");
        questManagerObj.AddComponent<QuestManager>();
        Debug.Log("[Setup] Created QuestManager");
    }

    private void CreateGrandpaNPC()
    {
        GameObject existingGrandpa = GameObject.Find("Grandpa NPC");
        if (existingGrandpa != null)
        {
            Debug.Log("[Setup] Grandpa NPC already exists in scene");
            return;
        }

        GameObject grandpaNPC = new GameObject("Grandpa NPC");
        grandpaNPC.transform.position = new Vector3(grandpaNPCPosition.x, grandpaNPCPosition.y, 0);

        SpriteRenderer spriteRenderer = grandpaNPC.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.gray;

        GameObject visualPlaceholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visualPlaceholder.name = "Visual (Replace with Sprite)";
        visualPlaceholder.transform.SetParent(grandpaNPC.transform);
        visualPlaceholder.transform.localPosition = Vector3.zero;
        visualPlaceholder.transform.localScale = new Vector3(1, 1.5f, 1);
        DestroyImmediate(visualPlaceholder.GetComponent<BoxCollider>());
        visualPlaceholder.GetComponent<MeshRenderer>().material.color = new Color(0.8f, 0.6f, 0.4f);

        DialogueQuestTrigger questTrigger = grandpaNPC.AddComponent<DialogueQuestTrigger>();

        DialogueData choresDialogue = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/GrandpaChoresDialogue.asset");
        DialogueData afterDialogue = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/GrandpaAfterChoresDialogue.asset");

        SerializedObject serializedTrigger = new SerializedObject(questTrigger);
        serializedTrigger.FindProperty("initialDialogue").objectReferenceValue = choresDialogue;
        serializedTrigger.FindProperty("afterQuestDialogue").objectReferenceValue = afterDialogue;
        serializedTrigger.FindProperty("interactionRange").floatValue = 2f;
        serializedTrigger.FindProperty("interactKey").intValue = (int)KeyCode.E;
        serializedTrigger.FindProperty("assignsQuest").boolValue = true;
        serializedTrigger.FindProperty("questID").stringValue = "grandpa_chores";
        serializedTrigger.FindProperty("questName").stringValue = "Help Grandpa";
        serializedTrigger.FindProperty("questDescription").stringValue = "Complete the chores around the house";
        
        GameObject promptUI = GameObject.Find("InteractionPrompt");
        if (promptUI != null)
        {
            serializedTrigger.FindProperty("interactionPrompt").objectReferenceValue = promptUI;
        }
        
        serializedTrigger.ApplyModifiedProperties();

        Debug.Log("[Setup] Created Grandpa NPC at " + grandpaNPCPosition);
    }

    private void CreateInteractionUI()
    {
        if (GameObject.Find("InteractionPrompt") != null)
        {
            Debug.Log("[Setup] InteractionPrompt already exists");
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            Debug.Log("[Setup] Created Canvas");
        }

        GameObject promptObj = new GameObject("InteractionPrompt");
        promptObj.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = promptObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.7f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.7f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(200, 50);

        TMPro.TextMeshProUGUI text = promptObj.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = "Press E to Talk";
        text.fontSize = 24;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.color = Color.white;

        UnityEngine.UI.Outline outline = promptObj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        promptObj.SetActive(false);

        Debug.Log("[Setup] Created InteractionPrompt UI");
    }
}
