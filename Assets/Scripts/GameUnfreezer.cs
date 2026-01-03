using UnityEngine;

public class GameUnfreezer : MonoBehaviour
{
    [Header("Emergency Controls")]
    [SerializeField] private KeyCode unfreezeKey = KeyCode.F1;
    
    private void Update()
    {
        if (Input.GetKeyDown(unfreezeKey))
        {
            UnfreezeGame();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShowGameStatus();
        }
    }

    public void UnfreezeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("[GameUnfreezer] Game unfrozen! Time.timeScale = 1");
        
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            Debug.Log("[GameUnfreezer] DialogueManager found, attempting to close dialogue");
        }
    }

    private void ShowGameStatus()
    {
        Debug.Log($"=== GAME STATUS ===");
        Debug.Log($"Time.timeScale: {Time.timeScale}");
        Debug.Log($"DialogueManager exists: {FindObjectOfType<DialogueManager>() != null}");
        
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        if (dm != null)
        {
            Debug.Log($"Dialogue active: {dm.IsDialogueActive()}");
        }
    }

    [ContextMenu("Force Unfreeze")]
    public void ForceUnfreeze()
    {
        UnfreezeGame();
    }
}
