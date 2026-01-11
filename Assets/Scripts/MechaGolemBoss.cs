using System.Collections;
using UnityEngine;

public class MechaGolemBoss : Enemy
{
    [Header("Boss Stats")]
    [SerializeField] private float maxHealth = 500f;
    [SerializeField] private float armorMultiplier = 0.5f;
    [SerializeField] private bool hasArmor = false;

    [Header("Movement")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float rangedAttackRange = 8f;
    [SerializeField] private Transform player;

    [Header("Melee Attack Settings")]
    [SerializeField] private Transform meleeAttackTransform;
    [SerializeField] private Vector2 meleeAttackArea = new Vector2(3f, 2.5f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float meleeAttackDamage = 15f;
    [SerializeField] private float meleeAttackDelay = 0.3f;

    [Header("Ranged Attack Settings")]
    [SerializeField] private float rangedAttackDamage = 10f;
    [SerializeField] private GameObject rangedProjectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 10f;

    [Header("Laser Attack Settings")]
    [SerializeField] private float laserDamage = 25f;
    [SerializeField] private float laserCastTime = 0.583f;
    [SerializeField] private float laserDuration = 1.167f;
    [SerializeField] private GameObject laserBeamPrefab;
    [SerializeField] private Transform laserSpawnPoint;
    [SerializeField] private float laserKnockbackForce = 20f;

    [Header("Combat Timing")]
    [SerializeField] private float attackCooldown = 2f;

    [Header("Block Settings")]
    [SerializeField] private float blockChance = 0.3f;
    [SerializeField] private float blockDuration = 0.75f;
    [SerializeField] private float blockCooldown = 5f;

    [Header("Armor Buff Settings")]
    [SerializeField] private float armorBuffHealthThreshold = 0.5f;
    [SerializeField] private float armorBuffDuration = 0.917f;

    private Animator animator;
    private Transform targetPlayer;
    private float lastAttackTime;
    private float lastBlockTime;
    private bool isAttacking = false;
    private bool isBlocking = false;
    private bool isCastingLaser = false;
    private bool isDead = false;
    private bool hasUsedArmorBuff = false;
    private GameObject activeLaserBeam;

    private const string ANIM_IDLE = "Idle";
    private const string ANIM_MOVING = "Moving";
    private const string ANIM_GLOWING = "Glowing";
    private const string ANIM_RANGED_ATTACK = "RangedAttack";
    private const string ANIM_MELEE_ATTACK = "MeleeAttack";
    private const string ANIM_LASER_CAST = "LaserCast";
    private const string ANIM_LASER_BEAM = "LaserBeam";
    private const string ANIM_ARMOR_BUFF = "ArmorBuff";
    private const string ANIM_DEFEATED = "Defeated";
    private const string ANIM_BLOCK = "Block";

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        health = maxHealth;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        lastBlockTime = -blockCooldown;
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

        if (!hasUsedArmorBuff && health <= maxHealth * armorBuffHealthThreshold)
        {
            StartCoroutine(ArmorBuffRoutine());
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

        if (isAttacking || isBlocking || isCastingLaser) return;

        if (player != null)
        {
            targetPlayer = player;
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer > attackRange)
                {
                    MoveTowardsPlayer();
                }
                else
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animator.SetBool(ANIM_MOVING, false);
                }

                FlipTowardsPlayer();

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    DecideAttack(distanceToPlayer);
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

    private void MoveTowardsPlayer()
    {
        if (targetPlayer == null) return;

        Vector2 direction = (targetPlayer.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        animator.SetBool(ANIM_MOVING, true);
        animator.SetBool(ANIM_IDLE, false);
    }

    private void FlipTowardsPlayer()
    {
        if (targetPlayer == null) return;

        if (targetPlayer.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void DecideAttack(float distanceToPlayer)
    {
        if (Random.value < 0.2f)
        {
            StartCoroutine(LaserAttackRoutine());
        }
        else if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(MeleeAttackRoutine());
        }
        else if (distanceToPlayer <= rangedAttackRange)
        {
            StartCoroutine(RangedAttackRoutine());
        }
    }

    private IEnumerator MeleeAttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger(ANIM_MELEE_ATTACK);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(meleeAttackDelay);

        PerformMeleeAttack();

        yield return new WaitForSeconds(0.283f);

        isAttacking = false;
    }

    private void PerformMeleeAttack()
    {
        if (meleeAttackTransform == null)
        {
            Debug.LogWarning("Melee Attack Transform is not assigned!");
            return;
        }

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(meleeAttackTransform.position, meleeAttackArea, 0, playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            if (playerCollider.CompareTag("Player"))
            {
                if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
                {
                    PlayerController.Instance.TakeDamage(meleeAttackDamage);
                    PlayerController.Instance.HitStopTime(0, 5, 0.5f);

                    Vector2 knockbackDirection = (playerCollider.transform.position - transform.position).normalized;
                    Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.AddForce(knockbackDirection * 10f, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    private IEnumerator RangedAttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger(ANIM_RANGED_ATTACK);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        FireRangedProjectile();

        yield return new WaitForSeconds(0.25f);

        isAttacking = false;
    }

    private void FireRangedProjectile()
    {
        if (rangedProjectilePrefab == null || projectileSpawnPoint == null) return;

        GameObject projectile = Instantiate(rangedProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        Vector2 direction = (targetPlayer.position - projectileSpawnPoint.position).normalized;

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            projectileRb.velocity = direction * projectileSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        BossProjectile projScript = projectile.GetComponent<BossProjectile>();
        if (projScript != null)
        {
            projScript.damage = rangedAttackDamage;
        }
    }

    private IEnumerator LaserAttackRoutine()
    {
        isAttacking = true;
        isCastingLaser = true;
        lastAttackTime = Time.time;

        animator.SetTrigger(ANIM_GLOWING);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.667f);

        animator.SetTrigger(ANIM_LASER_CAST);

        yield return new WaitForSeconds(laserCastTime);

        animator.SetTrigger(ANIM_LASER_BEAM);
        FireLaser();

        yield return new WaitForSeconds(laserDuration);

        if (activeLaserBeam != null)
        {
            Destroy(activeLaserBeam);
        }

        isAttacking = false;
        isCastingLaser = false;
    }

    private void FireLaser()
    {
        if (laserBeamPrefab == null || laserSpawnPoint == null) return;

        activeLaserBeam = Instantiate(laserBeamPrefab, laserSpawnPoint.position, laserSpawnPoint.rotation);
        activeLaserBeam.transform.SetParent(transform);

        BossLaser laserScript = activeLaserBeam.GetComponent<BossLaser>();
        if (laserScript != null)
        {
            laserScript.damage = laserDamage;
            laserScript.knockbackForce = laserKnockbackForce;
        }
    }

    private IEnumerator BlockRoutine()
    {
        isBlocking = true;
        lastBlockTime = Time.time;

        animator.SetTrigger(ANIM_BLOCK);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
    }

    private IEnumerator ArmorBuffRoutine()
    {
        hasUsedArmorBuff = true;
        isAttacking = true;

        animator.SetTrigger(ANIM_ARMOR_BUFF);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(armorBuffDuration);

        hasArmor = true;

        isAttacking = false;
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (isDead) return;

        if (isBlocking)
        {
            return;
        }

        if (Time.time >= lastBlockTime + blockCooldown && Random.value < blockChance && !isAttacking)
        {
            StartCoroutine(BlockRoutine());
            return;
        }

        float actualDamage = hasArmor ? _damageDone * armorMultiplier : _damageDone;
        health -= actualDamage;

        if (!isRecoiling && !isAttacking)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    protected override void Attack()
    {
        if (targetPlayer == null) return;
        PlayerController.Instance.TakeDamage(damage);
    }

    private void Die()
    {
        isDead = true;
        isAttacking = false;
        isBlocking = false;
        isCastingLaser = false;

        if (activeLaserBeam != null)
        {
            Destroy(activeLaserBeam);
        }

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        GetComponent<Collider2D>().enabled = false;

        animator.SetTrigger(ANIM_DEFEATED);

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1.167f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRange);

        if (meleeAttackTransform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(meleeAttackTransform.position, meleeAttackArea);
        }
    }
}
