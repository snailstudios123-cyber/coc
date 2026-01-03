using System.Collections;
using UnityEngine;

public class BossRewardHandler : MonoBehaviour
{
    [Header("Boss Reference")]
    [SerializeField] private GameObject bossObject;
    
    [Header("Rewards")]
    [SerializeField] private int healthIncrease = 1;
    [SerializeField] private bool giveFullMana = true;
    
    [Header("Botanica Return")]
    [SerializeField] private GameObject botanicaVisual;
    [SerializeField] private DialogueData finalDialogue;
    [SerializeField] private ParticleSystem appearEffect;
    [SerializeField] private float appearDelay = 2f;
    [SerializeField] private float fadeInDuration = 2f;
    
    [Header("Teleport Settings")]
    [SerializeField] private string villageSceneName = "Outdoors";
    [SerializeField] private Vector2 villageSpawnPosition = new Vector2(0, 0);
    [SerializeField] private float teleportDelay = 1f;
    
    private bool rewardsGiven = false;
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
        
        if (bossObject != null)
        {
            MechaGolemBoss boss = bossObject.GetComponent<MechaGolemBoss>();
            if (boss != null)
            {
                StartCoroutine(WaitForBossDeath(boss));
            }
            else
            {
                Debug.LogWarning("[BossReward] Boss object doesn't have MechaGolemBoss component!");
            }
        }
    }
    
    private IEnumerator WaitForBossDeath(MechaGolemBoss boss)
    {
        while (boss != null && boss.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("[BossReward] Boss defeated! Starting reward sequence");
        OnBossDefeated();
    }
    
    private void OnBossDefeated()
    {
        if (rewardsGiven) return;
        
        rewardsGiven = true;
        StartCoroutine(RewardSequence());
    }
    
    private IEnumerator RewardSequence()
    {
        yield return new WaitForSeconds(appearDelay);
        
        GiveRewards();
        
        yield return StartCoroutine(BotanicaAppearSequence());
        
        if (DialogueManager.Instance != null && finalDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(finalDialogue);
            
            while (DialogueManager.Instance.IsDialogueActive())
            {
                yield return null;
            }
        }
        
        Debug.Log("[BossReward] Final dialogue complete - teleporting to village");
        
        yield return new WaitForSeconds(teleportDelay);
        
        TeleportToVillage();
    }
    
    private void GiveRewards()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogWarning("[BossReward] PlayerController.Instance is null!");
            return;
        }
        
        Debug.Log($"[BossReward] Giving rewards: +{healthIncrease} max health, full mana");
        
        PlayerController.Instance.maxHealth += healthIncrease;
        
        PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
        
        if (giveFullMana)
        {
            PlayerController.Instance.Mana = 1f;
        }
        
        Debug.Log($"[BossReward] Player now has {PlayerController.Instance.maxHealth} max health");
    }
    
    private IEnumerator BotanicaAppearSequence()
    {
        Debug.Log("[BossReward] Botanica appearing for final dialogue");
        
        if (botanicaVisual != null)
        {
            botanicaVisual.SetActive(true);
        }
        
        if (appearEffect != null)
        {
            appearEffect.Play();
        }
        
        yield return StartCoroutine(FadeIn());
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
    
    private void TeleportToVillage()
    {
        if (SceneTransitionManager.Instance != null)
        {
            Debug.Log($"[BossReward] Teleporting to {villageSceneName}");
            SceneTransitionManager.Instance.TransitionToScene(villageSceneName, villageSpawnPosition);
        }
        else
        {
            Debug.LogError("[BossReward] SceneTransitionManager.Instance is null!");
        }
    }
}
