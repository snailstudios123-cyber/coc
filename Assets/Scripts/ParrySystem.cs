using System.Collections;
using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    public static ParrySystem Instance { get; private set; }

    [Header("Parry Timing")]
    [SerializeField] private float parryWindowDuration = 0.2f;
    
    [Header("Parry Rewards")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private float parrySlowMotionDuration = 0.15f;
    [SerializeField] private float parrySlowMotionScale = 0.2f;
    [SerializeField] private float enemyStunDuration = 1.5f;
    [SerializeField] private float parryDamageMultiplier = 1.5f;
    
    [Header("Visual & Audio")]
    [SerializeField] private GameObject parryEffect;
    [SerializeField] private AudioClip parrySound;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool TryParry(GameObject attackingEnemy, Vector3 parryPosition)
    {
        Enemy enemyComponent = attackingEnemy.GetComponent<Enemy>();
        if (enemyComponent == null) return false;
        
        StartCoroutine(ExecuteParry(attackingEnemy, enemyComponent, parryPosition));
        return true;
    }
    
    private IEnumerator ExecuteParry(GameObject enemyObj, Enemy enemy, Vector3 parryPosition)
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.pState.invincible = true;
        }
        
        if (parryEffect != null)
        {
            Instantiate(parryEffect, parryPosition, Quaternion.identity);
        }
        
        if (parrySound != null)
        {
            AudioSource.PlayClipAtPoint(parrySound, parryPosition, 1f);
        }
        
        Time.timeScale = parrySlowMotionScale;
        yield return new WaitForSecondsRealtime(parrySlowMotionDuration);
        Time.timeScale = 1f;
        
        if (enemy != null)
        {
            Vector2 knockbackDirection = (enemyObj.transform.position - PlayerController.Instance.transform.position).normalized;
            float parryDamage = PlayerController.Instance.damage * parryDamageMultiplier;
            enemy.EnemyHit(parryDamage, knockbackDirection, 20f);
            
            if (enemy is Skeleton skeleton)
            {
                StartCoroutine(StunEnemy(skeleton));
            }
        }
        
        yield return new WaitForSeconds(invulnerabilityDuration - parrySlowMotionDuration);
        
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.pState.invincible = false;
        }
    }
    
    private IEnumerator StunEnemy(Skeleton skeleton)
    {
        bool wasEnabled = skeleton.enabled;
        skeleton.enabled = false;
        
        yield return new WaitForSeconds(enemyStunDuration);
        
        if (skeleton != null)
        {
            skeleton.enabled = wasEnabled;
        }
    }
    
    public float GetParryWindow()
    {
        return parryWindowDuration;
    }
}
