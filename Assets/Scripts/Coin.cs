using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attractDistance = 3f;
    [SerializeField] private float collectDistance = 0.5f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float bounciness = 0.5f;
    
    private Transform player;
    private bool isBeingAttracted = false;
    private Rigidbody2D rb;
    private CircleCollider2D coinCollider;
    private float lifeTimer;
    private bool hasLanded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        coinCollider = GetComponent<CircleCollider2D>();
        if (coinCollider == null)
        {
            coinCollider = gameObject.AddComponent<CircleCollider2D>();
            coinCollider.radius = 0.3f;
        }
        
        coinCollider.isTrigger = false;
        
        PhysicsMaterial2D bounceMaterial = new PhysicsMaterial2D
        {
            bounciness = bounciness,
            friction = 0.4f
        };
        coinCollider.sharedMaterial = bounceMaterial;
    }

    private void Start()
    {
        if (PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
        }
        
        lifeTimer = lifetime;
        
        Vector2 randomForce = new Vector2(Random.Range(-2f, 2f), Random.Range(3f, 5f));
        rb.AddForce(randomForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (player == null && PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
        }

        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= collectDistance)
        {
            CollectCoin();
            return;
        }

        if (distanceToPlayer <= attractDistance)
        {
            if (!isBeingAttracted)
            {
                isBeingAttracted = true;
                rb.gravityScale = 0f;
                coinCollider.isTrigger = true;
            }
            
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        else if (isBeingAttracted)
        {
            isBeingAttracted = false;
            rb.gravityScale = 1f;
            coinCollider.isTrigger = false;
        }

        if (Mathf.Abs(rb.velocity.y) < 0.1f && !isBeingAttracted)
        {
            hasLanded = true;
        }

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void CollectCoin()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddCoins(coinValue);
        }
        
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectCoin();
        }
    }
}
