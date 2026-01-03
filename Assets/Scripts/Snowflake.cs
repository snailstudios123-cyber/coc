using UnityEngine;

public class Snowflake : MonoBehaviour
{
    private float fallSpeed = 2f;
    private float damage = 5f;
    private float freezeDuration = 3f;
    private float sidewaysDrift = 0.5f;
    private float driftSpeed = 1f;
    
    private Vector2 direction;
    private bool hasHitEnemy = false;

    public void Initialize(float speed, float dmg, float freezeTime)
    {
        fallSpeed = speed;
        damage = dmg;
        freezeDuration = freezeTime;
        
        sidewaysDrift = Random.Range(-0.5f, 0.5f);
        driftSpeed = Random.Range(0.5f, 1.5f);
    }

    private void Start()
    {
        float randomRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, randomRotation);
    }

    private void Update()
    {
        float drift = Mathf.Sin(Time.time * driftSpeed) * sidewaysDrift;
        direction = new Vector2(drift, -fallSpeed);
        
        transform.position += (Vector3)direction * Time.deltaTime;
        
        transform.Rotate(0, 0, 30f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHitEnemy) return;

        if (collision.CompareTag("Enemy"))
        {
            hasHitEnemy = true;
            FreezeEnemy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

    private void FreezeEnemy(GameObject enemy)
    {
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            Vector2 hitDirection = Vector2.down;
            enemyComponent.EnemyHit(damage, hitDirection, 0f);
        }

        FreezeEffect freezeEffect = enemy.GetComponent<FreezeEffect>();
        if (freezeEffect == null)
        {
            freezeEffect = enemy.AddComponent<FreezeEffect>();
        }

        freezeEffect.ApplyFreeze(freezeDuration);
    }
}
