using UnityEngine;
using UnityEditor;

public class CreateGrandpaChoresDialogue
{
    [MenuItem("Dialogue/Create Grandpa Chores Dialogue")]
    public static void CreateChoresDialogue()
    {
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

        string path = "Assets/GrandpaChoresDialogue.asset";
        AssetDatabase.CreateAsset(dialogue, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = dialogue;

        Debug.Log($"[DialogueTool] Created Grandpa Chores dialogue at: {path}");
    }

    [MenuItem("Dialogue/Create Grandpa After Quest Dialogue")]
    public static void CreateAfterQuestDialogue()
    {
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

        string path = "Assets/GrandpaAfterChoresDialogue.asset";
        AssetDatabase.CreateAsset(dialogue, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = dialogue;

        Debug.Log($"[DialogueTool] Created Grandpa After Chores dialogue at: {path}");
    }
}
