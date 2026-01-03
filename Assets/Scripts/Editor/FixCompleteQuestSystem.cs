using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FixCompleteQuestSystem : EditorWindow
{
    [MenuItem("Tools/Fix Complete Quest System NOW")]
    public static void FixEverything()
    {
        const string QUEST_ID = "complete_chores";
        const string QUEST_NAME = "Help with Chores";
        const string QUEST_DESCRIPTION = "Complete all the chores around the house";

        Scene activeScene = SceneManager.GetActiveScene();
        string originalScenePath = activeScene.path;

        Debug.Log("=== FIXING QUEST SYSTEM ===");

        FixDownstairsScene(QUEST_ID, QUEST_NAME, QUEST_DESCRIPTION);
        FixOutdoorsScene(QUEST_ID);

        if (!string.IsNullOrEmpty(originalScenePath) && System.IO.File.Exists(originalScenePath))
        {
            EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
        }

        EditorUtility.DisplayDialog("Success!", 
            "Quest system fixed!\n\n✅ Grandpa quest ID: 'complete_chores'\n✅ ChoreQuestTracker added to Outdoors\n✅ All components configured\n\nRestart Play mode to test!", 
            "OK");
    }

    static void FixDownstairsScene(string questID, string questName, string questDescription)
    {
        string scenePath = "Assets/Scenes/DownstairsScene.unity";
        if (!System.IO.File.Exists(scenePath))
        {
            Debug.LogWarning($"[FixQuest] Scene not found: {scenePath}");
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        Debug.Log($"[FixQuest] Opened scene: {scene.name}");

        DialogueQuestTrigger[] questTriggers = GameObject.FindObjectsOfType<DialogueQuestTrigger>();
        foreach (DialogueQuestTrigger trigger in questTriggers)
        {
            SerializedObject so = new SerializedObject(trigger);
            SerializedProperty questIDProp = so.FindProperty("questID");
            SerializedProperty questNameProp = so.FindProperty("questName");
            SerializedProperty questDescProp = so.FindProperty("questDescription");

            questIDProp.stringValue = questID;
            questNameProp.stringValue = questName;
            questDescProp.stringValue = questDescription;

            so.ApplyModifiedProperties();
            Debug.Log($"[FixQuest] ✓ Updated {trigger.gameObject.name} quest ID to '{questID}'");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[FixQuest] ✓ Saved DownstairsScene");
    }

    static void FixOutdoorsScene(string questID)
    {
        string scenePath = "Assets/Scenes/Outdoors.unity";
        if (!System.IO.File.Exists(scenePath))
        {
            Debug.LogWarning($"[FixQuest] Scene not found: {scenePath}");
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        Debug.Log($"[FixQuest] Opened scene: {scene.name}");

        ChoreQuestTracker existingTracker = GameObject.FindObjectOfType<ChoreQuestTracker>();
        
        if (existingTracker == null)
        {
            GameObject trackerObj = new GameObject("ChoreQuestTracker");
            Undo.RegisterCreatedObjectUndo(trackerObj, "Create ChoreQuestTracker");
            
            ChoreQuestTracker tracker = trackerObj.AddComponent<ChoreQuestTracker>();
            
            SerializedObject so = new SerializedObject(tracker);
            so.FindProperty("questID").stringValue = questID;
            so.FindProperty("totalChoresRequired").intValue = 3;
            so.ApplyModifiedProperties();

            Debug.Log($"[FixQuest] ✓ Created ChoreQuestTracker with quest ID '{questID}'");
        }
        else
        {
            SerializedObject so = new SerializedObject(existingTracker);
            so.FindProperty("questID").stringValue = questID;
            so.FindProperty("totalChoresRequired").intValue = 3;
            so.ApplyModifiedProperties();

            Debug.Log($"[FixQuest] ✓ Updated existing ChoreQuestTracker to quest ID '{questID}'");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[FixQuest] ✓ Saved Outdoors scene");
    }
}
