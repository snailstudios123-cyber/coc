using System.Collections;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    public float damage = 25f;
    public float knockbackForce = 20f;
    [SerializeField] private float damageTickRate = 0.2f;

    private float lastDamageTime;
    private BoxCollider2D laserCollider;
    private float duration;
    private bool isFlipped;

    private void Awake()
    {
        laserCollider = GetComponent<BoxCollider2D>();
        if (laserCollider == null)
        {
            laserCollider = gameObject.AddComponent<BoxCollider2D>();
            laserCollider.isTrigger = true;
        }
    }

    public void Initialize(float laserDamage, float laserDuration, bool flipX)
    {
        damage = laserDamage;
        duration = laserDuration;
        isFlipped = flipX;

        if (isFlipped)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= lastDamageTime + damageTickRate)
        {
            if (PlayerController.Instance != null && !PlayerController.Instance.pState.invincible)
            {
                PlayerController.Instance.TakeDamage(damage);

                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }

                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
                lastDamageTime = Time.time;
            }
        }
    }
}
