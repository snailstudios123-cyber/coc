using UnityEngine;

public class ConfusionEffect : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 0.3f;
    [SerializeField] private bool enableRotation = false;
    [SerializeField] private float rotationSpeed = 180f;
    
    private Vector3 startPosition;
    private float timeElapsed;
    
    private void Start()
    {
        startPosition = transform.localPosition;
    }
    
    private void Update()
    {
        timeElapsed += Time.deltaTime;
        
        float yOffset = Mathf.Sin(timeElapsed * floatSpeed) * floatHeight;
        transform.localPosition = startPosition + new Vector3(0, yOffset, 0);
        
        if (enableRotation)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}
