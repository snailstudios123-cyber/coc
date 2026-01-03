using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [System.Serializable]
    public class Quest
    {
        public string questID;
        public string questName;
        public string description;
        public bool isActive;
        public bool isCompleted;
        public int currentProgress;
        public int requiredProgress;
    }

    [SerializeField] private List<Quest> activeQuests = new List<Quest>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddQuest(string questID, string questName, string description, int requiredProgress = 0)
    {
        Quest existingQuest = activeQuests.Find(q => q.questID == questID);
        if (existingQuest != null)
        {
            Debug.LogWarning($"[QuestManager] Quest '{questID}' already exists!");
            return;
        }

        Quest newQuest = new Quest
        {
            questID = questID,
            questName = questName,
            description = description,
            isActive = true,
            isCompleted = false,
            currentProgress = 0,
            requiredProgress = requiredProgress
        };

        activeQuests.Add(newQuest);
        Debug.Log($"[QuestManager] Quest added: {questName}");
    }

    public void IncrementQuestProgress(string questID, int amount = 1)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        if (quest == null)
        {
            Debug.LogWarning($"[QuestManager] Quest '{questID}' not found for progress increment!");
            return;
        }

        if (quest.isCompleted)
        {
            Debug.Log($"[QuestManager] Quest '{questID}' is already completed!");
            return;
        }

        quest.currentProgress += amount;
        Debug.Log($"[QuestManager] Quest '{quest.questName}' progress: {quest.currentProgress}/{quest.requiredProgress}");

        if (quest.requiredProgress > 0 && quest.currentProgress >= quest.requiredProgress)
        {
            CompleteQuest(questID);
        }
    }

    public void CompleteQuest(string questID)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        if (quest != null)
        {
            quest.isCompleted = true;
            quest.isActive = false;
            Debug.Log($"[QuestManager] Quest completed: {quest.questName}");
        }
        else
        {
            Debug.LogWarning($"[QuestManager] Quest '{questID}' not found!");
        }
    }

    public bool IsQuestActive(string questID)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        return quest != null && quest.isActive;
    }

    public bool IsQuestCompleted(string questID)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        return quest != null && quest.isCompleted;
    }

    public Quest GetQuest(string questID)
    {
        return activeQuests.Find(q => q.questID == questID);
    }

    public List<Quest> GetActiveQuests()
    {
        return activeQuests.FindAll(q => q.isActive);
    }
}
