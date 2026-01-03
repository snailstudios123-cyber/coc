using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class DiagnoseQuestSystem : EditorWindow
{
    [MenuItem("Tools/Diagnose Quest System")]
    public static void Diagnose()
    {
        string report = "=== QUEST SYSTEM DIAGNOSTIC ===\n\n";
        
        Scene activeScene = SceneManager.GetActiveScene();
        string originalScene = activeScene.path;

        string[] scenesToCheck = new string[]
        {
            "Assets/Scenes/DownstairsScene.unity",
            "Assets/Scenes/OutsideScene.unity",
            "Assets/Scenes/Outside.unity",
            "Assets/Scenes/Outdoor.unity"
        };

        foreach (string scenePath in scenesToCheck)
        {
            if (!System.IO.File.Exists(scenePath)) continue;

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            report += $"📁 SCENE: {scene.name}\n";
            report += "─────────────────────────────\n";

            DialogueQuestTrigger[] questTriggers = GameObject.FindObjectsOfType<DialogueQuestTrigger>();
            report += $"DialogueQuestTrigger components: {questTriggers.Length}\n";
            foreach (DialogueQuestTrigger trigger in questTriggers)
            {
                SerializedObject so = new SerializedObject(trigger);
                string questID = so.FindProperty("questID").stringValue;
                string questName = so.FindProperty("questName").stringValue;
                bool assignsQuest = so.FindProperty("assignsQuest").boolValue;
                
                report += $"  • {trigger.gameObject.name}\n";
                report += $"    Quest ID: '{questID}'\n";
                report += $"    Quest Name: '{questName}'\n";
                report += $"    Assigns Quest: {assignsQuest}\n";
            }

            ChoreQuestTracker[] choreTrackers = GameObject.FindObjectsOfType<ChoreQuestTracker>();
            report += $"\nChoreQuestTracker components: {choreTrackers.Length}\n";
            foreach (ChoreQuestTracker tracker in choreTrackers)
            {
                SerializedObject so = new SerializedObject(tracker);
                string questID = so.FindProperty("questID").stringValue;
                int totalRequired = so.FindProperty("totalChoresRequired").intValue;
                
                report += $"  • {tracker.gameObject.name}\n";
                report += $"    Quest ID: '{questID}'\n";
                report += $"    Total Required: {totalRequired}\n";
            }

            ChoreInteractable[] chores = GameObject.FindObjectsOfType<ChoreInteractable>();
            report += $"\nChoreInteractable components: {chores.Length}\n";
            foreach (ChoreInteractable chore in chores)
            {
                report += $"  • {chore.gameObject.name}\n";
            }

            report += "\n";
        }

        if (!string.IsNullOrEmpty(originalScene))
        {
            EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
        }

        report += "\n=== RECOMMENDATIONS ===\n";
        report += "1. Make sure ChoreQuestTracker exists in Outside scene\n";
        report += "2. Quest IDs should match: 'complete_chores'\n";
        report += "3. ChoreQuestTracker should track same number as chores\n";

        Debug.Log(report);

        EditorUtility.DisplayDialog("Quest System Diagnostic", 
            "Diagnostic complete! Check the Console for full report.", 
            "OK");
    }
}
