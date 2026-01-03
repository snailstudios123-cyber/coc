using UnityEngine;
using System;

public class ChoreInteractable : MonoBehaviour
{
    public event Action OnChoreCompleted;

    [Header("Chore Settings")]
    [SerializeField] private string choreName = "Chore";
    [SerializeField] private SpellData spellReward;
    
    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactionRange = 2f;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color completedColor = Color.gray;
    
    private bool isCompleted = false;
    private bool playerInRange = false;
    private Transform playerTransform;
    private Color originalColor;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        ChoreQuestTracker tracker = FindObjectOfType<ChoreQuestTracker>();
        if (tracker != null)
        {
            tracker.RegisterChore(this);
            Debug.Log($"[ChoreInteractable] '{choreName}' registered with ChoreQuestTracker");
        }
        else
        {
            Debug.LogWarning($"[ChoreInteractable] ChoreQuestTracker not found in scene for '{choreName}'");
        }
    }

    private void Update()
    {
        if (isCompleted || playerTransform == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        playerInRange = distance <= interactionRange;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(playerInRange);
        }

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            CompleteChore();
        }
    }

    private void CompleteChore()
    {
        if (isCompleted)
        {
            return;
        }

        isCompleted = true;

        if (spellReward == null)
        {
            Debug.LogError($"[ChoreInteractable] '{choreName}' has no spell reward assigned!");
            return;
        }

        if (SpellManager.Instance == null)
        {
            Debug.LogError($"[ChoreInteractable] SpellManager.Instance is null! Make sure SpellManager exists in scene.");
            return;
        }

        Debug.Log($"[ChoreInteractable] Attempting to learn spell: {spellReward.spellName}");
        
        SpellManager.Instance.LearnAndEquipSpell(spellReward);
        
        Debug.Log($"[ChoreInteractable] ✓ Completed '{choreName}'!");

        if (spriteRenderer != null)
        {
            spriteRenderer.color = completedColor;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        gameObject.tag = "Untagged";

        OnChoreCompleted?.Invoke();

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.IncrementQuestProgress("complete_chores");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
