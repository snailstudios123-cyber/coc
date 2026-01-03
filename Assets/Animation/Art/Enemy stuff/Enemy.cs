using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected float speed;

    [SerializeField] protected float damage;
    
    [Header("Coin Drop Settings")]
    [SerializeField] protected GameObject coinPrefab;
    [SerializeField] protected int minCoins = 1;
    [SerializeField] protected int maxCoins = 3;
    [SerializeField] protected float coinDropChance = 0.8f;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (maxHealth == 0)
        {
            maxHealth = health;
        }
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
        {
            return;
        }

        if(health <= 0)
        {
            DropCoins();
            Destroy(gameObject);
        }
        if(isRecoiling)
        {
            if(recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        
        if(!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
        }
    }
    protected void OnCollisionStay2D(Collision2D _other)
    {
        if(_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
            PlayerController.Instance.HitStopTime(0, 5, 0.5f);
        }
    }
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
    
    protected virtual void DropCoins()
    {
        if (coinPrefab == null || Random.value > coinDropChance)
        {
            return;
        }

        int coinsToDrop = Random.Range(minCoins, maxCoins + 1);
        
        for (int i = 0; i < coinsToDrop; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }
    
    public float GetHealth()
    {
        return health;
    }
    
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
