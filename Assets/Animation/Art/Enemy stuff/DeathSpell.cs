using System.Collections;
using UnityEngine;

public class DeathSpell : MonoBehaviour
{
    [Header("Spell Settings")]
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackArea = new Vector2(1.5f, 2f);
    [SerializeField] private Vector3 attackPointOffset = new Vector3(0f, 1f, 0f);
    
    private float damage;
    private bool hasAttacked = false;
    private Animator anim;
    private Rigidbody2D rb;
    private const int SPELL_FRAME_COUNT = 16;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        if (anim != null)
        {
            anim.SetTrigger("spell");
        }
        
        StartCoroutine(AttackSequence());
        
        Destroy(gameObject, lifetime);
    }

    public void Initialize(float _damage)
    {
        damage = _damage;
    }

    private IEnumerator AttackSequence()
    {
        yield return new WaitForSeconds(attackDelay);
        
        if (!hasAttacked)
        {
            PerformAttack();
            hasAttacked = true;
        }
    }

    private void PerformAttack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("DeathSpell: Attack point not assigned!");
            return;
        }

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, attackArea, 0f);
        
        foreach (Collider2D player in hitPlayers)
        {
            if (player.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
            {
                DealDamageToPlayer();
                break;
            }
        }
    }

    private void DealDamageToPlayer()
    {
        if (PlayerController.Instance != null)
        {
            Vector2 knockbackDirection = new Vector2(0f, 1f);
            PlayerController.Instance.TakeDamageWithKnockback(damage, knockbackDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackPoint.position, attackArea);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + attackPointOffset, attackArea);
        }
    }
}
