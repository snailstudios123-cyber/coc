using System.Collections;
using UnityEngine;

public class FireSwordSpell : MonoBehaviour
{
    [Header("Spell Data")]
    [SerializeField] private SpellData spellData;
    
    [Header("Fire Sword Settings")]
    [SerializeField] private float manaPerSecond = 0.15f;
    [SerializeField] private float maxChargeTime = 3f;
    [SerializeField] private int strikeCount = 5;
    [SerializeField] private float timeBetweenStrikes = 0.1f;
    [SerializeField] private float strikeRange = 2f;
    [SerializeField] private float damagePerStrike = 15f;
    [SerializeField] private float minimumHoldTime = 0.15f;
    
    [Header("Attack Area")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackArea = new Vector2(2f, 1.5f);
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject fireSlashEffect;
    [SerializeField] private GameObject chargeEffect;
    
    private PlayerController playerController;
    private Animator animator;
    private PlayerStateList pState;
    
    private bool isCharging = false;
    private float currentChargeTime = 0f;
    private bool isPerformingStrikes = false;
    private GameObject activeChargeEffect;
    private float keyHoldTime = 0f;
    private bool wasJumpingLastFrame = false;
    
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
        
        isCharging = false;
        pState.charging = false;
    }
    
    private void OnDisable()
    {
        if (isCharging)
        {
            CancelCharge();
        }
    }
    
    private void Update()
    {
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
        {
            return;
        }
        
        if (pState.cutscene || isPerformingStrikes)
        {
            if (isCharging)
            {
                CancelCharge();
            }
            return;
        }
        
        if (pState.jumping || pState.dashing)
        {
            if (isCharging)
            {
                CancelCharge();
            }
            keyHoldTime = 0f;
            return;
        }
        
        HandleFireSwordInput();
    }
    
    private void HandleFireSwordInput()
    {
        if (spellData != null && !spellData.isEquipped)
        {
            return;
        }
        
        if (pState.jumping || pState.dashing || pState.attacking)
        {
            if (isCharging)
            {
                CancelCharge();
            }
            keyHoldTime = 0f;
            return;
        }
        
        if (Input.GetKey(KeyCode.C))
        {
            keyHoldTime += Time.deltaTime;
            
            if (!isCharging && keyHoldTime >= minimumHoldTime && playerController.HasEnoughMana(manaPerSecond * Time.deltaTime))
            {
                StartCharging();
            }
            
            if (isCharging)
            {
                ContinueCharging();
            }
        }
        else
        {
            if (isCharging)
            {
                ReleaseFireSword();
            }
            else
            {
                if (pState.charging)
                {
                    pState.charging = false;
                }
            }
            keyHoldTime = 0f;
        }
    }
    
    private void StartCharging()
    {
        isCharging = true;
        currentChargeTime = 0f;
        pState.charging = true;
        
        if (chargeEffect != null && activeChargeEffect == null)
        {
            activeChargeEffect = Instantiate(chargeEffect, transform);
        }
        
        if (animator != null)
        {
            animator.SetBool("IsCharging", true);
            animator.SetTrigger("CastCharge");
        }
    }
    
    private void ContinueCharging()
    {
        float manaCost = manaPerSecond * Time.deltaTime;
        
        if (playerController.TryUseMana(manaCost))
        {
            currentChargeTime += Time.deltaTime;
            
            if (currentChargeTime >= maxChargeTime)
            {
                currentChargeTime = maxChargeTime;
            }
        }
        else
        {
            CancelCharge();
        }
    }
    
    private void CancelCharge()
    {
        isCharging = false;
        currentChargeTime = 0f;
        pState.charging = false;
        keyHoldTime = 0f;
        
        if (activeChargeEffect != null)
        {
            Destroy(activeChargeEffect);
            activeChargeEffect = null;
        }
        
        if (animator != null)
        {
            animator.SetBool("IsCharging", false);
        }
    }
    
    private void ReleaseFireSword()
    {
        if (currentChargeTime < 0.2f)
        {
            CancelCharge();
            return;
        }
        
        isCharging = false;
        pState.charging = false;
        keyHoldTime = 0f;
        
        if (activeChargeEffect != null)
        {
            Destroy(activeChargeEffect);
            activeChargeEffect = null;
        }
        
        if (animator != null)
        {
            animator.SetBool("IsCharging", false);
        }
        
        int strikes = CalculateStrikeCount();
        StartCoroutine(PerformRapidStrikes(strikes));
    }
    
    private int CalculateStrikeCount()
    {
        float chargePercent = currentChargeTime / maxChargeTime;
        int strikes = Mathf.CeilToInt(strikeCount * Mathf.Clamp01(chargePercent));
        return Mathf.Max(strikes, 1);
    }
    
    private IEnumerator PerformRapidStrikes(int numberOfStrikes)
    {
        isPerformingStrikes = true;
        pState.attacking = true;
        
        animator.SetTrigger("FireSwordStrike");
        
        for (int i = 0; i < numberOfStrikes; i++)
        {
            animator.SetTrigger("FireSwordStrike");
            PerformSingleStrike();
            
            yield return new WaitForSeconds(timeBetweenStrikes);
        }
        
        isPerformingStrikes = false;
        pState.attacking = false;
        currentChargeTime = 0f;
    }
    
    private void PerformSingleStrike()
    {
        if (attackPoint == null)
        {
            attackPoint = transform;
        }
        
        Vector2 attackPosition = attackPoint.position;
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPosition, attackArea, 0f, enemyLayer);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                enemyComponent.EnemyHit(damagePerStrike, knockbackDirection, 5f);
                
                if (enemy.CompareTag("Enemy"))
                {
                    playerController.AddMana(0.05f);
                }
            }
        }
        
        if (fireSlashEffect != null)
        {
            GameObject effect = Instantiate(fireSlashEffect, attackPoint.position, Quaternion.identity);
            effect.transform.SetParent(attackPoint);
            effect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
        }
    }
    
    public bool IsCharging()
    {
        return isCharging;
    }
    
    public bool IsPerformingStrikes()
    {
        return isPerformingStrikes;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackArea);
    }
}
