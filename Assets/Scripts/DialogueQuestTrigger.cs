using UnityEngine;
using UnityEngine.Events;

public class DialogueQuestTrigger : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private DialogueData initialDialogue;
    [SerializeField] private DialogueData afterQuestDialogue;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Quest Settings")]
    [SerializeField] private bool assignsQuest = false;
    [SerializeField] private string questID;
    [SerializeField] private string questName;
    [SerializeField] private string questDescription;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueComplete;
    [SerializeField] private UnityEvent onQuestAssigned;

    private Transform player;
    private bool playerInRange = false;
    private bool hasGivenQuest = false;
    private bool isCurrentlyTalking = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null || initialDialogue == null) return;
        
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[DialogueQuestTrigger] DialogueManager.Instance is null!");
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        if (interactionPrompt != null)
        {
            bool showPrompt = playerInRange && 
                            !DialogueManager.Instance.IsDialogueActive() && 
                            !isCurrentlyTalking;
            interactionPrompt.SetActive(showPrompt);
        }

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (!DialogueManager.Instance.IsDialogueActive() && !isCurrentlyTalking)
            {
                StartInteraction();
            }
        }

        if (isCurrentlyTalking && !DialogueManager.Instance.IsDialogueActive())
        {
            OnDialogueEnded();
        }
    }

    private void StartInteraction()
    {
        isCurrentlyTalking = true;

        DialogueData dialogueToUse = initialDialogue;
        
        bool questWasGiven = hasGivenQuest || (QuestManager.Instance != null && 
                                                 !string.IsNullOrEmpty(questID) && 
                                                 (QuestManager.Instance.IsQuestActive(questID) || 
                                                  QuestManager.Instance.IsQuestCompleted(questID)));
        
        if (questWasGiven && afterQuestDialogue != null)
        {
            if (QuestManager.Instance != null && !string.IsNullOrEmpty(questID))
            {
                bool questCompleted = QuestManager.Instance.IsQuestCompleted(questID);
                Debug.Log($"[DialogueQuestTrigger] Quest '{questID}' completed: {questCompleted}");
                
                if (questCompleted)
                {
                    dialogueToUse = afterQuestDialogue;
                    Debug.Log($"[DialogueQuestTrigger] Using after-quest dialogue: {afterQuestDialogue.dialogueName}");
                }
                else
                {
                    Debug.Log($"[DialogueQuestTrigger] Quest not complete, using initial dialogue");
                }
            }
            else
            {
                dialogueToUse = afterQuestDialogue;
            }
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToUse);
        }
    }

    private void OnDialogueEnded()
    {
        isCurrentlyTalking = false;
        
        onDialogueComplete?.Invoke();

        if (assignsQuest && !hasGivenQuest)
        {
            AssignQuest();
        }
    }

    private void AssignQuest()
    {
        if (QuestManager.Instance != null)
        {
            int requiredProgress = (questID == "complete_chores") ? 3 : 0;
            QuestManager.Instance.AddQuest(questID, questName, questDescription, requiredProgress);
            hasGivenQuest = true;
            onQuestAssigned?.Invoke();
        }
        else
        {
            Debug.LogWarning("[DialogueQuestTrigger] QuestManager not found!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    public void SetHasGivenQuest(bool value)
    {
        hasGivenQuest = value;
    }

    public void ResetNPC()
    {
        hasGivenQuest = false;
        isCurrentlyTalking = false;
    }
}
