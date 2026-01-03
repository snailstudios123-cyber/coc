using UnityEngine;

public class ConditionalDialogueTrigger : MonoBehaviour
{
    [System.Serializable]
    public class ConditionalDialogue
    {
        public string conditionQuestID;
        public bool requireQuestCompleted = true;
        public DialogueData dialogue;
    }

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactionRange = 2f;

    [Header("Dialogues")]
    [SerializeField] private DialogueData defaultDialogue;
    [SerializeField] private ConditionalDialogue[] conditionalDialogues;

    [Header("Quest")]
    [SerializeField] private bool startsQuest = false;
    [SerializeField] private string questID;
    [SerializeField] private string questName;
    [SerializeField] private string questDescription;

    private bool playerInRange = false;
    private Transform playerTransform;
    private bool wasDialogueActiveLastFrame = false;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        playerInRange = distance <= interactionRange;

        bool dialogueCurrentlyActive = DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive();

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (dialogueCurrentlyActive || wasDialogueActiveLastFrame)
            {
                wasDialogueActiveLastFrame = dialogueCurrentlyActive;
                return;
            }
            
            TriggerDialogue();
        }

        wasDialogueActiveLastFrame = dialogueCurrentlyActive;
    }

    private void TriggerDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[ConditionalDialogueTrigger] DialogueManager.Instance is null!");
            return;
        }

        DialogueData dialogueToPlay = GetAppropriateDialogue();

        if (dialogueToPlay != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToPlay);

            if (startsQuest && QuestManager.Instance != null && !string.IsNullOrEmpty(questID))
            {
                if (!QuestManager.Instance.IsQuestActive(questID) && !QuestManager.Instance.IsQuestCompleted(questID))
                {
                    QuestManager.Instance.AddQuest(questID, questName, questDescription);
                }
            }
        }
    }

    private DialogueData GetAppropriateDialogue()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("[ConditionalDialogueTrigger] QuestManager.Instance is null!");
            return defaultDialogue;
        }

        Debug.Log($"[ConditionalDialogueTrigger] Checking {conditionalDialogues.Length} conditional dialogues");

        foreach (ConditionalDialogue conditional in conditionalDialogues)
        {
            if (conditional.dialogue == null || string.IsNullOrEmpty(conditional.conditionQuestID))
            {
                Debug.LogWarning("[ConditionalDialogueTrigger] Skipping conditional - dialogue or questID is null/empty");
                continue;
            }

            bool conditionMet = false;

            if (conditional.requireQuestCompleted)
            {
                conditionMet = QuestManager.Instance.IsQuestCompleted(conditional.conditionQuestID);
                Debug.Log($"[ConditionalDialogueTrigger] Quest '{conditional.conditionQuestID}' completed? {conditionMet}");
            }
            else
            {
                conditionMet = QuestManager.Instance.IsQuestActive(conditional.conditionQuestID);
                Debug.Log($"[ConditionalDialogueTrigger] Quest '{conditional.conditionQuestID}' active? {conditionMet}");
            }

            if (conditionMet)
            {
                Debug.Log($"[ConditionalDialogueTrigger] ✓ Using conditional dialogue: {conditional.dialogue.dialogueName}");
                return conditional.dialogue;
            }
        }

        Debug.Log($"[ConditionalDialogueTrigger] No conditions met, using default: {(defaultDialogue != null ? defaultDialogue.dialogueName : "NULL")}");
        return defaultDialogue;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
