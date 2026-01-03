using System.Collections;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("Spell Data")]
    [SerializeField] private SpellData spellData;
    
    [Header("Grappling Hook Settings")]
    [SerializeField] private float hookSpeed = 20f;
    [SerializeField] private float maxHookDistance = 15f;
    [SerializeField] private LayerMask grappableLayer = -1;
    [SerializeField] private float swingForce = 500f;
    [SerializeField] private float hookDamage = 10f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float ropeShortSpeed = 5f;
    [SerializeField] private float minRopeLength = 1f;

    [Header("Visual Settings")]
    [SerializeField] private LineRenderer chainRenderer;
    [SerializeField] private Transform hookPoint;
    [SerializeField] private SpriteRenderer hookSprite;

    [Header("Rope Image Settings")]
    [SerializeField] private bool useImageRope = false;
    [SerializeField] private Sprite ropeSprite;
    [SerializeField] private float ropeWidth = 0.2f;
    [SerializeField] private int ropeSegments = 10;
    [SerializeField] private Material ropeMaterial;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hookLaunchSound;
    [SerializeField] private AudioClip hookHitSound;
    [SerializeField] private AudioClip hookRetractSound;

    private Vector2 hookDirection;
    private Vector2 targetPosition;
    private bool isHookActive = false;
    private bool isHookAttached = false;
    private bool isRetracting = false;
    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private Transform player;
    private Vector2 hookAttachPoint;
    private DistanceJoint2D joint;
    private Collider2D hookCollider;
    private Coroutine currentMoveCoroutine;
    private Coroutine currentRetractCoroutine;
    private bool isPulling = false;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        player = playerController.transform;
        playerRb = playerController.GetComponent<Rigidbody2D>();
        hookCollider = GetComponent<Collider2D>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (chainRenderer == null)
        {
            chainRenderer = GetComponent<LineRenderer>();
            if (chainRenderer == null)
            {
                chainRenderer = gameObject.AddComponent<LineRenderer>();
            }
        }

        SetupChainRenderer();

        if (hookCollider != null)
            hookCollider.enabled = false;
        if (hookSprite != null)
            hookSprite.enabled = false;
        if (chainRenderer != null)
            chainRenderer.enabled = false;
    }

    private void SetupChainRenderer()
    {
        if (useImageRope && ropeSprite != null)
        {
            // Setup for image-based rope
            if (ropeMaterial == null)
            {
                ropeMaterial = new Material(Shader.Find("Sprites/Default"));
            }

            chainRenderer.material = ropeMaterial;
            chainRenderer.textureMode = LineTextureMode.Tile;

            // Assign the sprite's texture to the material
            if (ropeSprite.texture != null)
            {
                chainRenderer.material.mainTexture = ropeSprite.texture;
            }

            chainRenderer.startWidth = ropeWidth;
            chainRenderer.endWidth = ropeWidth;
            chainRenderer.startColor = Color.white;
            chainRenderer.endColor = Color.white;
        }
        else
        {
            // Default setup for simple line
            chainRenderer.material = new Material(Shader.Find("Sprites/Default"));
            chainRenderer.startColor = Color.gray;
            chainRenderer.endColor = Color.gray;
            chainRenderer.startWidth = 0.1f;
            chainRenderer.endWidth = 0.05f;
        }

        chainRenderer.positionCount = 2;
        chainRenderer.useWorldSpace = true;
        chainRenderer.sortingLayerName = "Default";
        chainRenderer.sortingOrder = 1;
    }

    public void LaunchHook(Vector2 direction, Vector2 startPosition)
    {
        if (spellData != null && !spellData.isEquipped)
        {
            return;
        }
        
        if (isHookActive)
        {
            CancelCurrentGrapple();
        }

        hookDirection = direction.normalized;
        transform.position = startPosition;
        targetPosition = startPosition + hookDirection * maxHookDistance;

        isHookActive = true;
        isHookAttached = false;
        isRetracting = false;

        if (hookSprite != null)
            hookSprite.enabled = true;
        if (chainRenderer != null)
            chainRenderer.enabled = true;

        PlaySound(hookLaunchSound);

        if (hookCollider != null)
            hookCollider.enabled = true;

        currentMoveCoroutine = StartCoroutine(MoveHook());
    }

    public void LaunchHookToTarget(Vector2 direction, Vector2 startPosition, Vector2 targetPos)
    {
        if (isHookActive)
        {
            CancelCurrentGrapple();
        }

        hookDirection = direction.normalized;
        transform.position = startPosition;

        float distanceToTarget = Vector2.Distance(startPosition, targetPos);
        if (distanceToTarget > maxHookDistance)
        {
            targetPosition = startPosition + hookDirection * maxHookDistance;
        }
        else
        {
            targetPosition = targetPos;
        }

        Debug.Log($"Hook launched from {startPosition} to {targetPosition}, distance: {distanceToTarget}");

        isHookActive = true;
        isHookAttached = false;
        isRetracting = false;

        if (hookSprite != null)
            hookSprite.enabled = true;
        if (chainRenderer != null)
            chainRenderer.enabled = true;

        PlaySound(hookLaunchSound);

        if (hookCollider != null)
            hookCollider.enabled = true;

        currentMoveCoroutine = StartCoroutine(MoveHook());
    }

    private void CancelCurrentGrapple()
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }

        if (currentRetractCoroutine != null)
        {
            StopCoroutine(currentRetractCoroutine);
            currentRetractCoroutine = null;
        }

        isHookAttached = false;
        isRetracting = false;
    }

    private IEnumerator MoveHook()
    {
        Vector2 startPos = transform.position;
        float journeyLength = Vector2.Distance(startPos, targetPosition);
        float journeyTime = journeyLength / hookSpeed;
        float elapsedTime = 0;

        while (elapsedTime < journeyTime && !isHookAttached && !isRetracting)
        {
            elapsedTime += Time.deltaTime;
            float fractionOfJourney = elapsedTime / journeyTime;

            transform.position = Vector2.Lerp(startPos, targetPosition, fractionOfJourney);

            UpdateChainVisual();

            yield return null;
        }

        if (!isHookAttached && !isRetracting)
        {
            CheckForNearbyGrappableSurface();
        }

        if (!isHookAttached)
        {
            yield return new WaitForSeconds(0.1f);
            RetractHook();
        }

        currentMoveCoroutine = null;
    }

    private void CheckForNearbyGrappableSurface()
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, grappableLayer);

        if (nearbyColliders.Length > 0)
        {
            AttachHook(nearbyColliders[0]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isHookActive || isHookAttached || isRetracting) return;

        Debug.Log($"Hook collided with: {other.name} on layer {other.gameObject.layer} ({LayerMask.LayerToName(other.gameObject.layer)})");
        Debug.Log($"Grappable layer mask: {grappableLayer.value}, checking: {((1 << other.gameObject.layer) & grappableLayer) != 0}");

        if (((1 << other.gameObject.layer) & grappableLayer) != 0)
        {
            Debug.Log("Attaching to grappable surface!");
            AttachHook(other);
        }
        else if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Debug.Log("Hit enemy!");
            HitEnemy(other);
        }
        else
        {
            Debug.Log("Hit non-grappable surface");
        }
    }

    private void AttachHook(Collider2D surface)
    {
        isHookAttached = true;
        hookAttachPoint = transform.position;

        PlaySound(hookHitSound);

        if (joint != null)
        {
            Destroy(joint);
        }

        joint = player.gameObject.AddComponent<DistanceJoint2D>();
        joint.connectedAnchor = hookAttachPoint;
        joint.distance = Vector2.Distance(player.position, hookAttachPoint);
        joint.maxDistanceOnly = true;
        joint.enableCollision = false;

        if (hookCollider != null)
            hookCollider.enabled = false;

        Vector2 swingDirection = (hookAttachPoint - (Vector2)player.position).normalized;
        Vector2 perpendicular = new Vector2(-swingDirection.y, swingDirection.x);

        if (Vector2.Dot(playerRb.velocity, perpendicular) < 0)
            perpendicular = -perpendicular;

        playerRb.AddForce(perpendicular * swingForce);
    }

    private void HitEnemy(Collider2D enemy)
    {
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            Vector2 knockbackDirection = (enemy.transform.position - player.position).normalized;
            enemyScript.EnemyHit(hookDamage, knockbackDirection, 300f);
        }

        RetractHook();
    }

    public void RetractHook()
    {
        if (!isHookActive) return;

        isRetracting = true;

        PlaySound(hookRetractSound);

        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }

        currentRetractCoroutine = StartCoroutine(RetractCoroutine());
    }

    private IEnumerator RetractCoroutine()
    {
        Vector2 startPos = transform.position;
        Vector2 playerPos = player.position;
        float journeyLength = Vector2.Distance(startPos, playerPos);
        float journeyTime = journeyLength / (hookSpeed * 1.5f);
        float elapsedTime = 0;

        while (elapsedTime < journeyTime)
        {
            elapsedTime += Time.deltaTime;
            float fractionOfJourney = elapsedTime / journeyTime;

            playerPos = player.position;
            transform.position = Vector2.Lerp(startPos, playerPos, fractionOfJourney);

            UpdateChainVisual();

            yield return null;
        }

        DeactivateHook();

        currentRetractCoroutine = null;
    }

    private void DeactivateHook()
    {
        isHookActive = false;
        isHookAttached = false;
        isRetracting = false;

        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }

        if (hookCollider != null)
            hookCollider.enabled = false;

        if (hookSprite != null)
            hookSprite.enabled = false;
        if (chainRenderer != null)
            chainRenderer.enabled = false;
        if (hookCollider != null)
            hookCollider.enabled = false;
    }

    private void UpdateChainVisual()
    {
        if (chainRenderer == null || player == null) return;

        if (isHookActive)
        {
            if (isHookAttached)
            {
                int segments = useImageRope ? ropeSegments : 10;
                chainRenderer.positionCount = segments + 1;

                Vector3 startPos = player.position;
                Vector3 endPos = hookAttachPoint;
                float ropeLength = Vector3.Distance(startPos, endPos);
                float sagAmount = ropeLength * 0.2f;

                for (int i = 0; i <= segments; i++)
                {
                    float t = (float)i / segments;
                    Vector3 point = Vector3.Lerp(startPos, endPos, t);

                    float sagMultiplier = 4 * t * (1 - t);
                    point.y -= sagAmount * sagMultiplier;

                    chainRenderer.SetPosition(i, point);
                }
            }
            else
            {
                chainRenderer.positionCount = 2;
                chainRenderer.SetPosition(0, player.position);
                chainRenderer.SetPosition(1, transform.position);
            }
        }
        else
        {
            chainRenderer.positionCount = 0;
        }
    }

    private void Update()
    {
        if (isHookActive)
        {
            UpdateChainVisual();

            if (isHookAttached && Input.GetKey(KeyCode.H))
            {
                ShortenRope();
            }

            if (isHookAttached && Input.GetButtonUp("CastSpell"))
            {
                RetractHook();
            }
        }
    }

    private void ShortenRope()
    {
        if (!isHookAttached || joint == null) return;

        float newDistance = joint.distance - (ropeShortSpeed * Time.deltaTime);

        joint.distance = Mathf.Max(newDistance, minRopeLength);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public bool IsHookActive()
    {
        return isHookActive;
    }

    public bool IsHookAttached()
    {
        return isHookAttached;
    }

    public bool CanLaunchHook()
    {
        return true;
    }
}