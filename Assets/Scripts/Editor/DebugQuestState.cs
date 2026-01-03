using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class DebugQuestState : EditorWindow
{
    [MenuItem("Tools/Debug Quest State (Play Mode Only)")]
    public static void ShowWindow()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Error", "This tool only works in Play Mode!\n\nEnter Play Mode first.", "OK");
            return;
        }

        string report = "=== QUEST STATE DEBUG ===\n\n";

        if (QuestManager.Instance == null)
        {
            report += "❌ QuestManager.Instance is NULL!\n";
        }
        else
        {
            report += "✅ QuestManager.Instance exists\n\n";
            
            report += "QUEST: 'complete_chores'\n";
            bool isActive = QuestManager.Instance.IsQuestActive("complete_chores");
            bool isCompleted = QuestManager.Instance.IsQuestCompleted("complete_chores");
            report += $"  - Active: {isActive}\n";
            report += $"  - Completed: {isCompleted}\n\n";
        }

        ChoreQuestTracker tracker = GameObject.FindObjectOfType<ChoreQuestTracker>();
        if (tracker == null)
        {
            report += "❌ NO ChoreQuestTracker in scene!\n";
        }
        else
        {
            report += "✅ ChoreQuestTracker found\n";
            report += $"  Scene: {tracker.gameObject.scene.name}\n\n";
        }

        ChoreInteractable[] chores = GameObject.FindObjectsOfType<ChoreInteractable>();
        report += $"ChoreInteractable count: {chores.Length}\n";
        foreach (ChoreInteractable chore in chores)
        {
            report += $"  - {chore.gameObject.name} in scene '{chore.gameObject.scene.name}'\n";
        }
        report += "\n";

        DialogueQuestTrigger[] triggers = GameObject.FindObjectsOfType<DialogueQuestTrigger>();
        report += $"DialogueQuestTrigger count: {triggers.Length}\n";
        foreach (DialogueQuestTrigger trigger in triggers)
        {
            report += $"  - {trigger.gameObject.name} in scene '{trigger.gameObject.scene.name}'\n";
        }

        Debug.Log(report);
        EditorUtility.DisplayDialog("Quest State", report, "OK");
    }
}
