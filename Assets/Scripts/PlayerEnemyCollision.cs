using UnityEngine;

public class PlayerEnemyCollision : MonoBehaviour
{
    [Header("Contact Damage Settings")]
    [SerializeField] private float contactDamage = 1f;
    [SerializeField] private float contactDamageCooldown = 0.5f;
    
    private PlayerController playerController;
    private PlayerStateList pState;
    private float lastContactDamageTime = -999f;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        pState = GetComponent<PlayerStateList>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamageFromEnemy(collision);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (Time.time >= lastContactDamageTime + contactDamageCooldown)
            {
                TakeDamageFromEnemy(collision);
            }
        }
    }

    void TakeDamageFromEnemy(Collision2D collision)
    {
        if (pState.invincible) return;

        lastContactDamageTime = Time.time;
        
        Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
        knockbackDirection.y = Mathf.Max(knockbackDirection.y, 0.5f);
        
        playerController.TakeDamageWithKnockback(contactDamage, knockbackDirection);
    }
}
