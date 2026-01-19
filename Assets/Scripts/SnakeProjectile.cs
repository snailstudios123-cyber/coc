using UnityEngine;

public class SnakeProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float knockbackForce = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                PlayerController.Instance.TakeDamageWithKnockback(damage, knockbackDirection * knockbackForce);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
