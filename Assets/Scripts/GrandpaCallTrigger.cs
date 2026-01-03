using System.Collections;
using UnityEngine;

public class GrandpaCallTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData grandpaCallDialogue;
    [SerializeField] private float delayBeforeCall = 2f;
    
    [Header("Auto-Start")]
    [SerializeField] private bool autoTriggerOnStart = true;

    private bool hasTriggered = false;

    private void Start()
    {
        if (autoTriggerOnStart)
        {
            StartCoroutine(TriggerGrandpaCall());
        }
    }

    private IEnumerator TriggerGrandpaCall()
    {
        yield return new WaitForSeconds(delayBeforeCall);

        if (!hasTriggered && grandpaCallDialogue != null)
        {
            TriggerCall();
        }
    }

    public void TriggerCall()
    {
        if (DialogueManager.Instance != null && grandpaCallDialogue != null)
        {
            hasTriggered = true;
            DialogueManager.Instance.StartDialogue(grandpaCallDialogue);
        }
        else
        {
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("[GrandpaCallTrigger] DialogueManager not found in scene!");
            }
            if (grandpaCallDialogue == null)
            {
                Debug.LogError("[GrandpaCallTrigger] Grandpa dialogue data not assigned!");
            }
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
