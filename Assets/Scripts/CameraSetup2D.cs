using UnityEngine;

public class CameraSetup2D : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
    [SerializeField] private float cameraZPosition = -10f;
    
    [Header("Follow Target")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private bool followPlayer = true;
    [SerializeField] private float followSpeed = 5f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        SetupCamera();
        
        if (followPlayer && followTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                followTarget = player.transform;
            }
        }
        
        if (followTarget != null)
        {
            PositionCameraAtTarget();
        }
    }

    private void SetupCamera()
    {
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = backgroundColor;
        cam.orthographic = true;
        
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, currentPos.y, cameraZPosition);
    }

    private void PositionCameraAtTarget()
    {
        if (followTarget != null)
        {
            Vector3 targetPos = followTarget.position;
            transform.position = new Vector3(targetPos.x, targetPos.y, cameraZPosition);
        }
    }

    private void LateUpdate()
    {
        if (followPlayer && followTarget != null)
        {
            Vector3 targetPos = followTarget.position;
            Vector3 smoothPos = Vector3.Lerp(transform.position, 
                new Vector3(targetPos.x, targetPos.y, cameraZPosition), 
                followSpeed * Time.deltaTime);
            transform.position = smoothPos;
        }
    }
}
