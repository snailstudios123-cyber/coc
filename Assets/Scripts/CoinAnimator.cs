using UnityEngine;

public class CoinAnimator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobAmount = 0.1f;
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private bool enableBobbing = true;
    
    private Vector3 startPosition;
    private float bobTimer;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (enableRotation)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        if (enableBobbing)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float newY = startPosition.y + Mathf.Sin(bobTimer) * bobAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            startPosition = new Vector3(transform.position.x, startPosition.y, startPosition.z);
        }
    }
}
