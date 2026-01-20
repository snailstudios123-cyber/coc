using UnityEngine;

public class HazardTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    public int damageAmount = 1;
    public float damageInterval = 1f;
    public bool requiresShieldToPass = true;
    
    [Header("Trap Type")]
    public TrapType trapType = TrapType.Fire;
    
    [Header("Visual")]
    public SpriteRenderer trapRenderer;
    public Color fireColor = new Color(1f, 0.3f, 0f, 1f);
    public Color spikeColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public GameObject effectPrefab;
    
    private float damageTimer = 0f;
    
    public enum TrapType
    {
        Fire,
        Spike,
        Poison,
        Electric
    }
    
    private void Start()
    {
        if (trapRenderer == null)
        {
            trapRenderer = GetComponent<SpriteRenderer>();
        }
        
        UpdateVisuals();
    }
    
    private void Update()
    {
        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && damageTimer <= 0f)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                bool hasShield = CheckPlayerHasShield(player);
                
                if (!requiresShieldToPass || !hasShield)
                {
                    player.TakeDamage(damageAmount);
                    damageTimer = damageInterval;
                    Debug.Log($"[HazardTrap] {trapType} trap damaged player");
                }
                else
                {
                    Debug.Log($"[HazardTrap] Player protected by shield");
                }
            }
        }
    }
    
    private bool CheckPlayerHasShield(PlayerController player)
    {
        VineShield shield = player.GetComponentInChildren<VineShield>();
        return shield != null && shield.isActiveAndEnabled;
    }
    
    private void UpdateVisuals()
    {
        if (trapRenderer != null)
        {
            switch (trapType)
            {
                case TrapType.Fire:
                    trapRenderer.color = fireColor;
                    break;
                case TrapType.Spike:
                    trapRenderer.color = spikeColor;
                    break;
                case TrapType.Poison:
                    trapRenderer.color = Color.green;
                    break;
                case TrapType.Electric:
                    trapRenderer.color = Color.yellow;
                    break;
            }
        }
    }
}
