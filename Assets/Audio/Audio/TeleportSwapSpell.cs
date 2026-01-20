using UnityEngine;

public class TeleportSwapSpell : MonoBehaviour
{
    [Header("Spell Settings")]
    [SerializeField] private SpellData spellData;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float confusionDuration = 2f;
    [SerializeField] private float groundCheckDistance = 1f;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject swapEffect;
    [SerializeField] private GameObject targetSwapEffect;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip swapSound;
    
    private bool pendingSwap = false;
    private Transform targetToSwap;
    private float lastSwapTime = -999f;
    
    public bool JustSwapped(float timeWindow = 0.5f)
    {
        return Time.time - lastSwapTime < timeWindow;
    }
    
    public void CastSwap()
    {
        if (spellData != null && !spellData.isEquipped)
        {
            return;
        }
        
        Transform nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null)
        {
            Debug.Log("[TeleportSwap] No valid target found within range");
            return;
        }
        
        pendingSwap = true;
        targetToSwap = nearestEnemy;
        
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("CastInstant");
        }
    }
    
    public void OnInstantSpellCast()
    {
        if (pendingSwap && targetToSwap != null)
        {
            pendingSwap = false;
            PerformSwap(targetToSwap);
            targetToSwap = null;
        }
    }
    
    private void PerformSwap(Transform target)
    {
        Vector3 playerPosition = transform.position;
        Vector3 targetPosition = target.position;
        
        bool targetIsGrounded = Physics2D.Raycast(
            targetPosition, 
            Vector2.down, 
            groundCheckDistance, 
            groundLayer
        );
        
        if (swapEffect != null)
        {
            Instantiate(swapEffect, playerPosition, Quaternion.identity);
        }
        
        if (targetSwapEffect != null)
        {
            Instantiate(targetSwapEffect, targetPosition, Quaternion.identity);
        }
        
        if (targetIsGrounded)
        {
            transform.position = targetPosition;
            target.position = playerPosition;
        }
        else
        {
            transform.position = new Vector3(targetPosition.x, playerPosition.y, playerPosition.z);
            target.position = new Vector3(playerPosition.x, targetPosition.y, targetPosition.z);
        }
        
        Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }
        
        Enemy enemyScript = target.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.ApplyConfusion(confusionDuration);
        }
        
        lastSwapTime = Time.time;
        
        if (audioSource != null && swapSound != null)
        {
            audioSource.PlayOneShot(swapSound);
        }
        
        Debug.Log($"[TeleportSwap] Swapped positions with {target.name} (Target grounded: {targetIsGrounded})");
    }
    
    private Transform FindNearestEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        
        Transform nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (Collider2D enemy in hitEnemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
        
        return nearestEnemy;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
