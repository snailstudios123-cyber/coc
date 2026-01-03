using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float damage = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float hitForce = 10f;

    private Vector2 direction;
    private float speed;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 moveDirection, float projectileSpeed, float projectileDamage)
    {
        direction = moveDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;

        if (rb != null)
        {
            rb.velocity = direction * speed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
            {
                PlayerController.Instance.TakeDamage(damage);
                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
