using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isLocked = true;
    public float openSpeed = 2f;
    public Vector3 closedPosition;
    public Vector3 openPosition;
    
    [Header("Requirements")]
    public PressurePlate[] requiredPlates;
    public BeamCrystal[] requiredCrystals;
    
    [Header("Visual")]
    public SpriteRenderer doorRenderer;
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;
    
    private bool isOpening = false;
    private bool isOpen = false;
    
    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * 10f;
        
        if (doorRenderer == null)
        {
            doorRenderer = GetComponent<SpriteRenderer>();
        }
        
        UpdateVisuals();
    }
    
    private void Update()
    {
        CheckUnlockConditions();
        
        if (isOpening && !isOpen)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, openSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, openPosition) < 0.1f)
            {
                isOpen = true;
                Debug.Log($"[PuzzleDoor] {name} fully opened");
            }
        }
        else if (!isOpening && isOpen)
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, openSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, closedPosition) < 0.1f)
            {
                isOpen = false;
                Debug.Log($"[PuzzleDoor] {name} fully closed");
            }
        }
    }
    
    private void CheckUnlockConditions()
    {
        bool allConditionsMet = true;
        
        if (requiredPlates != null && requiredPlates.Length > 0)
        {
            foreach (PressurePlate plate in requiredPlates)
            {
                if (plate != null && !plate.IsActive())
                {
                    allConditionsMet = false;
                    break;
                }
            }
        }
        
        if (requiredCrystals != null && requiredCrystals.Length > 0)
        {
            foreach (BeamCrystal crystal in requiredCrystals)
            {
                if (crystal != null && !crystal.IsActive())
                {
                    allConditionsMet = false;
                    break;
                }
            }
        }
        
        if (allConditionsMet && isLocked)
        {
            Unlock();
        }
        else if (!allConditionsMet && !isLocked)
        {
            Lock();
        }
    }
    
    public void Unlock()
    {
        isLocked = false;
        isOpening = true;
        UpdateVisuals();
        Debug.Log($"[PuzzleDoor] {name} unlocked");
    }
    
    public void Lock()
    {
        isLocked = true;
        isOpening = false;
        UpdateVisuals();
        Debug.Log($"[PuzzleDoor] {name} locked");
    }
    
    private void UpdateVisuals()
    {
        if (doorRenderer != null)
        {
            doorRenderer.color = isLocked ? lockedColor : unlockedColor;
        }
    }
}
