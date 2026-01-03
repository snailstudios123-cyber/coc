using System.Collections;
using UnityEngine;

public class StatueCutsceneTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject statueObject;
    [SerializeField] private ParticleSystem glowEffect;
    [SerializeField] private ParticleSystem groundBreakEffect;
    [SerializeField] private GameObject groundCrackSprite;
    
    [Header("Camera Shake")]
    [SerializeField] private float shakeIntensity = 0.3f;
    [SerializeField] private float shakeDuration = 1.5f;
    
    [Header("Cutscene Timing")]
    [SerializeField] private float groundBreakDuration = 1.5f;
    
    [Header("Scene Transition")]
    [SerializeField] private string undergroundSceneName = "UndergroundRuins";
    [SerializeField] private Vector2 undergroundSpawnPosition = new Vector2(0, 0);
    
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactPrompt;
    
    private bool cutscenePlayed = false;
    private bool playerInRange = false;
    private Transform playerTransform;
    
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        if (glowEffect != null)
        {
            glowEffect.Play();
        }
        
        if (groundBreakEffect != null)
        {
            groundBreakEffect.Stop();
        }
        
        if (groundCrackSprite != null)
        {
            groundCrackSprite.SetActive(false);
        }
        
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (cutscenePlayed || playerTransform == null) return;
        
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        playerInRange = distance <= interactionRange;
        
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(playerInRange);
        }
        
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            StartCutscene();
        }
    }
    
    private void StartCutscene()
    {
        if (cutscenePlayed) return;
        
        cutscenePlayed = true;
        Debug.Log("[StatueCutscene] Starting statue cutscene");
        
        StartCoroutine(PlayCutsceneSequence());
    }
    
    private IEnumerator PlayCutsceneSequence()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
        
        DisablePlayerControl();
        
        Debug.Log("[StatueCutscene] Player interacted with statue - ground breaking!");
        
        if (groundBreakEffect != null)
        {
            groundBreakEffect.Play();
        }
        
        if (groundCrackSprite != null)
        {
            groundCrackSprite.SetActive(true);
        }
        
        StartCoroutine(ShakeCamera());
        
        yield return new WaitForSeconds(groundBreakDuration);
        
        Debug.Log("[StatueCutscene] Falling underground - transitioning to scene");
        TransitionToUnderground();
    }
    
    private IEnumerator ShakeCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) yield break;
        
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            
            mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.transform.localPosition = originalPosition;
    }
    
    private void DisablePlayerControl()
    {
        if (playerTransform == null) return;
        
        PlayerController playerController = playerTransform.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        Debug.Log("[StatueCutscene] Player control disabled for cutscene");
    }
    
    private void TransitionToUnderground()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(undergroundSceneName, undergroundSpawnPosition);
        }
        else
        {
            Debug.LogError("[StatueCutscene] SceneTransitionManager not found!");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(undergroundSpawnPosition, Vector3.one * 0.5f);
    }
}
