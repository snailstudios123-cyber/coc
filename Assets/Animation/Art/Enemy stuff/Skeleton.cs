using System.Collections;
using UnityEngine;

public class Skeleton : Enemy
{
    [Header("Skeleton Settings")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float losePlayerRange = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float patrolDistance = 5f;
    
    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackArea = new Vector2(1.5f, 1.5f);
    [SerializeField] private Vector3 attackPointOffset = new Vector3(0.7f, 0.5f, 0f);
    [SerializeField] private float attackWarningTime = 0.3f;
    
    private EnemyAttackWarning attackWarning;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector2 startPosition;
    private bool movingRight = true;
    private float attackTimer;
    private bool isAttacking;
    private bool isDead;
    private float flipCooldown = 0f;
    private float stateChangeTimer = 0f;
    private Vector3 attackLockedPosition;
    private const float FLIP_COOLDOWN_TIME = 0.2f;
    private const float STATE_CHANGE_COOLDOWN = 0.3f;
    
    private enum State { Idle, Patrol, Chase, Attack, Hurt, Dead, ReturnToStart }
    private State currentState = State.Patrol;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        if (anim != null)
        {
            anim.applyRootMotion = false;
            anim.updateMode = AnimatorUpdateMode.Normal;
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
        
        if (anim == null)
        {
            Debug.LogError("Skeleton: Animator component is missing!");
        }
        else if (anim.runtimeAnimatorController == null)
        {
            Debug.LogError("Skeleton: Animator Controller is not assigned!");
        }
    }

    protected override void Start()
    {
        base.Start();
        attackTimer = attackCooldown;
        UpdateAttackPointPosition();
        
        if (attackPoint != null)
        {
            attackWarning = attackPoint.GetComponent<EnemyAttackWarning>();
            if (attackWarning == null)
            {
                attackWarning = attackPoint.gameObject.AddComponent<EnemyAttackWarning>();
            }
        }
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
        {
            return;
        }

        if (isDead)
        {
            return;
        }

        if (health <= 0)
        {
            Die();
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
                currentState = State.Idle;
                stateChangeTimer = 0f;
            }
            return;
        }
        
        if (isConfused)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        attackTimer += Time.deltaTime;
        flipCooldown -= Time.deltaTime;
        stateChangeTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (distanceToPlayer <= detectionRange && stateChangeTimer >= STATE_CHANGE_COOLDOWN)
                {
                    currentState = State.Chase;
                    stateChangeTimer = 0f;
                }
                break;

            case State.Chase:
                if (distanceToPlayer > losePlayerRange && stateChangeTimer >= STATE_CHANGE_COOLDOWN)
                {
                    float distanceFromStart = Vector2.Distance(transform.position, startPosition);
                    if (distanceFromStart > patrolDistance * 0.5f)
                    {
                        currentState = State.ReturnToStart;
                        stateChangeTimer = 0f;
                    }
                    else
                    {
                        currentState = State.Patrol;
                        stateChangeTimer = 0f;
                        ResetPatrolDirection();
                    }
                }
                else if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attack;
                    stateChangeTimer = 0f;
                }
                else
                {
                    Chase();
                }
                break;

            case State.ReturnToStart:
                ReturnToStart();
                if (Vector2.Distance(transform.position, startPosition) < 0.5f && stateChangeTimer >= STATE_CHANGE_COOLDOWN)
                {
                    currentState = State.Patrol;
                    stateChangeTimer = 0f;
                    ResetPatrolDirection();
                }
                else if (distanceToPlayer <= detectionRange && stateChangeTimer >= STATE_CHANGE_COOLDOWN)
                {
                    currentState = State.Chase;
                    stateChangeTimer = 0f;
                }
                break;

            case State.Attack:
                if (!isAttacking && attackTimer >= attackCooldown)
                {
                    StartCoroutine(AttackSequence());
                }
                else if (isAttacking)
                {
                }
                else
                {
                    currentState = State.Idle;
                    stateChangeTimer = 0f;
                }
                break;

