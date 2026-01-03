using System.Collections;
using UnityEngine;

public class BotanicaEncounter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject botanicaVisual;
    [SerializeField] private DialogueData initialDialogue;
    [SerializeField] private ParticleSystem appearEffect;
    [SerializeField] private ParticleSystem disappearEffect;
    
    [Header("Appearance Settings")]
    [SerializeField] private float appearDelay = 1f;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private bool autoTrigger = true;
    
    private bool hasTriggered = false;
    private SpriteRenderer botanicaRenderer;
    
    private void Start()
    {
        if (botanicaVisual != null)
        {
            botanicaRenderer = botanicaVisual.GetComponent<SpriteRenderer>();
            if (botanicaRenderer != null)
            {
                Color color = botanicaRenderer.color;
                color.a = 0;
                botanicaRenderer.color = color;
            }
            botanicaVisual.SetActive(false);
        }
        
        if (appearEffect != null)
        {
            appearEffect.Stop();
        }
        
        if (disappearEffect != null)
        {
            disappearEffect.Stop();
        }
        
        if (autoTrigger)
        {
            StartCoroutine(TriggerEncounterAfterDelay());
        }
    }
    
    private IEnumerator TriggerEncounterAfterDelay()
    {
        yield return new WaitForSeconds(appearDelay);
        TriggerEncounter();
    }
    
    public void TriggerEncounter()
    {
        if (hasTriggered) return;
        
        hasTriggered = true;
        Debug.Log("[Botanica] Encounter triggered - goddess appearing");
        
        StartCoroutine(AppearSequence());
    }
    
    private IEnumerator AppearSequence()
    {
        if (botanicaVisual != null)
        {
            botanicaVisual.SetActive(true);
        }
        
        if (appearEffect != null)
        {
            appearEffect.Play();
        }
        
        yield return StartCoroutine(FadeIn());
        
        if (DialogueManager.Instance != null && initialDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(initialDialogue);
            
            while (DialogueManager.Instance.IsDialogueActive())
            {
                yield return null;
            }
        }
        
        Debug.Log("[Botanica] Initial dialogue complete - player can now explore");
    }
    
    private IEnumerator FadeIn()
    {
        if (botanicaRenderer == null) yield break;
        
        float elapsed = 0f;
        Color color = botanicaRenderer.color;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            botanicaRenderer.color = color;
            yield return null;
        }
        
        color.a = 1;
        botanicaRenderer.color = color;
    }
    
    public IEnumerator Disappear()
    {
        Debug.Log("[Botanica] Goddess disappearing");
        
        if (disappearEffect != null)
        {
            disappearEffect.Play();
        }
        
        yield return StartCoroutine(FadeOut());
        
        if (botanicaVisual != null)
        {
            botanicaVisual.SetActive(false);
        }
    }
    
    private IEnumerator FadeOut()
    {
        if (botanicaRenderer == null) yield break;
        
        float elapsed = 0f;
        Color color = botanicaRenderer.color;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsed / fadeInDuration);
            botanicaRenderer.color = color;
            yield return null;
        }
        
        color.a = 0;
        botanicaRenderer.color = color;
    }
}
