using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Damage and Knockback Settings:")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;
    [Space(5)]

    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45f;

    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;

    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;

    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;

    private float gravity;
    [Space(5)]

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private GameObject dashEffect;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashDirection = 0f;
    [Space(5)]

    [Header("Attack Settings:")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 SideAttackArea;

    [SerializeField] private Transform UpAttackTransform;
    [SerializeField] private Vector2 UpAttackArea;

    [SerializeField] private Transform DownAttackTransform;
    [SerializeField] private Vector2 DownAttackArea;

    [SerializeField] private LayerMask attackableLayer;

    private float timeBetweenAttack, timeSinceAttck;

    [SerializeField] public float damage;

    [SerializeField] private GameObject slashEffect;

    bool restoreTime;
    float restoreTimeSpeed;
    
    private bool isAttacking = false;
    private string queuedAttackType = "";
    [Space(5)]

    [Header("Down Thrust Settings:")]
    [SerializeField] private float downThrustSpeed = 25f;
    [SerializeField] private float downThrustBounceForce = 20f;
    [SerializeField] private float downThrustDuration = 0.5f;
    [SerializeField] private Transform thrustPoint;
    [SerializeField] private float thrustPointRadius = 0.3f;
    private bool isDownThrusting = false;
    private float downThrustTimer = 0f;
    [Space(5)]

    [Header("Audio Settings:")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sideAttackSound;
    [SerializeField] private AudioClip upAttackSound;
    [SerializeField] private AudioClip downAttackSound;
    [Space(5)]

    [Header("Recoil Settings:")]
    [SerializeField] private int recoilXSteps = 5;
    [SerializeField] private int recoilYSteps = 5;

    [SerializeField] private float recoilXSpeed = 100;
    [SerializeField] private float recoilYSpeed = 100;

    private int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    [Space(5)]

    [Header("Mana Settings")]
    [SerializeField] UnityEngine.UI.Image manaStorage;

    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    [Space(5)]

    [Header("Spell Settings")]
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    float timeSinceCast;
    [SerializeField] float spellDamage;
    [SerializeField] float downSpellForce;
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    
    [Header("Snow Spell Settings")]
    [SerializeField] private GameObject snowSpellPrefab;
    [SerializeField] private SpellData snowSpellData;
    [SerializeField] private float snowSpellManaCost = 0.5f;
    [SerializeField] private KeyCode snowSpellKey = KeyCode.Q;
    private float timeSinceSnowCast;
    [SerializeField] private float timeBetweenSnowCast = 2f;
    private bool pendingSnowCast = false;
    [Space(5)]

    [Header("Grappling Hook Settings")]
    [SerializeField] private GrapplingHook grapplingHook;
    [SerializeField] private float grapplingHookManaCost = 0.2f;
    [SerializeField] private Transform grapplingHookOrigin;
    [Space(5)]

    [Header("Telekinesis Settings")]
    [SerializeField] private TelekinesisSpell telekinesis;
    [SerializeField] private float telekinesisManaPerSecond = 0.2f;
    [Space(5)]

    [Header("Folium Turbo Settings")]
    [SerializeField] private FoliumTurboSpell foliumTurbo;
    [SerializeField] private float foliumTurboManaCost = 0.3f;
    [SerializeField] private float foliumTurboManaPerSecond = 0.2f;
    [SerializeField] private float holdTimeThreshold = 0.3f;
    private float fKeyHoldTime = 0f;
    private bool fKeyWasPressed = false;
    [Space(5)]

    [Header("Fire Sword Spell")]
    [SerializeField] private FireSwordSpell fireSwordSpell;
    [Space(5)]
    
    [Header("Teleport Swap Spell")]
    [SerializeField] private TeleportSwapSpell teleportSwapSpell;
    [SerializeField] private SpellData teleportSwapSpellData;
    [SerializeField] private float teleportSwapManaCost = 1f;
    [SerializeField] private KeyCode teleportSwapKey = KeyCode.R;
    private float timeSinceTeleportSwap;
    [SerializeField] private float timeBetweenTeleportSwap = 3f;
    private bool pendingTeleportSwap = false;
    [Space(5)]

    [Header("Climbing Settings")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private Transform ledgeCheckPoint;
    [SerializeField] private float ledgeCheckDistance = 0.5f;
    [SerializeField] private float ledgeClimbXOffset = 0.5f;
    [SerializeField] private float ledgeClimbYOffset = 1.5f;
    [SerializeField] private float ledgeClimbDuration = 0.3f;
    private bool isLedgeClimbing = false;
    [Space(5)]



    [HideInInspector] public PlayerStateList pState;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private float xAxis, yAxis;
    private bool attack = false;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        Health = maxHealth;
    }

    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        gravity = rb.gravityScale;
        Mana = mana;
        
        if (manaStorage != null)
        {
            manaStorage.fillAmount = Mana;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (attackPoint != null)
        {
            Gizmos.DrawWireCube(attackPoint.position, SideAttackArea);
        }
        
        if (UpAttackTransform != null)
        {
            Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        }
        
        if (DownAttackTransform != null)
        {
            Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
        }

        if (thrustPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(thrustPoint.position, thrustPointRadius);
        }

        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.cyan;
            Vector2 direction = pState != null && pState.lookingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(wallCheckPoint.position, direction * wallCheckDistance);
        }
        
        if (ledgeCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Vector2 direction = pState != null && pState.lookingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(ledgeCheckPoint.position, direction * ledgeCheckDistance);
        }
    }

    void Update()
    {
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
        {
            return;
        }

        if (pState.cutscene)
        {
            return;
        }

        GetInputs();
        UpdateJumpVariables();

        if (isDashing) return;

        if (!isDownThrusting)
        {
            Flip();
            Move();
            Jump();
            Climb();
        }

        HandleDash();
        Attack();
        UpdateDownThrust();
        RestoreTimeScale();
        FlashWhileInvincible();
        CastSpell();
        CastSnowSpell();
        HandleGrapplingHook();
        HandleTelekinesis();
        HandleFoliumTurbo();
        CastTeleportSwap();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        if (isDownThrusting)
        {
            PerformDownThrust();
        }
        else
        {
            Recoil();
        }
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");

        if (Input.GetKeyDown(KeyCode.X))
        {
            attack = true;
        }
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        if (isKnockedBack) return;
        
        if (pState.charging || pState.attacking || isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("Walking", false);
            return;
        }
        
        if (!pState.climbing)
        {
            rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
            bool shouldWalk = xAxis != 0 && Grounded();
            anim.SetBool("Walking", shouldWalk);
        }
        else
        {
            anim.SetBool("Walking", false);
        }
    }

    void HandleDash()
    {
        if (canDash)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                dashDirection = pState.lookingRight ? -1f : 1f;
                StartCoroutine(PerformDash());
            }
            else if (Input.GetKeyDown(KeyCode.RightShift))
            {
                dashDirection = pState.lookingRight ? 1f : -1f;
                StartCoroutine(PerformDash());
            }
        }
    }

    IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        pState.dashing = true;
        
        Debug.Log("[DASH] Triggering Dashing animation!");
        anim.SetTrigger("Dashing");
        
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        
        if (Grounded() && dashEffect != null)
        {
            Instantiate(dashEffect, transform);
        }
        
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        rb.gravityScale = originalGravity;
        rb.velocity = new Vector2(rb.velocity.x * 0.5f, rb.velocity.y);
        
        isDashing = false;
        pState.dashing = false;
        
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Attack()
    {
        if (pState.charging || pState.attacking || isAttacking) return;
        
        timeSinceAttck += Time.deltaTime;

        if (attack && timeSinceAttck >= timeBetweenAttack)
        {
            timeSinceAttck = 0;
            isAttacking = true;
            pState.attacking = true;
            anim.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                queuedAttackType = "side";
            }
            else if (yAxis > 0)
            {
                queuedAttackType = "up";
            }
            else if (yAxis < 0 && !Grounded())
            {
                queuedAttackType = "down";
                StartDownThrust();
            }
        }
    }
    
    public void PerformSideAttack()
    {
        CheckForParry();
        Hit(attackPoint, SideAttackArea, ref pState.recoilingX, recoilXSpeed);
        Instantiate(slashEffect, attackPoint);
        PlayAttackSound(sideAttackSound);
    }
    
    public void PerformUpAttack()
    {
        CheckForParry();
        Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
        SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
        PlayAttackSound(upAttackSound);
    }
    
    private void CheckForParry()
    {
        if (ParrySystem.Instance == null) return;
        
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint.position, SideAttackArea, 0f, attackableLayer);
        
        foreach (Collider2D hit in hits)
        {
            EnemyAttackWarning warning = hit.GetComponent<EnemyAttackWarning>();
            if (warning != null && warning.IsInParryWindow())
            {
                ParrySystem.Instance.TryParry(hit.gameObject, hit.transform.position);
                return;
            }
        }
    }
    
    public void OnAttackAnimationHit()
    {
        if (queuedAttackType == "side")
        {
            PerformSideAttack();
        }
        else if (queuedAttackType == "up")
        {
            PerformUpAttack();
        }
    }
    
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        pState.attacking = false;
        queuedAttackType = "";
        anim.SetBool("Walking", false);
        anim.ResetTrigger("Attacking");
    }

    void StartDownThrust()
    {
        isDownThrusting = true;
        downThrustTimer = 0f;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        anim.SetBool("DownThrusting", true);

        SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
        PlayAttackSound(downAttackSound);
        
        isAttacking = false;
        pState.attacking = false;
        queuedAttackType = "";
    }

    void UpdateDownThrust()
    {
        if (isDownThrusting)
        {
            downThrustTimer += Time.deltaTime;

            if (Grounded() || downThrustTimer >= downThrustDuration)
            {
                EndDownThrust();
            }
        }
    }

    void PerformDownThrust()
    {
        rb.velocity = new Vector2(0, -downThrustSpeed);

        if (thrustPoint != null)
        {
            Collider2D[] objectsToHit = Physics2D.OverlapCircleAll(thrustPoint.position, thrustPointRadius, attackableLayer);

            if (objectsToHit.Length > 0)
            {
                bool hitSomething = false;

                for (int i = 0; i < objectsToHit.Length; i++)
                {
                    if (objectsToHit[i].GetComponent<Enemy>() != null)
                    {
                        objectsToHit[i].GetComponent<Enemy>().EnemyHit(
                            damage,
                            Vector2.down,
                            0
                        );

                        if (objectsToHit[i].CompareTag("Enemy"))
                        {
                            Mana += manaGain;
                        }

                        hitSomething = true;
                    }
                    else
                    {
                        hitSomething = true;
                        Debug.Log("Hit attackable object: " + objectsToHit[i].name);
                    }
                }

                if (hitSomething)
                {
                    BounceFromDownThrust();
                }
            }
        }
        else
        {
            Debug.LogWarning("Thrust Point is not assigned! Assign it in the inspector.");
        }
    }

    void BounceFromDownThrust()
    {
        isDownThrusting = false;
        rb.gravityScale = gravity;

        anim.SetBool("DownThrusting", false);

        rb.velocity = new Vector2(rb.velocity.x, downThrustBounceForce);

        airJumpCounter = 0;
        pState.jumping = false;

        Debug.Log("BOUNCED!");
    }

    void EndDownThrust()
    {
        isDownThrusting = false;
        rb.gravityScale = gravity;
        downThrustTimer = 0f;

        anim.SetBool("DownThrusting", false);
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(
                    damage,
                    (transform.position - objectsToHit[i].transform.position).normalized,
                    _recoilStrength
                );

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void PlayAttackSound(AudioClip _attackSound)
    {
        if (audioSource != null && _attackSound != null)
        {
            if (!audioSource.enabled)
                audioSource.enabled = true;

            if (audioSource.volume <= 0)
                audioSource.volume = 0.8f;

            audioSource.PlayOneShot(_attackSound, audioSource.volume);
        }
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        if (pState.invincible) return;
        
        Health -= Mathf.RoundToInt(_damage);
        isAttacking = false;
        pState.attacking = false;
        queuedAttackType = "";
        StartCoroutine(StopTakingDamage());
    }
    
    public void TakeDamageWithKnockback(float _damage, Vector2 knockbackDirection)
    {
        if (pState.invincible) return;
        
        Health -= Mathf.RoundToInt(_damage);
        isAttacking = false;
        pState.attacking = false;
        queuedAttackType = "";
        
        if (pState.climbing)
        {
            pState.climbing = false;
            rb.gravityScale = gravity;
        }
        
        StartCoroutine(Knockback(knockbackDirection));
        StartCoroutine(StopTakingDamage());
    }
    
    IEnumerator Knockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(knockbackDuration);
        
        isKnockedBack = false;
        rb.velocity = new Vector2(rb.velocity.x * 0.5f, rb.velocity.y);
    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = _newTimeScale;
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
                manaStorage.fillAmount = Mana;
            }
        }
    }

    public bool TryUseMana(float amount)
    {
        if (mana >= amount)
        {
            Mana -= amount;
            return true;
        }
        return false;
    }

    public bool HasEnoughMana(float amount)
    {
        return mana >= amount;
    }

    void CastSpell()
    {
        if (Input.GetButtonDown("CastSpell") && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if (Grounded())
        {
            downSpellFireball.SetActive(false);
        }

        if (downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }

    void CastSnowSpell()
    {
        if (snowSpellData != null && !snowSpellData.isEquipped)
        {
            return;
        }
        
        if (Input.GetKeyDown(snowSpellKey) && timeSinceSnowCast >= timeBetweenSnowCast && Mana >= snowSpellManaCost)
        {
            if (snowSpellPrefab != null)
            {
                Mana -= snowSpellManaCost;
                timeSinceSnowCast = 0;
                
                pendingSnowCast = true;
                anim.SetTrigger("CastInstant");
            }
            else
            {
                Debug.LogWarning("Snow Spell Prefab is not assigned!");
            }
        }
        else
        {
            timeSinceSnowCast += Time.deltaTime;
        }
    }
    
    void CastTeleportSwap()
    {
        if (teleportSwapSpellData != null && !teleportSwapSpellData.isEquipped)
        {
            return;
        }
        
        if (Input.GetKeyDown(teleportSwapKey) && timeSinceTeleportSwap >= timeBetweenTeleportSwap && Mana >= teleportSwapManaCost)
        {
            if (teleportSwapSpell != null)
            {
                Mana -= teleportSwapManaCost;
                timeSinceTeleportSwap = 0;
                
                pendingTeleportSwap = true;
                teleportSwapSpell.CastSwap();
            }
            else
            {
                Debug.LogWarning("Teleport Swap Spell component is not assigned!");
            }
        }
        else
        {
            timeSinceTeleportSwap += Time.deltaTime;
        }
    }

    IEnumerator CastCoroutine()
    {
        anim.SetBool("Casting", true);
        yield return new WaitForSeconds(0.15f);

        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            GameObject _fireBall = Instantiate(sideSpellFireball, attackPoint.position, Quaternion.identity);

            if (pState.lookingRight)
            {
                _fireBall.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _fireBall.transform.eulerAngles = new Vector2(_fireBall.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;
        }
        else if (yAxis > 0)
        {
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;
        }
        else if (yAxis < 0 && !Grounded())
        {
            downSpellFireball.SetActive(true);
        }

        Mana -= manaSpellCost;
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("Casting", false);
        pState.casting = false;
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (!pState.jumping && !pState.climbing)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0 && !pState.climbing)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        anim.SetBool("Jumping", !Grounded() && !pState.climbing);
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    bool OnWall()
    {
        if (wallCheckPoint == null) return false;

        RaycastHit2D hit = Physics2D.Raycast(
            wallCheckPoint.position, 
            pState.lookingRight ? Vector2.right : Vector2.left, 
            wallCheckDistance, 
            climbableLayer
        );

        return hit.collider != null;
    }
    
    bool AtLedgeTop()
    {
        if (wallCheckPoint == null || !pState.climbing) return false;
        
        bool wallInFront = OnWall();
        
        return !wallInFront && yAxis > 0;
    }

    void Climb()
    {
        if (isLedgeClimbing) return;
        
        bool isOnWall = OnWall();
        bool notGrounded = !Grounded();
        bool pressingTowardWall = xAxis != 0;

        if (isOnWall && notGrounded && pressingTowardWall)
        {
            pState.climbing = true;
        }
        else if (pState.climbing && AtLedgeTop())
        {
            StartCoroutine(LedgeClimb());
            return;
        }
        else
        {
            pState.climbing = false;
        }

        if (pState.climbing)
        {
            rb.gravityScale = 0;
            
            float verticalVelocity = yAxis * climbSpeed;
            rb.velocity = new Vector2(rb.velocity.x, verticalVelocity);
            
            anim.SetBool("Climbing", true);

            if (Input.GetButtonDown("Jump"))
            {
                pState.climbing = false;
                rb.gravityScale = gravity;
                pState.jumping = true;
                
                Vector2 wallJumpDirection = pState.lookingRight ? new Vector2(-1, 1) : new Vector2(1, 1);
                rb.velocity = new Vector2(wallJumpDirection.x * walkSpeed * 1.5f, jumpForce * 0.8f);
            }
        }
        else
        {
            if (rb.gravityScale == 0 && !isLedgeClimbing)
            {
                rb.gravityScale = gravity;
            }
            anim.SetBool("Climbing", false);
        }
    }
    
    IEnumerator LedgeClimb()
    {
        isLedgeClimbing = true;
        pState.climbing = false;
        anim.SetBool("Climbing", false);
        
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        
        Vector2 startPosition = transform.position;
        float xOffset = pState.lookingRight ? ledgeClimbXOffset : -ledgeClimbXOffset;
        Vector2 targetPosition = new Vector2(startPosition.x + xOffset, startPosition.y + ledgeClimbYOffset);
        
        float elapsed = 0f;
        while (elapsed < ledgeClimbDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / ledgeClimbDuration);
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.position = targetPosition;
        rb.gravityScale = gravity;
        rb.velocity = new Vector2(0, 0);
        isLedgeClimbing = false;
    }

    void HandleGrapplingHook()
    {
        if (!IsSpellEquipped("Lora Vitis"))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && Mana >= grapplingHookManaCost && grapplingHook != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            worldMousePos.z = 0;

            Vector2 startPosition = grapplingHookOrigin != null ? grapplingHookOrigin.position : transform.position;
            Vector2 hookDirection = ((Vector2)worldMousePos - startPosition).normalized;

            grapplingHook.LaunchHookToTarget(hookDirection, startPosition, worldMousePos);
            Mana -= grapplingHookManaCost;
            pState.grappling = true;
        }

        if (pState.grappling)
        {
            if (grapplingHook == null || !grapplingHook.IsHookActive())
            {
                pState.grappling = false;
            }
        }

        if (pState.grappling && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.G)))
        {
            if (grapplingHook != null)
            {
                grapplingHook.RetractHook();
            }
        }
    }

    void HandleTelekinesis()
    {
        if (telekinesis == null) return;

        if (!IsSpellEquipped("Telekinesis"))
        {
            if (telekinesis.IsHoldingObject())
            {
                telekinesis.ReleaseObject();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.T) && !telekinesis.IsHoldingObject() && Mana > 0)
        {
            telekinesis.TryGrabObject();
            return;
        }

        if (telekinesis.IsHoldingObject())
        {
            Mana -= Time.deltaTime * telekinesisManaPerSecond;
            telekinesis.UpdateHeldObject();

            if (Input.GetMouseButtonDown(0))
            {
                telekinesis.ThrowObject();
            }

            if (Input.GetKeyDown(KeyCode.T) || Mana <= 0)
            {
                telekinesis.ReleaseObject();
            }
        }
    }

    void HandleFoliumTurbo()
    {
        if (foliumTurbo == null) return;

        if (!IsSpellEquipped("Folium Turbo"))
        {
            if (foliumTurbo.IsDefensiveActive())
            {
                foliumTurbo.StopDefensiveTornado();
            }
            fKeyHoldTime = 0f;
            fKeyWasPressed = false;
            return;
        }

        if (Input.GetKey(KeyCode.F))
        {
            if (!fKeyWasPressed)
            {
                fKeyWasPressed = true;
                fKeyHoldTime = 0f;
            }
            
            fKeyHoldTime += Time.deltaTime;
            
            if (fKeyHoldTime >= holdTimeThreshold && !foliumTurbo.IsDefensiveActive() && !foliumTurbo.IsAnySpellActive())
            {
                if (Mana >= foliumTurboManaCost)
                {
                    foliumTurbo.StartDefensiveTornado();
                    Mana -= foliumTurboManaCost;
                }
            }
            
            if (foliumTurbo.IsDefensiveActive())
            {
                if (Mana > 0)
                {
                    Mana -= Time.deltaTime * foliumTurboManaPerSecond;
                }
                else
                {
                    foliumTurbo.StopDefensiveTornado();
                }
            }
        }
        
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (foliumTurbo.IsDefensiveActive())
            {
                foliumTurbo.StopDefensiveTornado();
            }
            else if (fKeyHoldTime < holdTimeThreshold && !foliumTurbo.IsAnySpellActive())
            {
                if (Mana >= foliumTurboManaCost)
                {
                    foliumTurbo.CastOffensiveTornado();
                    Mana -= foliumTurboManaCost;
                }
            }
            
            fKeyHoldTime = 0f;
            fKeyWasPressed = false;
        }
    }

    private bool IsSpellEquipped(string spellName)
    {
        if (SpellManager.Instance == null)
        {
            return true;
        }

        List<SpellData> equippedSpells = SpellManager.Instance.GetEquippedSpells();
        foreach (SpellData spell in equippedSpells)
        {
            if (spell.spellName == spellName)
            {
                return true;
            }
        }
        return false;
    }

    public float GetDamage()
    {
        return damage;
    }

    public void SetInvulnerable(bool invulnerable)
    {
        pState.invincible = invulnerable;
    }

    public void AddMana(float amount)
    {
        Mana += amount;
    }

    public void OnInstantSpellCast()
    {
        Debug.Log("[SpellCast] Instant spell animation event triggered");
        
        if (pendingSnowCast)
        {
            pendingSnowCast = false;
            Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0);
            Instantiate(snowSpellPrefab, spawnPosition, Quaternion.identity);
        }
        
        if (pendingTeleportSwap && teleportSwapSpell != null)
        {
            pendingTeleportSwap = false;
            teleportSwapSpell.OnInstantSpellCast();
        }
        
        FoliumTurboSpell foliumSpell = GetComponent<FoliumTurboSpell>();
        if (foliumSpell != null)
        {
            foliumSpell.OnInstantSpellCast();
        }
        
        LevitationSpell levitationSpell = GetComponent<LevitationSpell>();
        if (levitationSpell != null)
        {
            levitationSpell.OnInstantSpellCast();
        }
    }

    public void OnChargingLoopStart()
    {
        Debug.Log("[SpellCast] ⚡ Charging loop animation started - wind-up complete!");
    }

    public void OnSpellRelease()
    {
        Debug.Log("[SpellCast] Spell release animation event triggered");
    }
}