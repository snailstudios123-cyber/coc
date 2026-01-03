using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FixQuestIDs : EditorWindow
{
    [MenuItem("Tools/Fix Quest ID Mismatch")]
    public static void FixQuests()
    {
        const string QUEST_ID = "complete_chores";
        
        bool madeChanges = false;

        Scene activeScene = SceneManager.GetActiveScene();
        string originalScenePath = activeScene.path;

        string[] scenePaths = new string[]
        {
            "Assets/Scenes/DownstairsScene.unity",
            "Assets/Scenes/OutsideScene.unity",
            "Assets/Scenes/Outside.unity"
        };

        foreach (string scenePath in scenePaths)
        {
            if (!System.IO.File.Exists(scenePath)) continue;

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Debug.Log($"[FixQuestIDs] Checking scene: {scenePath}");

            DialogueQuestTrigger[] questTriggers = GameObject.FindObjectsOfType<DialogueQuestTrigger>();
            foreach (DialogueQuestTrigger trigger in questTriggers)
            {
                SerializedObject so = new SerializedObject(trigger);
                SerializedProperty questIDProp = so.FindProperty("questID");
                
                if (questIDProp.stringValue != QUEST_ID && !string.IsNullOrEmpty(questIDProp.stringValue))
                {
                    Debug.Log($"[FixQuestIDs] Updating DialogueQuestTrigger on {trigger.gameObject.name}: '{questIDProp.stringValue}' → '{QUEST_ID}'");
                    questIDProp.stringValue = QUEST_ID;
                    so.ApplyModifiedProperties();
                    EditorSceneManager.MarkSceneDirty(scene);
                    madeChanges = true;
                }
            }

            ChoreQuestTracker[] choreTrackers = GameObject.FindObjectsOfType<ChoreQuestTracker>();
            foreach (ChoreQuestTracker tracker in choreTrackers)
            {
                SerializedObject so = new SerializedObject(tracker);
                SerializedProperty questIDProp = so.FindProperty("questID");
                
                if (questIDProp.stringValue != QUEST_ID)
                {
                    Debug.Log($"[FixQuestIDs] Updating ChoreQuestTracker: '{questIDProp.stringValue}' → '{QUEST_ID}'");
                    questIDProp.stringValue = QUEST_ID;
                    so.ApplyModifiedProperties();
                    EditorSceneManager.MarkSceneDirty(scene);
                    madeChanges = true;
                }
            }

            if (EditorSceneManager.GetSceneManagerSetup().Length > 0)
            {
                EditorSceneManager.SaveScene(scene);
            }
        }

        if (!string.IsNullOrEmpty(originalScenePath) && System.IO.File.Exists(originalScenePath))
        {
            EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
        }

        if (madeChanges)
        {
            EditorUtility.DisplayDialog("Success!", 
                $"Fixed quest IDs!\n\nAll DialogueQuestTrigger and ChoreQuestTracker components now use quest ID: '{QUEST_ID}'\n\nRestart Play mode to test.", 
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", 
                "No quest ID mismatches found. Everything looks good!", 
                "OK");
        }
    }
}
