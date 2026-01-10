using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationSpell : MonoBehaviour
{
    [Header("Spell Data")]
    [SerializeField] private SpellData spellData;
    
    [Header("Levitation Settings")]
    [SerializeField] private float levitationRange = 10f;
    [SerializeField] private float levitationHeight = 3f;
    [SerializeField] private float liftSpeed = 5f;
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private LayerMask levitatableLayer;

    [Header("Visual Effects")]
    [SerializeField] private GameObject levitationEffect;
    [SerializeField] private LineRenderer levitationBeam;
    [SerializeField] private Color beamColor = new Color(0.5f, 0f, 1f, 0.5f);

    private GameObject levitatedObject;
    private Rigidbody2D levitatedRb;
    private Vector3 targetPosition;
    private float originalGravityScale;
    private bool isLevitating = false;

    private void Start()
    {
        if (levitationBeam != null)
        {
            levitationBeam.enabled = false;
            levitationBeam.startColor = beamColor;
            levitationBeam.endColor = beamColor;
            levitationBeam.startWidth = 0.1f;
            levitationBeam.endWidth = 0.1f;
        }
    }

    private void Update()
    {
        if (isLevitating && levitatedObject != null)
        {
            UpdateLevitatedObject();
            UpdateVisuals();
        }
    }

    public bool TryLevitateObject()
    {
        if (spellData != null && !spellData.isEquipped)
        {
            return false;
        }
        
        if (isLevitating)
        {
            return false;
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0;

        Vector2 direction = (worldMousePos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, levitationRange, levitatableLayer);

        if (hit.collider != null)
        {
            GameObject target = hit.collider.gameObject;
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                StartLevitation(target, rb);
                return true;
            }
        }

        return false;
    }

    private void StartLevitation(GameObject target, Rigidbody2D rb)
    {
        levitatedObject = target;
        levitatedRb = rb;
        originalGravityScale = rb.gravityScale;

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        isLevitating = true;

        if (levitationBeam != null)
        {
            levitationBeam.enabled = true;
        }

        if (levitationEffect != null)
        {
            Instantiate(levitationEffect, levitatedObject.transform.position, Quaternion.identity, levitatedObject.transform);
        }
        
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("CastInstant");
        }

        Debug.Log($"Started levitating: {target.name}");
    }

    public void OnInstantSpellCast()
    {
    }

    private void UpdateLevitatedObject()
    {
        if (levitatedObject == null || levitatedRb == null)
        {
            ReleaseLevitation();
            return;
        }

        // Calculate target position relative to player
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0;

        // Keep object at a fixed distance from player, following mouse direction
        Vector2 directionToMouse = (worldMousePos - transform.position).normalized;
        targetPosition = (Vector2)transform.position + directionToMouse * holdDistance;
        targetPosition.y += levitationHeight;

        // Smoothly move object to target position
        Vector2 newPosition = Vector2.Lerp(levitatedRb.position, targetPosition, Time.deltaTime * liftSpeed);
        levitatedRb.MovePosition(newPosition);
    }

    private void UpdateVisuals()
    {
        if (levitationBeam != null && levitatedObject != null)
        {
            levitationBeam.SetPosition(0, transform.position);
            levitationBeam.SetPosition(1, levitatedObject.transform.position);
        }
    }

    public void ReleaseLevitation()
    {
        if (!isLevitating)
        {
            return;
        }

        if (levitatedRb != null)
        {
            levitatedRb.gravityScale = originalGravityScale;
        }

        if (levitationBeam != null)
        {
            levitationBeam.enabled = false;
        }

        levitatedObject = null;
        levitatedRb = null;
        isLevitating = false;

        Debug.Log("Released levitation");
    }

    public void ThrowLevitatedObject(float throwForce = 15f)
    {
        if (!isLevitating || levitatedObject == null || levitatedRb == null)
        {
            return;
        }

        // Calculate throw direction toward mouse
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0;

        Vector2 throwDirection = (worldMousePos - levitatedObject.transform.position).normalized;

        // Restore physics
        levitatedRb.gravityScale = originalGravityScale;

        // Apply throw force
        levitatedRb.velocity = throwDirection * throwForce;

        if (levitationBeam != null)
        {
            levitationBeam.enabled = false;
        }

        Debug.Log($"Threw object: {levitatedObject.name}");

        levitatedObject = null;
        levitatedRb = null;
        isLevitating = false;
    }

    public bool IsLevitating()
    {
        return isLevitating;
    }

    public GameObject GetLevitatedObject()
    {
        return levitatedObject;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, levitationRange);
    }
}