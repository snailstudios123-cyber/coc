using UnityEngine;

public class VineShield : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Color vineColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseIntensity = 0.2f;

    private SpriteRenderer spriteRenderer;
    private float startTime;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = vineColor;
            originalColor = vineColor;
        }

        startTime = Time.time;
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            float pulse = Mathf.Sin((Time.time - startTime) * pulseSpeed) * pulseIntensity;
            spriteRenderer.color = new Color(
                originalColor.r + pulse,
                originalColor.g + pulse,
                originalColor.b + pulse,
                originalColor.a
            );
        }
    }
}
