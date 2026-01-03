using UnityEngine;
using UnityEditor;

public class CreateGrandpaDialogue : MonoBehaviour
{
    [MenuItem("Dialogue/Create Grandpa Call Dialogue")]
    private static void CreateGrandpaCallDialogue()
    {
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

        string assetPath = "Assets/GrandpaCallDialogue.asset";
        AssetDatabase.CreateAsset(dialogue, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = dialogue;

        Debug.Log($"Created Grandpa Call Dialogue at {assetPath}");
    }
}
