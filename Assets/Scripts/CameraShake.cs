using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPosition;
    private float shakeTimeRemaining;
    private float shakePower;
    private float shakeFadeTime;
    private float shakeRotation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shakePower;
            float yAmount = Random.Range(-1f, 1f) * shakePower;
            float zRotation = Random.Range(-1f, 1f) * shakeRotation;

            transform.localPosition += new Vector3(xAmount, yAmount, 0f);
            transform.localRotation = Quaternion.Euler(0, 0, zRotation);

            shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);
            shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * Time.deltaTime);
        }
        else
        {
            transform.localRotation = Quaternion.identity;
        }
    }

    public void StartShake(float length, float power, float rotation = 1f)
    {
        shakeTimeRemaining = length;
        shakePower = power;
        shakeFadeTime = power / length;
        shakeRotation = rotation;
    }

    public void StopShake()
    {
        shakeTimeRemaining = 0f;
        shakePower = 0f;
        transform.localRotation = Quaternion.identity;
    }
}
