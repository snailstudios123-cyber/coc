using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Settings")]
    public bool requiresWeight = true;
    public float activationDelay = 0.2f;
    
    [Header("Visual Feedback")]
    public SpriteRenderer plateRenderer;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.gray;
    
    [Header("Events")]
    public UnityEvent onActivated;
    public UnityEvent onDeactivated;
    
    private bool isActive = false;
    private int objectsOnPlate = 0;
    private float activationTimer = 0f;
    
    private void Start()
    {
        if (plateRenderer == null)
        {
            plateRenderer = GetComponent<SpriteRenderer>();
        }
        
        UpdateVisuals();
    }
    
    private void Update()
    {
        if (objectsOnPlate > 0 && !isActive)
        {
            activationTimer += Time.deltaTime;
            if (activationTimer >= activationDelay)
            {
                Activate();
            }
        }
        else if (objectsOnPlate == 0 && isActive)
        {
            Deactivate();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Levitatable"))
        {
            objectsOnPlate++;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Levitatable"))
        {
            objectsOnPlate = Mathf.Max(0, objectsOnPlate - 1);
            activationTimer = 0f;
        }
    }
    
    private void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            UpdateVisuals();
            onActivated?.Invoke();
            Debug.Log($"[PressurePlate] {name} activated");
        }
    }
    
    private void Deactivate()
    {
        if (isActive)
        {
            isActive = false;
            activationTimer = 0f;
            UpdateVisuals();
            onDeactivated?.Invoke();
            Debug.Log($"[PressurePlate] {name} deactivated");
        }
    }
    
    private void UpdateVisuals()
    {
        if (plateRenderer != null)
        {
            plateRenderer.color = isActive ? activeColor : inactiveColor;
        }
    }
    
    public bool IsActive()
    {
        return isActive;
    }
}