            case State.Idle:
                rb.velocity = new Vector2(0, rb.velocity.y);
                if (anim != null)
                {
                    anim.SetBool("isWalking", false);
                }
                if (distanceToPlayer <= detectionRange && stateChangeTimer >= STATE_CHANGE_COOLDOWN)
                {
                    currentState = State.Chase;
                    stateChangeTimer = 0f;
                }
                else if (attackTimer >= attackCooldown * 0.5f && stateChangeTimer >= STATE_CHANGE_COOLDOWN)
                {
                    currentState = State.Patrol;
                    stateChangeTimer = 0f;
                }
                break;
        }
    }

    private void Patrol()
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", true);
        }
        
        float distanceFromStart = Mathf.Abs(transform.position.x - startPosition.x);
        
        if (distanceFromStart >= patrolDistance && flipCooldown <= 0f)
        {
            movingRight = !movingRight;
            Flip();
            flipCooldown = FLIP_COOLDOWN_TIME;
        }

        float direction = movingRight ? 1 : -1;
        rb.velocity = new Vector2(direction * patrolSpeed, rb.velocity.y);
    }

    private void ResetPatrolDirection()
    {
        float directionToStart = transform.position.x > startPosition.x ? -1 : 1;
        bool shouldMoveRight = directionToStart > 0;
        
        if (movingRight != shouldMoveRight)
        {
            movingRight = shouldMoveRight;
            Flip();
        }
    }

    private void Chase()
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", true);
        }
        
        float direction = PlayerController.Instance.transform.position.x > transform.position.x ? 1 : -1;
        
        if (flipCooldown <= 0f)
        {
            if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
            {
                movingRight = !movingRight;
                Flip();
                flipCooldown = FLIP_COOLDOWN_TIME;
            }
        }

        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);
    }
    
    private void ReturnToStart()
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", true);
        }
        
        float direction = startPosition.x > transform.position.x ? 1 : -1;
        
        if (flipCooldown <= 0f)
        {
            if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
            {
                movingRight = !movingRight;
                Flip();
                flipCooldown = FLIP_COOLDOWN_TIME;
            }
        }

        rb.velocity = new Vector2(direction * patrolSpeed, rb.velocity.y);
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        attackLockedPosition = transform.position;
        
        RigidbodyType2D originalBodyType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        
        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetTrigger("attack1");
        }
        
        if (attackWarning != null)
        {
            attackWarning.StartAttackWarning(attackWarningTime);
        }
        
        float waitTime = 0f;
        float attackDuration = 0.8f;
        while (waitTime < attackDuration)
        {
            waitTime += Time.deltaTime;
            transform.position = attackLockedPosition;
            rb.velocity = Vector2.zero;
            
            yield return null;
        }
        
        if (attackWarning != null)
        {
            attackWarning.EndAttackWarning();
        }
        
        rb.bodyType = originalBodyType;
        
        attackTimer = 0;
        isAttacking = false;
        attackLockedPosition = Vector3.zero;
        currentState = State.Idle;
        stateChangeTimer = 0f;
    }
    
    private void FixedUpdate()
    {
    }
    
    private void LateUpdate()
    {
    }

    private void PerformAttack()
    {
        if (attackPoint == null)
        {
            return;
        }

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, attackArea, 0f);
        
        foreach (Collider2D player in hitPlayers)
        {
            if (player.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
            {
                Attack();
            }
        }
    }

    protected override void Attack()
    {
        Vector2 knockbackDirection = (PlayerController.Instance.transform.position - transform.position).normalized;
        knockbackDirection.y = Mathf.Max(knockbackDirection.y, 0.5f);
        
        PlayerController.Instance.TakeDamageWithKnockback(damage, knockbackDirection);
    }
    
    private void OnAttackAnimationHit()
    {
        PerformAttack();
    }

    private void OnAttackAnimationEnd()
    {
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (isDead)
        {
            return;
        }

        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
        
        if (health > 0)
        {
            if (anim != null)
            {
                anim.SetTrigger("hurt");
            }
            currentState = State.Hurt;
            isRecoiling = true;
            
            if (isAttacking)
            {
                StopAllCoroutines();
                isAttacking = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
    
    protected override void OnConfusionEnded()
    {
        base.OnConfusionEnded();
        
        currentState = State.Idle;
        stateChangeTimer = 0f;
        
        if (isAttacking)
        {
            StopAllCoroutines();
            isAttacking = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        
        rb.velocity = new Vector2(0, rb.velocity.y);
        
        Debug.Log($"[Skeleton] State reset after confusion - now in {currentState} state");
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        GetComponent<Collider2D>().enabled = false;
        
        DropCoins();
        
        if (anim != null)
        {
            anim.SetTrigger("die");
        }
        
        Destroy(gameObject, 1.5f);
    }

    private void Flip()
    {
        sr.flipX = !movingRight;
        UpdateAttackPointPosition();
    }

    private void UpdateAttackPointPosition()
    {
        if (attackPoint != null)
        {
            float xOffset = movingRight ? attackPointOffset.x : -attackPointOffset.x;
            attackPoint.localPosition = new Vector3(xOffset, attackPointOffset.y, attackPointOffset.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(attackPoint.position, attackArea);
        }
    }
}
