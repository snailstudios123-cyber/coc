using System.Collections;
using UnityEngine;

public class MedusaBoss : Enemy
{
    [Header("Detection & Movement")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float meleeRange = 3.5f;
    [SerializeField] private float preferredDistance = 6.5f;
    [SerializeField] private float slitherSpeed = 4f;
    [SerializeField] private float slitherAmplitude = 1.2f;
    [SerializeField] private float movementSmoothTime = 0.15f;
    [SerializeField] private Transform player;

    [Header("Petrify & Charge Attack")]
    [SerializeField] private float gazeCastTime = 0.9f;
    [SerializeField] private float petrificationDuration = 1.5f;
    [SerializeField] private float gazeRange = 12f;
    [SerializeField] private float gazeAngle = 60f;
    [SerializeField] private float chargeSpeed = 22f;
    [SerializeField] private float chargeDamage = 25f;
    [SerializeField] private float chargeKnockbackForce = 35f;
    [SerializeField] private float screechDuration = 0.6f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float gazeDodgeWindowStart = 0.35f;
    [SerializeField] private float gazeDodgeWindowEnd = 0.75f;

    [Header("Tail Whip Attack")]
    [SerializeField] private float tailWhipRange = 4f;
    [SerializeField] private float tailWhipDamage = 18f;
    [SerializeField] private float tailWhipKnockback = 28f;
    [SerializeField] private float tailWhipWindup = 0.35f;
    [SerializeField] private float hissDuration = 0.3f;

    [Header("Jump Slam Attack")]
    [SerializeField] private float jumpHeight = 8f;
    [SerializeField] private float jumpDuration = 0.75f;
    [SerializeField] private float slamDamage = 30f;
    [SerializeField] private float shockwaveSpeed = 16f;
    [SerializeField] private int shockwaveCount = 3;
    [SerializeField] private float shockwaveDamage = 12f;
    [SerializeField] private float shockwaveInterval = 0.22f;
    [SerializeField] private float slamRadius = 3.5f;

    [Header("Combat Timing & Flow")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float minAttackCooldown = 0.8f;
    [SerializeField] private float maxAttackCooldown = 2.2f;
    [SerializeField] private float recoveryTime = 0.4f;
    [SerializeField] private float anticipationTime = 0.25f;
    [SerializeField] private float comboChance = 0.3f;

    [Header("Polish & Feedback")]
    [SerializeField] private float screenShakeIntensity = 0.25f;
    [SerializeField] private float hitPauseTime = 0.08f;
    [SerializeField] private AnimationCurve chargeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private GameObject gazeDodgeEffect;
    [SerializeField] private float gazeWarningFlashDuration = 0.5f;
    [SerializeField] private Color gazeWarningColor = Color.magenta;
    [SerializeField] private float attackTelegraphTime = 0.15f;
    [SerializeField] private float movementDampingDuringAttack = 0.7f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform targetPlayer;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isDead = false;
    private bool hitWall = false;
    private bool hitPlayer = false;
    private Vector2 chargeStartPos;
    private int attackPattern = 0;
    private float currentSpeed;
    private float gazeStartTime = 0f;
    private bool isGazeCasting = false;
    private int consecutiveAttacks = 0;
    private float velocitySmoothRef = 0f;

    private const string ANIM_IDLE = "Idle";
    private const string ANIM_MOVING = "Moving";
    private const string ANIM_GAZE_ATTACK = "GazeAttack";
    private const string ANIM_CHARGE_ATTACK = "ChargeAttack";
    private const string ANIM_DEFEATED = "Defeated";

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else if (PlayerController.Instance != null)
            {
                player = PlayerController.Instance.transform;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        if (health <= 0)
        {
            Die();
            return;
        }

        if (isConfused)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (animator != null)
            {
                animator.Play(ANIM_IDLE);
            }
            return;
        }

        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
            return;
        }

        if (isAttacking || isCharging) return;

        if (player != null)
        {
            targetPlayer = player;
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

            if (distanceToPlayer <= detectionRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    ChooseAndExecuteAttack(distanceToPlayer);
                }
                else
                {
                    SlitherMovement();
                }
            }
            else
            {
                animator.SetBool(ANIM_IDLE, true);
                animator.SetBool(ANIM_MOVING, false);
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    protected virtual void LateUpdate()
    {
        if (!isDead && targetPlayer != null && !isCharging)
        {
            FlipTowardsPlayer();
        }
    }

    private void ChooseAndExecuteAttack(float distance)
    {
        bool canCombo = consecutiveAttacks > 0 && Random.value < comboChance;
        
        if (distance <= meleeRange)
        {
            if (Random.value > 0.5f || canCombo)
            {
                StartCoroutine(TailWhipAttack());
            }
            else
            {
                StartCoroutine(JumpSlamAttack());
            }
        }
        else if (distance <= preferredDistance + 2f)
        {
            float rand = Random.value;
            if (rand > 0.55f)
            {
                StartCoroutine(JumpSlamAttack());
            }
            else if (rand > 0.25f)
            {
                StartCoroutine(PetrifyAndChargeAttack());
            }
            else
            {
                StartCoroutine(TailWhipAttack());
            }
        }
        else
        {
            if (Random.value > 0.35f)
            {
                StartCoroutine(PetrifyAndChargeAttack());
            }
            else
            {
                StartCoroutine(JumpSlamAttack());
            }
        }
        
        attackPattern++;
        consecutiveAttacks++;
    }

    private void SlitherMovement()
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);
        float horizontalMove = 0f;
        
        float distanceThreshold = 2f;

        if (distanceToPlayer < preferredDistance - distanceThreshold)
        {
            horizontalMove = Mathf.Sign(transform.position.x - targetPlayer.position.x);
        }
        else if (distanceToPlayer > preferredDistance + distanceThreshold)
        {
            horizontalMove = Mathf.Sign(targetPlayer.position.x - transform.position.x);
        }

        float slitherOscillation = Mathf.Sin(Time.time * 3f) * slitherAmplitude;
        float targetVelocity = (horizontalMove * slitherSpeed) + slitherOscillation;
        
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetVelocity, ref velocitySmoothRef, movementSmoothTime);
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.x) > 0.15f)
        {
            animator.SetBool(ANIM_MOVING, true);
            animator.SetBool(ANIM_IDLE, false);
        }
        else
        {
            animator.SetBool(ANIM_MOVING, false);
            animator.SetBool(ANIM_IDLE, true);
        }
    }

    private void FlipTowardsPlayer()
    {
        if (targetPlayer == null || spriteRenderer == null) return;

        bool shouldFlip = targetPlayer.position.x < transform.position.x;
        if (spriteRenderer.flipX != shouldFlip)
        {
            spriteRenderer.flipX = shouldFlip;
        }
    }

    private IEnumerator PetrifyAndChargeAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        attackCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        
        rb.velocity = new Vector2(rb.velocity.x * movementDampingDuringAttack, rb.velocity.y);
        yield return new WaitForSeconds(attackTelegraphTime);
        
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(anticipationTime * 0.5f);

        StartCoroutine(FlashGazeWarning());

        animator.SetTrigger(ANIM_GAZE_ATTACK);
        
        isGazeCasting = true;
        gazeStartTime = Time.time;
        
        yield return new WaitForSeconds(gazeCastTime);

        PerformPetrifyAttack();
        isGazeCasting = false;

        yield return new WaitForSeconds(0.15f);

        if (targetPlayer != null)
        {
            FlipTowardsPlayer();
            
            Vector2 chargeDir = (targetPlayer.position - transform.position).normalized;
            chargeStartPos = transform.position;
            hitWall = false;
            hitPlayer = false;

            yield return new WaitForSeconds(0.1f);
            
            animator.SetTrigger(ANIM_CHARGE_ATTACK);
            isCharging = true;

            float chargeTime = 0f;
            float maxChargeTime = 2f;
            
            while (chargeTime < maxChargeTime && !hitWall && !hitPlayer)
            {
                float speedMultiplier = chargeCurve.Evaluate(chargeTime / maxChargeTime);
                rb.velocity = new Vector2(chargeDir.x * chargeSpeed * speedMultiplier, rb.velocity.y);

                RaycastHit2D wallHit = Physics2D.Raycast(transform.position, chargeDir, 1f, groundLayer);
                if (wallHit.collider != null)
                {
                    hitWall = true;
                    rb.velocity = Vector2.zero;
                    
                    animator.SetTrigger("Screech");
                    ShakeScreen(screenShakeIntensity * 0.6f);
                    yield return new WaitForSeconds(screechDuration);
                    break;
                }

                chargeTime += Time.deltaTime;
                yield return null;
            }

            if (hitPlayer)
            {
                rb.velocity = Vector2.zero;
                ShakeScreen(screenShakeIntensity * 0.8f);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                float slowdownTime = 0f;
                Vector2 currentVel = rb.velocity;
                while (slowdownTime < 0.3f)
                {
                    slowdownTime += Time.deltaTime;
                    rb.velocity = Vector2.Lerp(currentVel, Vector2.zero, slowdownTime / 0.3f);
                    yield return null;
                }
            }

            isCharging = false;
        }

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(recoveryTime);
        consecutiveAttacks = 0;
        isAttacking = false;
    }

    private IEnumerator FlashGazeWarning()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < gazeWarningFlashDuration)
        {
            float t = Mathf.PingPong(elapsed * 10f, 1f);
            spriteRenderer.color = Color.Lerp(originalColor, gazeWarningColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;
    }

    private void PerformPetrifyAttack()
    {
        if (targetPlayer == null) return;

        Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= gazeRange)
        {
            Vector2 facingDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            float angleToPlayer = Vector2.Angle(facingDirection, directionToPlayer);

            if (angleToPlayer <= gazeAngle)
            {
                if (HasLineOfSightToPlayer())
                {
                    if (IsPlayerLookingAtMedusa())
                    {
                        PetrifyPlayer();
                    }
                    else
                    {
                        OnGazeDodged();
                    }
                }
                else
                {
                    OnGazeDodged("BLOCKED!");
                }
            }
        }
    }

    private bool HasLineOfSightToPlayer()
    {
        if (targetPlayer == null) return false;

        Vector2 origin = transform.position;
        Vector2 direction = (targetPlayer.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        int layerMask = LayerMask.GetMask("Ground") | (1 << LayerMask.NameToLayer("Default"));
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        
        Debug.DrawRay(origin, direction * distance, hit.collider != null ? Color.red : Color.green, 0.5f);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("VineShield"))
            {
                Debug.Log($"Line of sight blocked by Vine Shield!");
                return false;
            }
            else if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                Debug.Log($"Line of sight blocked by: {hit.collider.name} (tag: {hit.collider.tag})");
                return false;
            }
        }

        return true;
    }

    private bool IsPlayerLookingAtMedusa()
    {
        if (PlayerController.Instance == null) return false;

        if (PlayerController.Instance.pState.invincible || PlayerController.Instance.pState.dashing)
        {
            return false;
        }

        if (DidPlayerUseSwapSpell())
        {
            return false;
        }

        return true;
    }

    private bool DidPlayerUseSwapSpell()
    {
        if (!isGazeCasting) return false;
        
        float timeSinceGazeStart = Time.time - gazeStartTime;
        
        bool inDodgeWindow = timeSinceGazeStart >= gazeDodgeWindowStart && 
                            timeSinceGazeStart <= gazeDodgeWindowEnd;
        
        if (!inDodgeWindow) return false;
        
        TeleportSwapSpell swapSpell = PlayerController.Instance.GetComponent<TeleportSwapSpell>();
        if (swapSpell != null && swapSpell.JustSwapped(0.3f))
        {
            return true;
        }
        
        return false;
    }

    private void OnGazeDodged(string message = "SWAPPED!")
    {
        if (gazeDodgeEffect != null && targetPlayer != null)
        {
            Instantiate(gazeDodgeEffect, targetPlayer.position + Vector3.up * 1.5f, Quaternion.identity);
        }
        
        StartCoroutine(ShowDodgeText(message));
    }

    private IEnumerator ShowDodgeText(string message = "SWAPPED!")
    {
        if (targetPlayer != null)
        {
            GameObject textObj = new GameObject("DodgeText");
            textObj.transform.position = targetPlayer.position + Vector3.up * 2f;
            
            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = message;
            textMesh.fontSize = 50;
            textMesh.color = message == "BLOCKED!" ? new Color(0.2f, 0.8f, 0.2f) : new Color(0f, 0.9f, 1f);
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            
            MeshRenderer meshRenderer = textObj.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = 100;
            }
            
            float elapsed = 0f;
            float duration = 1.2f;
            Vector3 startPos = textObj.transform.position;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                textObj.transform.position = startPos + Vector3.up * (t * 2.5f);
                textObj.transform.localScale = Vector3.one * (1f + Mathf.Sin(t * Mathf.PI) * 0.25f);
                
                Color color = textMesh.color;
                color.a = 1f - t;
                textMesh.color = color;
                
                yield return null;
            }
            
            Destroy(textObj);
        }
    }

    private void PetrifyPlayer()
    {
        if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
        {
            StartCoroutine(PetrifyPlayerCoroutine());
        }
    }

    private IEnumerator PetrifyPlayerCoroutine()
    {
        PlayerStateList pState = PlayerController.Instance.GetComponent<PlayerStateList>();
        if (pState != null)
        {
            pState.cutscene = true;
        }
        
        PlayerController.Instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        Animator playerAnimator = PlayerController.Instance.GetComponent<Animator>();
        SpriteRenderer playerSprite = PlayerController.Instance.GetComponent<SpriteRenderer>();
        
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("TurnToStone");
        }
        else if (playerSprite != null)
        {
            Color originalColor = playerSprite.color;
            float elapsed = 0f;
            
            while (elapsed < petrificationDuration)
            {
                float t = elapsed / petrificationDuration;
                playerSprite.color = Color.Lerp(originalColor, new Color(0.5f, 0.5f, 0.5f), t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            playerSprite.color = new Color(0.5f, 0.5f, 0.5f);
        }
        
        yield return new WaitForSeconds(petrificationDuration);
        
        if (playerSprite != null)
        {
            float elapsed = 0f;
            Color stoneColor = playerSprite.color;
            Color originalColor = Color.white;
            
            while (elapsed < 0.3f)
            {
                float t = elapsed / 0.3f;
                playerSprite.color = Color.Lerp(stoneColor, originalColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            playerSprite.color = originalColor;
        }
        
        if (pState != null)
        {
            pState.cutscene = false;
        }
    }

    private IEnumerator TailWhipAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        attackCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        
        rb.velocity = new Vector2(rb.velocity.x * movementDampingDuringAttack, rb.velocity.y);
        yield return new WaitForSeconds(attackTelegraphTime);
        
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(anticipationTime * 0.6f);

        animator.SetTrigger("Hiss");
        yield return new WaitForSeconds(hissDuration);

        animator.SetTrigger("TailWhip");
        yield return new WaitForSeconds(tailWhipWindup);

        PerformTailWhip();

        yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(recoveryTime);
        isAttacking = false;
    }

    private void PerformTailWhip()
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);
        
        if (distanceToPlayer <= tailWhipRange)
        {
            if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
            {
                Vector2 knockbackDirection = (targetPlayer.position - transform.position).normalized;
                
                ShakeScreen(screenShakeIntensity * 0.7f);
                StartCoroutine(HitPause(hitPauseTime * 0.8f));
                
                PlayerController.Instance.TakeDamageWithKnockback(
                    tailWhipDamage,
                    knockbackDirection * tailWhipKnockback
                );
            }
        }
    }

    private IEnumerator JumpSlamAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        attackCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        
        rb.velocity = new Vector2(rb.velocity.x * movementDampingDuringAttack, rb.velocity.y);
        yield return new WaitForSeconds(attackTelegraphTime);
        
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(anticipationTime * 0.7f);

        Vector2 jumpStart = transform.position;
        Vector2 targetPos = targetPlayer != null ? (Vector2)targetPlayer.position : jumpStart;
        
        animator.SetTrigger("Jump");
        
        float elapsed = 0f;
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;
            
            float arcHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            Vector2 currentPos = Vector2.Lerp(jumpStart, targetPos, t);
            currentPos.y += arcHeight;
            
            transform.position = currentPos;
            yield return null;
        }

        animator.SetTrigger("Slam");
        ShakeScreen(screenShakeIntensity * 1.2f);
        StartCoroutine(HitPause(hitPauseTime));
        
        PerformSlam();

        yield return new WaitForSeconds(0.15f);
        StartCoroutine(CreateShockwaves());

        yield return new WaitForSeconds(0.4f);
        yield return new WaitForSeconds(recoveryTime);
        isAttacking = false;
    }

    private void PerformSlam()
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);
        
        if (distanceToPlayer <= slamRadius)
        {
            if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
            {
                Vector2 knockbackDirection = (targetPlayer.position - transform.position).normalized;
                knockbackDirection.y = Mathf.Max(knockbackDirection.y, 0.6f);
                
                PlayerController.Instance.TakeDamageWithKnockback(
                    slamDamage,
                    knockbackDirection * 35f
                );
            }
        }
    }

    private IEnumerator CreateShockwaves()
    {
        for (int i = 0; i < shockwaveCount; i++)
        {
            CreateShockwave(Vector2.left);
            CreateShockwave(Vector2.right);
            
            yield return new WaitForSeconds(shockwaveInterval);
        }
    }

    private void CreateShockwave(Vector2 direction)
    {
        StartCoroutine(ShockwaveRoutine(direction));
    }

    private IEnumerator ShockwaveRoutine(Vector2 direction)
    {
        Vector2 startPos = transform.position;
        float distance = 0f;
        float maxDistance = 15f;
        
        while (distance < maxDistance)
        {
            distance += shockwaveSpeed * Time.deltaTime;
            Vector2 shockwavePos = startPos + direction * distance;
            
            if (targetPlayer != null)
            {
                float distToPlayer = Vector2.Distance(shockwavePos, targetPlayer.position);
                if (distToPlayer < 1f && PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
                {
                    PlayerController.Instance.TakeDamage(shockwaveDamage);
                    break;
                }
            }
            
            RaycastHit2D hit = Physics2D.Raycast(shockwavePos, direction, 0.5f, groundLayer);
            if (hit.collider != null) break;
            
            yield return null;
        }
    }

    private void ShakeScreen(float intensity)
    {
        if (Camera.main != null)
        {
            StartCoroutine(ScreenShakeRoutine(intensity));
        }
    }

    private IEnumerator ScreenShakeRoutine(float intensity)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            Camera.main.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Camera.main.transform.localPosition = originalPos;
    }

    private IEnumerator HitPause(float duration)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCharging && collision.CompareTag("Player"))
        {
            hitPlayer = true;
            rb.velocity = Vector2.zero;
            
            if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                knockbackDirection.y = Mathf.Max(knockbackDirection.y, 0.3f);
                
                StartCoroutine(HitPause(hitPauseTime * 0.9f));
                ShakeScreen(screenShakeIntensity);
                
                PlayerController.Instance.TakeDamageWithKnockback(
                    chargeDamage,
                    knockbackDirection * chargeKnockbackForce
                );
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
            hitPlayer = true;
            rb.velocity = Vector2.zero;
            
            if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                knockbackDirection.y = Mathf.Max(knockbackDirection.y, 0.3f);
                
                StartCoroutine(HitPause(hitPauseTime * 0.9f));
                ShakeScreen(screenShakeIntensity);
                
                PlayerController.Instance.TakeDamageWithKnockback(
                    chargeDamage,
                    knockbackDirection * chargeKnockbackForce
                );
            }
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (isDead) return;

        health -= _damageDone;

        if (!isRecoiling && !isAttacking)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        rb.velocity = Vector2.zero;
        
        if (animator != null)
        {
            animator.SetTrigger(ANIM_DEFEATED);
        }
        
        DropCoins();
        Destroy(gameObject, 1.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        Gizmos.color = Color.cyan;
        Vector2 facingDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector3 gazeLeft = Quaternion.Euler(0, 0, gazeAngle) * facingDirection * gazeRange;
        Vector3 gazeRight = Quaternion.Euler(0, 0, -gazeAngle) * facingDirection * gazeRange;

        Gizmos.DrawLine(transform.position, transform.position + gazeLeft);
        Gizmos.DrawLine(transform.position, transform.position + gazeRight);
        Gizmos.DrawLine(transform.position + gazeLeft, transform.position + gazeRight);
    }
}