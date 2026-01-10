using System.Collections;
using UnityEngine;

public class BeamSpell : MonoBehaviour
{
    [Header("Spell Data")]
    [SerializeField] private SpellData spellData;
    
    [Header("Beam Settings")]
    [SerializeField] private float manaPerSecond = 0.2f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float minimumHoldTime = 0.2f;
    [SerializeField] private float beamDuration = 0.5f;
    [SerializeField] private float baseDamagePerSecond = 30f;
    [SerializeField] private float maxDamageMultiplier = 3f;
    
    [Header("Beam Properties")]
    [SerializeField] private float beamRange = 10f;
    [SerializeField] private float beamWidth = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject chargeEffect;
    [SerializeField] private GameObject beamEffect;
    [SerializeField] private Transform beamOrigin;
    
    [Header("Audio")]
    [SerializeField] private AudioClip chargeSound;
    [SerializeField] private AudioClip fireSound;
    
    private PlayerController playerController;
    private Animator animator;
    private PlayerStateList pState;
    
    private bool isCharging = false;
    private float currentChargeTime = 0f;
    private bool isFiringBeam = false;
    private GameObject activeChargeEffect;
    private GameObject activeBeamEffect;
    private float keyHoldTime = 0f;
    
    private const float DAMAGE_TICK_RATE = 0.1f;
    
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
        
        if (beamOrigin == null)
        {
            beamOrigin = transform;
        }
        
        isCharging = false;
        pState.charging = false;
    }
    
    private void OnDisable()
    {
        if (isCharging)
        {
            CancelCharge();
        }
        
        if (isFiringBeam)
        {
            StopBeam();
        }
    }
    
    private void Update()
    {
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
        {
            return;
        }
        
        if (pState.cutscene || isFiringBeam)
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
        
        HandleBeamInput();
    }
    
    private void HandleBeamInput()
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
        
        if (Input.GetKey(KeyCode.V))
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
                ReleaseBeam();
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
            Vector3 effectPosition = beamOrigin.position;
            activeChargeEffect = Instantiate(chargeEffect, effectPosition, Quaternion.identity);
            activeChargeEffect.transform.SetParent(beamOrigin);
        }
        
        if (animator != null)
        {
            Debug.Log("[BeamSpell] Setting IsCharging=true and triggering CastCharge");
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
    
    private void ReleaseBeam()
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
            Debug.Log("[BeamSpell] Setting IsCharging=false (releasing beam)");
            animator.SetBool("IsCharging", false);
        }
        
        float damageMultiplier = CalculateDamageMultiplier();
        StartCoroutine(FireBeam(damageMultiplier));
    }
    
    private float CalculateDamageMultiplier()
    {
        float chargePercent = currentChargeTime / maxChargeTime;
        return 1f + (maxDamageMultiplier - 1f) * chargePercent;
    }
    
    private IEnumerator FireBeam(float damageMultiplier)
    {
        isFiringBeam = true;
        pState.attacking = true;
        
        if (animator != null)
        {
            animator.SetTrigger("FireBeam");
        }
        
        Vector2 beamDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        
        if (beamEffect != null)
        {
            activeBeamEffect = Instantiate(beamEffect, beamOrigin.position, Quaternion.identity);
            activeBeamEffect.transform.SetParent(beamOrigin);
            
            float angle = transform.localScale.x > 0 ? 0f : 180f;
            activeBeamEffect.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            SpriteRenderer beamRenderer = activeBeamEffect.GetComponent<SpriteRenderer>();
            if (beamRenderer != null)
            {
                Vector3 beamScale = activeBeamEffect.transform.localScale;
                beamScale.x = beamRange;
                activeBeamEffect.transform.localScale = beamScale;
            }
        }
        
        float elapsedTime = 0f;
        float nextDamageTick = 0f;
        
        while (elapsedTime < beamDuration)
        {
            if (elapsedTime >= nextDamageTick)
            {
                DealBeamDamage(beamDirection, damageMultiplier);
                nextDamageTick += DAMAGE_TICK_RATE;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        StopBeam();
    }
    
    private void DealBeamDamage(Vector2 beamDirection, float damageMultiplier)
    {
        Vector2 beamStart = beamOrigin.position;
        
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            beamStart,
            new Vector2(beamRange, beamWidth),
            0f,
            beamDirection,
            beamRange,
            enemyLayer
        );
        
        float damage = baseDamagePerSecond * DAMAGE_TICK_RATE * damageMultiplier;
        
        foreach (RaycastHit2D hit in hits)
        {
            Enemy enemyComponent = hit.collider.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                Vector2 knockbackDirection = beamDirection;
                enemyComponent.EnemyHit(damage, knockbackDirection, 2f);
                
                if (hit.collider.CompareTag("Enemy"))
                {
                    playerController.AddMana(0.02f);
                }
            }
        }
    }
    
    private void StopBeam()
    {
        isFiringBeam = false;
        pState.attacking = false;
        currentChargeTime = 0f;
        
        if (activeBeamEffect != null)
        {
            Destroy(activeBeamEffect);
            activeBeamEffect = null;
        }
    }
    
    public bool IsCharging()
    {
        return isCharging;
    }
    
    public bool IsFiringBeam()
    {
        return isFiringBeam;
    }
    
    public float GetChargePercent()
    {
        return Mathf.Clamp01(currentChargeTime / maxChargeTime);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (beamOrigin == null)
            return;
        
        Vector2 beamDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 beamStart = beamOrigin.position;
        Vector2 beamEnd = beamStart + beamDirection * beamRange;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(beamStart + Vector2.up * (beamWidth / 2), beamEnd + Vector2.up * (beamWidth / 2));
        Gizmos.DrawLine(beamStart - Vector2.up * (beamWidth / 2), beamEnd - Vector2.up * (beamWidth / 2));
        
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawCube((beamStart + beamEnd) / 2, new Vector3(beamRange, beamWidth, 0.1f));
    }
}
