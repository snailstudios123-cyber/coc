using UnityEngine;

public class SpellCastController : MonoBehaviour
{
    [Header("Spell Cast Settings")]
    [SerializeField] private Transform spellSpawnPoint;
    
    [Header("Audio")]
    [SerializeField] private AudioClip instantCastSound;
    [SerializeField] private AudioClip chargeSound;
    [SerializeField] private AudioClip releaseSound;
    private AudioSource audioSource;
    
    [Header("VFX")]
    [SerializeField] private GameObject chargingEffect;
    private GameObject activeChargingEffect;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnInstantSpellCast()
    {
        Debug.Log("[SpellCast] Instant spell fired!");
        
        if (instantCastSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(instantCastSound);
        }
    }

    public void OnChargingLoopStart()
    {
        Debug.Log("[SpellCast] Charging started");
        
        if (chargeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(chargeSound);
        }
        
        if (chargingEffect != null && activeChargingEffect == null)
        {
            activeChargingEffect = Instantiate(chargingEffect, transform);
        }
    }

    public void OnSpellRelease()
    {
        Debug.Log("[SpellCast] Spell released!");
        
        if (releaseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }
        
        if (activeChargingEffect != null)
        {
            Destroy(activeChargingEffect);
            activeChargingEffect = null;
        }
    }
}

