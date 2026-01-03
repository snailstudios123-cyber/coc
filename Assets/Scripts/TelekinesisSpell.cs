using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelekinesisSpell : MonoBehaviour
{
    [Header("Spell Data")]
    [SerializeField] private SpellData spellData;
    
    [Header("Telekinesis Settings")]
    [SerializeField] private float maxGrabDistance = 10f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float throwForce = 20f;

    [Header("Detection Method")]
    [SerializeField] private bool useTag = true; // TRUE = use tag, FALSE = use layer
    [SerializeField] private string levitatableTag = "Levitatable";
    [SerializeField] private LayerMask levitatableLayer;

    [Header("Visual Settings")]
    [SerializeField] private LineRenderer telekinesisLine;
    [SerializeField] private GameObject grabEffect;
    [SerializeField] private Color lineColor = new Color(0.5f, 0f, 1f, 0.8f);

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip holdLoopSound;
    [SerializeField] private AudioClip releaseSound;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private float debugScanRadius = 20f;

    private GameObject heldObject;
    private Rigidbody2D heldObjectRb;
    private Collider2D heldObjectCollider;
    private float originalGravityScale;
    private float originalDrag;
    private Vector3 targetPosition;
    private Transform playerTransform;
    private bool isHoldingObject = false;
    private GameObject currentGrabEffect;

    private void Start()
    {
        playerTransform = PlayerController.Instance.transform;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (telekinesisLine == null)
        {
            telekinesisLine = GetComponent<LineRenderer>();
            if (telekinesisLine == null)
            {
                telekinesisLine = gameObject.AddComponent<LineRenderer>();
            }
        }

        SetupLineRenderer();
        telekinesisLine.enabled = false;

        // Debug info
        if (showDebugInfo)
        {
            Debug.Log("=== TELEKINESIS INITIALIZED ===");
            Debug.Log($"Detection Method: {(useTag ? "TAG" : "LAYER")}");

            if (useTag)
            {
                Debug.Log($"Looking for tag: '{levitatableTag}'");

                try
                {
                    GameObject[] allObjects = FindObjectsOfType<GameObject>();
                    int taggedCount = 0;
                    foreach (GameObject obj in allObjects)
                    {
                        if (obj.tag == levitatableTag)
                        {
                            taggedCount++;
                            bool hasRB = obj.GetComponent<Rigidbody2D>() != null;
                            bool hasCol = obj.GetComponent<Collider2D>() != null;
                            Debug.Log($"? Found: {obj.name} | RigidBody2D: {hasRB} | Collider2D: {hasCol}");

                            if (!hasRB) Debug.LogWarning($"  ? {obj.name} needs Rigidbody2D!");
                            if (!hasCol) Debug.LogWarning($"  ? {obj.name} needs Collider2D!");
                        }
                    }
                    Debug.Log($"Total tagged objects: {taggedCount}");
                }
                catch (UnityException)
                {
                    Debug.LogWarning($"Tag '{levitatableTag}' is not defined. Add it in Edit > Project Settings > Tags and Layers, or switch to Layer mode.");
                }
            }
            else
            {
                Debug.Log($"LayerMask value: {levitatableLayer.value}");
                if (levitatableLayer.value == 0)
                {
                    Debug.LogError("? LayerMask is set to Nothing! Switch to Tag mode or fix LayerMask.");
                }
            }
        }
    }

    private void SetupLineRenderer()
    {
        telekinesisLine.material = new Material(Shader.Find("Sprites/Default"));
        telekinesisLine.startColor = lineColor;
        telekinesisLine.endColor = lineColor;
        telekinesisLine.startWidth = 0.15f;
        telekinesisLine.endWidth = 0.05f;
        telekinesisLine.positionCount = 2;
        telekinesisLine.useWorldSpace = true;
        telekinesisLine.sortingLayerName = "Default";
        telekinesisLine.sortingOrder = 5;
    }

    public void TryGrabObject()
    {
        if (spellData != null && !spellData.isEquipped)
        {
            return;
        }
        
        if (isHoldingObject) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (showDebugInfo)
        {
            Debug.Log("=== GRAB ATTEMPT ===");
            Debug.Log($"Mouse world position: {mousePos}");
            Debug.Log($"Player position: {playerTransform.position}");
        }

        GameObject objectToGrab = null;

        if (useTag)
        {
            objectToGrab = FindObjectByTag(mousePos);
        }
        else
        {
            objectToGrab = FindObjectByLayer(mousePos);
        }

        if (objectToGrab != null)
        {
            GrabObject(objectToGrab);
        }
        else if (showDebugInfo)
        {
            Debug.Log("? No grabbable object found!");
        }
    }

    private GameObject FindObjectByTag(Vector3 mousePos)
    {
        // Get all colliders near mouse
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(mousePos, 2f);

        if (showDebugInfo)
        {
            Debug.Log($"Found {nearbyColliders.Length} colliders near mouse");
        }

        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D col in nearbyColliders)
        {
            if (showDebugInfo)
            {
                Debug.Log($"  Checking: {col.gameObject.name} | Tag: '{col.tag}'");
            }

            if (col.tag == levitatableTag)
            {
                float distToMouse = Vector2.Distance(mousePos, col.transform.position);
                float distToPlayer = Vector2.Distance(playerTransform.position, col.transform.position);

                if (showDebugInfo)
                {
                    Debug.Log($"    ? Tag matches! Dist to mouse: {distToMouse:F2}, Dist to player: {distToPlayer:F2}");
                }

                if (distToPlayer <= maxGrabDistance && distToMouse < closestDistance)
                {
                    closestDistance = distToMouse;
                    closestObject = col.gameObject;
                }
            }
        }

        if (closestObject != null && showDebugInfo)
        {
            Debug.Log($"? Selected object: {closestObject.name}");
        }

        return closestObject;
    }

    private GameObject FindObjectByLayer(Vector3 mousePos)
    {
        if (levitatableLayer.value == 0)
        {
            Debug.LogError("LayerMask is not set! Set it in inspector or switch to Tag mode.");
            return null;
        }

        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(mousePos, 2f, levitatableLayer);

        if (showDebugInfo)
        {
            Debug.Log($"Found {nearbyObjects.Length} objects on levitatable layer");
        }

        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D col in nearbyObjects)
        {
            float distToMouse = Vector2.Distance(mousePos, col.transform.position);
            float distToPlayer = Vector2.Distance(playerTransform.position, col.transform.position);

            if (distToPlayer <= maxGrabDistance && distToMouse < closestDistance)
            {
                closestDistance = distToMouse;
                closestObject = col.gameObject;
            }
        }

        return closestObject;
    }

    private void GrabObject(GameObject obj)
    {
        heldObject = obj;
        heldObjectRb = obj.GetComponent<Rigidbody2D>();
        heldObjectCollider = obj.GetComponent<Collider2D>();

        if (heldObjectRb == null)
        {
            Debug.LogError($"Cannot grab {obj.name} - missing Rigidbody2D!");
            heldObject = null;
            return;
        }

        originalGravityScale = heldObjectRb.gravityScale;
        originalDrag = heldObjectRb.drag;

        heldObjectRb.gravityScale = 0;
        heldObjectRb.drag = 5f;
        heldObjectRb.velocity = Vector2.zero;
        heldObjectRb.angularVelocity = 0;
        heldObjectRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (heldObjectCollider != null && PlayerController.Instance != null)
        {
            Collider2D playerCol = PlayerController.Instance.GetComponent<Collider2D>();
            if (playerCol != null)
            {
                Physics2D.IgnoreCollision(heldObjectCollider, playerCol, true);
            }
        }

        isHoldingObject = true;
        telekinesisLine.enabled = true;

        if (audioSource != null && grabSound != null)
        {
            audioSource.PlayOneShot(grabSound);
        }

        if (audioSource != null && holdLoopSound != null)
        {
            audioSource.clip = holdLoopSound;
            audioSource.loop = true;
            audioSource.volume = 0.3f;
            audioSource.Play();
        }

        if (grabEffect != null)
        {
            currentGrabEffect = Instantiate(grabEffect, obj.transform.position, Quaternion.identity);
            currentGrabEffect.transform.SetParent(obj.transform);
        }

        Debug.Log($"??? Successfully grabbed: {obj.name} ???");
    }

    public void UpdateHeldObject()
    {
        if (!isHoldingObject || heldObject == null)
        {
            if (isHoldingObject)
            {
                ReleaseObject();
            }
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        targetPosition = mousePos;

        if (heldObjectRb != null)
        {
            float distance = Vector2.Distance(targetPosition, heldObject.transform.position);
            float speedMultiplier = Mathf.Clamp(distance * 0.5f, 0.5f, 2f);

            Vector2 newPosition = Vector2.MoveTowards(
                heldObjectRb.position,
                targetPosition,
                moveSpeed * speedMultiplier * Time.fixedDeltaTime
            );

            heldObjectRb.MovePosition(newPosition);
        }

        UpdateTelekinesisLine();
    }

    private void UpdateTelekinesisLine()
    {
        if (telekinesisLine != null && heldObject != null && playerTransform != null)
        {
            Vector3 startPos = playerTransform.position + Vector3.up * 0.5f;
            telekinesisLine.SetPosition(0, startPos);
            telekinesisLine.SetPosition(1, heldObject.transform.position);
        }
    }

    public void ThrowObject()
    {
        if (!isHoldingObject || heldObject == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 throwDirection = (mousePos - playerTransform.position).normalized;

        if (heldObjectRb != null)
        {
            heldObjectRb.gravityScale = originalGravityScale;
            heldObjectRb.drag = originalDrag;
            heldObjectRb.constraints = RigidbodyConstraints2D.None;
            heldObjectRb.velocity = throwDirection * throwForce;
        }

        if (heldObjectCollider != null && PlayerController.Instance != null)
        {
            Collider2D playerCol = PlayerController.Instance.GetComponent<Collider2D>();
            if (playerCol != null)
            {
                Physics2D.IgnoreCollision(heldObjectCollider, playerCol, false);
            }
        }

        Debug.Log($"Threw {heldObject.name} in direction: {throwDirection}");
        ReleaseObject();
    }

    public void ReleaseObject()
    {
        if (!isHoldingObject) return;

        if (heldObjectRb != null)
        {
            heldObjectRb.gravityScale = originalGravityScale;
            heldObjectRb.drag = originalDrag;
            heldObjectRb.constraints = RigidbodyConstraints2D.None;
        }

        if (heldObjectCollider != null && PlayerController.Instance != null)
        {
            Collider2D playerCol = PlayerController.Instance.GetComponent<Collider2D>();
            if (playerCol != null)
            {
                Physics2D.IgnoreCollision(heldObjectCollider, playerCol, false);
            }
        }

        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        if (audioSource != null && releaseSound != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }

        if (currentGrabEffect != null)
        {
            Destroy(currentGrabEffect);
            currentGrabEffect = null;
        }

        Debug.Log($"Released: {heldObject?.name}");

        heldObject = null;
        heldObjectRb = null;
        heldObjectCollider = null;
        isHoldingObject = false;
        telekinesisLine.enabled = false;
    }

    public bool IsHoldingObject()
    {
        return isHoldingObject && heldObject != null;
    }

    public GameObject GetHeldObject()
    {
        return heldObject;
    }

    private void OnDestroy()
    {
        if (isHoldingObject)
        {
            ReleaseObject();
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugInfo || !Application.isPlaying) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(mousePos, 2f);

        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, maxGrabDistance);
        }

        if (isHoldingObject && heldObject != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(heldObject.transform.position, 0.5f);
        }
    }
}