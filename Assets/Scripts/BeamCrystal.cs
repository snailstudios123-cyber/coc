using UnityEngine;
using UnityEngine.Events;

public class BeamCrystal : MonoBehaviour
{
    [Header("Settings")]
    public float activationDuration = 5f;
    public bool staysActivated = false;
    
    [Header("Visual Feedback")]
    public SpriteRenderer crystalRenderer;
    public Color inactiveColor = new Color(0.3f, 0.3f, 1f, 1f);
    public Color activeColor = new Color(0f, 1f, 1f, 1f);
    public GameObject glowEffect;
    
    [Header("Events")]
    public UnityEvent onActivated;
    public UnityEvent onDeactivated;
    
    private bool isActive = false;
    private float activeTimer = 0f;
    
    private void Start()
    {
        if (crystalRenderer == null)
        {
            crystalRenderer = GetComponent<SpriteRenderer>();
        }
        
        UpdateVisuals();
    }
    
    private void Update()
    {
        if (isActive && !staysActivated)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0f)
            {
                Deactivate();
            }
        }
    }
    
    public void HitByBeam()
    {
        Activate();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Activate();
        }
    }
    
    private void Activate()
    {
        if (!isActive || !staysActivated)
        {
            isActive = true;
            activeTimer = activationDuration;
            UpdateVisuals();
            onActivated?.Invoke();
            Debug.Log($"[BeamCrystal] {name} activated");
        }
    }
    
    private void Deactivate()
    {
        if (isActive)
        {
            isActive = false;
            UpdateVisuals();
            onDeactivated?.Invoke();
            Debug.Log($"[BeamCrystal] {name} deactivated");
        }
    }
    
    private void UpdateVisuals()
    {
        if (crystalRenderer != null)
        {
            crystalRenderer.color = isActive ? activeColor : inactiveColor;
        }
        
        if (glowEffect != null)
        {
            glowEffect.SetActive(isActive);
        }
    }
    
    public bool IsActive()
    {
        return isActive;
    }
}
