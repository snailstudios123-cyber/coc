using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float spreadAmount = 0.5f;

    private TextMeshPro textMesh;
    private Color textColor;
    private Vector3 moveDirection;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            textMesh = gameObject.AddComponent<TextMeshPro>();
        }
        textColor = textMesh.color;
    }

    private void Start()
    {
        moveDirection = new Vector3(Random.Range(-spreadAmount, spreadAmount), 1f, 0f).normalized;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        textColor.a = Mathf.Lerp(1f, 0f, timer / lifetime);
        textMesh.color = textColor;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(float damage, bool isCritical = false)
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        textMesh.text = Mathf.RoundToInt(damage).ToString();
        textMesh.fontSize = isCritical ? 6f : 4f;
        textMesh.color = isCritical ? new Color(1f, 0.5f, 0f) : Color.white;
        textColor = textMesh.color;

        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.sortingOrder = 100;

        if (isCritical)
        {
            textMesh.fontStyle = FontStyles.Bold;
            textMesh.text = "CRIT! " + textMesh.text;
        }
    }
}
