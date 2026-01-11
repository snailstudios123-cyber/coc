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
    
    [Header("Confusion Settings")]
    [SerializeField] protected Sprite confusionSprite;
    [SerializeField] protected float confusionIndicatorHeight = 2f;
    [SerializeField] protected float confusionIndicatorScale = 0.5f;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    
    protected bool isConfused = false;
    protected float confusionTimer;
    protected float confusionDuration;
    protected GameObject confusionIndicator;
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
        if(health <= 0)
        {
            DropCoins();
            Destroy(gameObject);
            return;
        }
        
        if(isConfused)
        {
            if(confusionTimer < confusionDuration)
            {
                confusionTimer += Time.deltaTime;
            }
            else
            {
                Debug.Log($"[Enemy] {gameObject.name} confusion ended (timer: {confusionTimer:F2}s / duration: {confusionDuration:F2}s)");
                isConfused = false;
                confusionTimer = 0;
                
                OnConfusionEnded();
                
                if (confusionIndicator != null)
                {
                    Destroy(confusionIndicator);
                    confusionIndicator = null;
                }
            }
        }
        
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
        {
            return;
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
    
    public void ApplyConfusion(float duration)
    {
        isConfused = true;
        confusionDuration = duration;
        confusionTimer = 0;
        
        if (confusionIndicator == null)
        {
            CreateConfusionIndicator();
        }
        
        Debug.Log($"{gameObject.name} is confused for {duration} seconds!");
    }
    
    private void CreateConfusionIndicator()
    {
        GameObject indicator = new GameObject("ConfusionIndicator");
        indicator.transform.SetParent(transform);
        indicator.transform.localPosition = new Vector3(0, confusionIndicatorHeight, 0);
        
        SpriteRenderer sr = indicator.AddComponent<SpriteRenderer>();
        sr.sprite = confusionSprite != null ? confusionSprite : CreateQuestionMarkSprite();
        sr.color = new Color(1f, 1f, 0.2f, 0.9f);
        sr.sortingOrder = 100;
        
        indicator.transform.localScale = new Vector3(confusionIndicatorScale, confusionIndicatorScale, 1f);
        
        ConfusionEffect effect = indicator.AddComponent<ConfusionEffect>();
        
        confusionIndicator = indicator;
    }
    
    private Sprite CreateQuestionMarkSprite()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float centerX = 16f;
                float centerY = 16f;
                float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                
                if (distance < 12f && distance > 8f && y > centerY)
                {
                    pixels[y * 32 + x] = Color.white;
                }
                else if (x > 14 && x < 18 && y > 4 && y < 12)
                {
                    pixels[y * 32 + x] = Color.white;
                }
                else if (x > 14 && x < 18 && y > 0 && y < 4)
                {
                    pixels[y * 32 + x] = Color.white;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
    }
    
    public bool IsConfused()
    {
        return isConfused;
    }
    
    public float GetConfusionTimeRemaining()
    {
        if (!isConfused) return 0f;
        return Mathf.Max(0, confusionDuration - confusionTimer);
    }
    
    protected virtual void OnConfusionEnded()
    {
    }
}
