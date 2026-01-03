using UnityEngine;

public class BeamChargeEffect : MonoBehaviour
{
    [Header("Charge Visual")]
    [SerializeField] private SpriteRenderer glowSprite;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private Color chargeColor = new Color(0, 1, 1, 0.8f);
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem chargeParticles;
    
    private float pulseTimer = 0f;
    
    private void Awake()
    {
        if (glowSprite == null)
        {
            glowSprite = GetComponent<SpriteRenderer>();
        }
        
        if (glowSprite != null)
        {
            glowSprite.color = chargeColor;
        }
    }
    
    private void Update()
    {
        pulseTimer += Time.deltaTime * pulseSpeed;
        
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(pulseTimer) + 1f) / 2f);
        
        if (glowSprite != null)
        {
            transform.localScale = Vector3.one * scale;
        }
    }
    
    private void OnDestroy()
    {
        if (chargeParticles != null)
        {
            chargeParticles.Stop();
        }
    }
}
