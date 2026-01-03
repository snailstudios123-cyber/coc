using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class FixGrandpaDialogue : EditorWindow
{
    [MenuItem("Tools/Quest System/Fix Grandpa Dialogue (Use Ruins Dialogue)")]
    public static void ShowWindow()
    {
        FixDialogue();
    }

    private static void FixDialogue()
    {
        string scenePath = "Assets/Scenes/DownstairsScene.unity";
        UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject grandpa = GameObject.Find("Grandpa NPC");
        if (grandpa == null)
        {
            Debug.LogError("[FixGrandpaDialogue] Could not find 'Grandpa NPC' in DownstairsScene!");
            return;
        }

        DialogueQuestTrigger trigger = grandpa.GetComponent<DialogueQuestTrigger>();
        if (trigger == null)
        {
            Debug.LogError("[FixGrandpaDialogue] Grandpa NPC does not have DialogueQuestTrigger component!");
            return;
        }

        DialogueData ruinsDialogue = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/GrandpaAfterChores.asset");
        if (ruinsDialogue == null)
        {
            Debug.LogError("[FixGrandpaDialogue] Could not load 'GrandpaAfterChores.asset'!");
            return;
        }

        SerializedObject so = new SerializedObject(trigger);
        SerializedProperty afterQuestProp = so.FindProperty("afterQuestDialogue");
        afterQuestProp.objectReferenceValue = ruinsDialogue;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log($"[FixGrandpaDialogue] ✓ Updated Grandpa's after-quest dialogue to '{ruinsDialogue.dialogueName}'");
        Debug.Log($"[FixGrandpaDialogue] ✓ Scene saved. Restart Play mode to test!");
        
        EditorUtility.DisplayDialog(
            "Grandpa Dialogue Fixed!", 
            $"Grandpa will now say:\n\n" +
            $"• \"You've completed all the chores.\"\n" +
            $"• \"There are ancient ruins to the east.\"\n" +
            $"• \"Go there and test your abilities.\"\n\n" +
            $"Scene saved! Restart Play mode to test.", 
            "OK"
        );
    }
}
