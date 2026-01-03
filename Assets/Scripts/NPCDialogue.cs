using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool hasBeenTriggered = false;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;

    private Transform player;
    private bool playerInRange = false;

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
        if (player == null || dialogue == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(playerInRange && !DialogueManager.Instance.IsDialogueActive());
        }

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (!DialogueManager.Instance.IsDialogueActive())
            {
                if (!hasBeenTriggered || dialogue.canRepeat)
                {
                    TriggerDialogue();
                }
            }
        }
    }

    private void TriggerDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            hasBeenTriggered = true;
        }
        else
        {
            Debug.LogError("[NPCDialogue] DialogueManager not found in scene!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    public void SetDialogue(DialogueData newDialogue)
    {
        dialogue = newDialogue;
    }

    public void ResetDialogue()
    {
        hasBeenTriggered = false;
    }
}
