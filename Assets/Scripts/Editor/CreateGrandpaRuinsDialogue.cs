using UnityEngine;
using UnityEditor;

public class CreateGrandpaRuinsDialogue : EditorWindow
{
    [MenuItem("Tools/Create Grandpa Ruins Dialogues")]
    public static void CreateDialogues()
    {
        CreateBeforeChoresDialogue();
        CreateAfterChoresDialogue();
        
        EditorUtility.DisplayDialog("Success!", 
            "Created 2 Grandpa dialogue assets:\n\n" +
            "1. GrandpaBeforeChores.asset\n   - Initial dialogue about doing chores\n\n" +
            "2. GrandpaAfterChores.asset\n   - Tells you to go to the ruins\n\n" +
            "Assign these to Grandpa's ConditionalDialogueTrigger component!", 
            "OK");
    }

    private static void CreateBeforeChoresDialogue()
    {
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

        AssetDatabase.CreateAsset(dialogue, "Assets/GrandpaBeforeChores.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✓ Created GrandpaBeforeChores.asset");
    }

    private static void CreateAfterChoresDialogue()
    {
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

        AssetDatabase.CreateAsset(dialogue, "Assets/GrandpaAfterChores.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✓ Created GrandpaAfterChores.asset");
    }
}
