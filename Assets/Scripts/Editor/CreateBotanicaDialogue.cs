using UnityEditor;
using UnityEngine;

public class CreateBotanicaDialogue
{
    [MenuItem("Tools/Story Progression/1. Create Botanica Dialogue")]
    public static void CreateDialogueAssets()
    {
        bool initial = CreateInitialDialogue();
        bool final = CreateFinalDialogue();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (initial || final)
        {
            string message = "Created dialogue assets:\n\n";
            if (initial) message += "✓ Assets/BotanicaInitialDialogue.asset\n";
            if (final) message += "✓ Assets/BotanicaFinalDialogue.asset\n";
            message += "\nNext: Open UndergroundRuins scene and add Botanica manually.";

            EditorUtility.DisplayDialog("Botanica Dialogue Created!", message, "OK");

            Object asset = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/BotanicaInitialDialogue.asset");
            if (asset != null)
            {
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Already Exists", "Dialogue assets already exist!\n\nCheck Assets folder for:\n- BotanicaInitialDialogue.asset\n- BotanicaFinalDialogue.asset", "OK");
        }
    }

    private static bool CreateInitialDialogue()
    {
        string path = "Assets/BotanicaInitialDialogue.asset";

        if (AssetDatabase.LoadAssetAtPath<DialogueData>(path) != null)
        {
            Debug.Log("[BotanicaDialogue] Initial dialogue already exists at " + path);
            return false;
        }

        DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();

        dialogue.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "At last... a worthy soul descends into my domain.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "I am Botanica, Guardian of Nature's Power.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "These ruins hold ancient magic... and great danger.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "Prove your strength by conquering the beast that dwells within.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "I will be watching...",
                typingSpeed = 0.05f
            }
        };

        AssetDatabase.CreateAsset(dialogue, path);
        Debug.Log($"[BotanicaDialogue] ✓ Created {path}");
        return true;
    }

    private static bool CreateFinalDialogue()
    {
        string path = "Assets/BotanicaFinalDialogue.asset";

        if (AssetDatabase.LoadAssetAtPath<DialogueData>(path) != null)
        {
            Debug.Log("[BotanicaDialogue] Final dialogue already exists at " + path);
            return false;
        }

        DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();

        dialogue.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "Impressive... You have proven yourself worthy.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "The guardian has fallen, and you grow stronger.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "Your heart expands with newfound vitality.",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "But this is only the beginning of your journey...",
                typingSpeed = 0.05f
            },
            new DialogueLine
            {
                speakerName = "Botanica",
                text = "Return to your village. Greater trials await you.",
                typingSpeed = 0.05f
            }
        };

        AssetDatabase.CreateAsset(dialogue, path);
        Debug.Log($"[BotanicaDialogue] ✓ Created {path}");
        return true;
    }
}
