using UnityEngine;
using System.Collections.Generic;

public class ChoreQuestTracker : MonoBehaviour
{
    [SerializeField] private string questID = "complete_chores";
    [SerializeField] private int totalChoresRequired = 3;
    
    private int choresCompleted = 0;
    private HashSet<ChoreInteractable> registeredChores = new HashSet<ChoreInteractable>();

    public void RegisterChore(ChoreInteractable chore)
    {
        if (registeredChores.Add(chore))
        {
            chore.OnChoreCompleted += OnChoreCompleted;
            Debug.Log($"[ChoreQuestTracker] Registered chore. Total registered: {registeredChores.Count}");
        }
    }

    private void OnChoreCompleted()
    {
        choresCompleted++;
        Debug.Log($"[ChoreQuestTracker] Chores completed: {choresCompleted}/{totalChoresRequired}");

        if (choresCompleted >= totalChoresRequired)
        {
            if (QuestManager.Instance == null)
            {
                Debug.LogError("[ChoreQuestTracker] QuestManager.Instance is NULL! Cannot complete quest.");
                return;
            }

            Debug.Log($"[ChoreQuestTracker] All chores completed! Attempting to complete quest '{questID}'");
            QuestManager.Instance.CompleteQuest(questID);
            
            bool isCompleted = QuestManager.Instance.IsQuestCompleted(questID);
            Debug.Log($"[ChoreQuestTracker] Quest '{questID}' completed status: {isCompleted}");
        }
    }

    private void OnDestroy()
    {
        foreach (ChoreInteractable chore in registeredChores)
        {
            if (chore != null)
            {
                chore.OnChoreCompleted -= OnChoreCompleted;
            }
        }
    }
}
