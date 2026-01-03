using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupQuestProgression : EditorWindow
{
    [MenuItem("Tools/Setup Quest Progression System")]
    public static void SetupEverything()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Setup Quest Progression",
            "This will:\n\n" +
            "1. Create dialogue assets for Grandpa\n" +
            "2. Setup Grandpa NPC in DownstairsScene with ConditionalDialogueTrigger\n" +
            "3. Add ChoreQuestTracker to Outside scene (if it exists)\n\n" +
            "Continue?",
            "Yes", "Cancel");

        if (!confirm) return;

        DialogueData beforeChores = CreateBeforeChoresDialogue();
        DialogueData afterChores = CreateAfterChoresDialogue();

        SetupGrandpaInDownstairsScene(beforeChores, afterChores);

        SetupChoreTrackerInOutsideScene();

        EditorUtility.DisplayDialog("Success!", 
            "Quest progression system setup complete!\n\n" +
            "✓ Created dialogue assets\n" +
            "✓ Setup Grandpa NPC with ConditionalDialogueTrigger\n" +
            "✓ Quest tracking configured\n\n" +
            "Talk to Grandpa to start the quest!", 
            "OK");
    }

    private static DialogueData CreateBeforeChoresDialogue()
    {
        string path = "Assets/GrandpaBeforeChores.asset";
        
        DialogueData existing = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
        if (existing != null)
        {
            Debug.Log("[QuestSetup] GrandpaBeforeChores.asset already exists, using existing");
            return existing;
        }

        DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
        dialogue.dialogueName = "Grandpa - Before Chores";
        dialogue.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                text = "Good morning! I hope you're ready for the day.",
                speakerName = "Grandpa",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                text = "There are some chores that need doing outside.",
                speakerName = "Grandpa",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                text = "Go complete them and I'll teach you some magic to help on your journey.",
                speakerName = "Grandpa",
                typingSpeed = 0.05f
            }
        };
        dialogue.canRepeat = true;

        AssetDatabase.CreateAsset(dialogue, path);
        AssetDatabase.SaveAssets();
        Debug.Log("[QuestSetup] ✓ Created GrandpaBeforeChores.asset");
        return dialogue;
    }

    private static DialogueData CreateAfterChoresDialogue()
    {
        string path = "Assets/GrandpaAfterChores.asset";
        
        DialogueData existing = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
        if (existing != null)
        {
            Debug.Log("[QuestSetup] GrandpaAfterChores.asset already exists, using existing");
            return existing;
        }

        DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
        dialogue.dialogueName = "Grandpa - After Chores";
        dialogue.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                text = "Well done! You've completed all the chores.",
                speakerName = "Grandpa",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                text = "You've learned some powerful spells. Now it's time to put them to use.",
                speakerName = "Grandpa",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                text = "There are ancient ruins to the east. Go there and test your abilities.",
                speakerName = "Grandpa",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                text = "Be careful out there. The ruins are dangerous, but I know you can handle it.",
                speakerName = "Grandpa",
                typingSpeed = 0.05f
            }
        };
        dialogue.canRepeat = true;

        AssetDatabase.CreateAsset(dialogue, path);
        AssetDatabase.SaveAssets();
        Debug.Log("[QuestSetup] ✓ Created GrandpaAfterChores.asset");
        return dialogue;
    }

    private static void SetupGrandpaInDownstairsScene(DialogueData beforeChores, DialogueData afterChores)
    {
        string scenePath = "Assets/Scenes/DownstairsScene.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject grandpa = GameObject.Find("Grandpa NPC");
        if (grandpa == null)
        {
            grandpa = GameObject.Find("Grandpa");
        }

        if (grandpa == null)
        {
            Debug.LogWarning("[QuestSetup] Could not find Grandpa NPC in DownstairsScene! Please add ConditionalDialogueTrigger manually.");
            return;
        }

        Component oldTrigger = grandpa.GetComponent("DialogueTrigger");
        if (oldTrigger != null)
        {
            Undo.DestroyObjectImmediate(oldTrigger);
            Debug.Log("[QuestSetup] Removed old DialogueTrigger from Grandpa");
        }

        ConditionalDialogueTrigger conditionalTrigger = grandpa.GetComponent<ConditionalDialogueTrigger>();
        if (conditionalTrigger == null)
        {
            conditionalTrigger = Undo.AddComponent<ConditionalDialogueTrigger>(grandpa);
        }

        SerializedObject so = new SerializedObject(conditionalTrigger);
        
        so.FindProperty("interactKey").enumValueIndex = (int)KeyCode.E;
        so.FindProperty("interactionRange").floatValue = 2f;
        so.FindProperty("defaultDialogue").objectReferenceValue = beforeChores;

        SerializedProperty conditionalDialogues = so.FindProperty("conditionalDialogues");
        conditionalDialogues.arraySize = 1;
        
        SerializedProperty element0 = conditionalDialogues.GetArrayElementAtIndex(0);
        element0.FindPropertyRelative("conditionQuestID").stringValue = "complete_chores";
        element0.FindPropertyRelative("requireQuestCompleted").boolValue = true;
        element0.FindPropertyRelative("dialogue").objectReferenceValue = afterChores;

        so.FindProperty("startsQuest").boolValue = true;
        so.FindProperty("questID").stringValue = "complete_chores";
        so.FindProperty("questName").stringValue = "Help with Chores";
        so.FindProperty("questDescription").stringValue = "Complete the 3 chores outside";

        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[QuestSetup] ✓ Setup Grandpa NPC with ConditionalDialogueTrigger in DownstairsScene");
    }

    private static void SetupChoreTrackerInOutsideScene()
    {
        string[] possibleScenePaths = new string[]
        {
            "Assets/Scenes/OutsideScene.unity",
            "Assets/Scenes/Outside.unity",
            "Assets/Scenes/Outdoor.unity"
        };

        string foundScenePath = null;
        foreach (string path in possibleScenePaths)
        {
            if (System.IO.File.Exists(path))
            {
                foundScenePath = path;
                break;
            }
        }

        if (foundScenePath == null)
        {
            Debug.LogWarning("[QuestSetup] No Outside scene found. Create an Outside scene and add ChoreQuestTracker manually.");
            return;
        }

        Scene originalScene = SceneManager.GetActiveScene();
        Scene outsideScene = EditorSceneManager.OpenScene(foundScenePath, OpenSceneMode.Single);

        GameObject tracker = GameObject.Find("ChoreQuestTracker");
        if (tracker == null)
        {
            tracker = new GameObject("ChoreQuestTracker");
            Undo.RegisterCreatedObjectUndo(tracker, "Create ChoreQuestTracker");
        }

        ChoreQuestTracker choreTracker = tracker.GetComponent<ChoreQuestTracker>();
        if (choreTracker == null)
        {
            choreTracker = Undo.AddComponent<ChoreQuestTracker>(tracker);
        }

        SerializedObject so = new SerializedObject(choreTracker);
        so.FindProperty("questID").stringValue = "complete_chores";
        so.FindProperty("totalChoresRequired").intValue = 3;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(outsideScene);
        EditorSceneManager.SaveScene(outsideScene);

        Debug.Log($"[QuestSetup] ✓ Setup ChoreQuestTracker in {foundScenePath}");
    }
}
