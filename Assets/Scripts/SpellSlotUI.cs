using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SpellSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image spellIcon;
    [SerializeField] private TextMeshProUGUI spellNameText;
    [SerializeField] private Button selectButton;

    private SpellData spellData;
    private PauseMenuManager pauseMenuManager;
    private Image backgroundImage;
    private Color normalColor;
    private Color hoverColor;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
        {
            normalColor = backgroundImage.color;
            hoverColor = new Color(
                normalColor.r * 1.4f,
                normalColor.g * 1.4f,
                normalColor.b * 1.4f,
                normalColor.a
            );
        }
    }

    public void Initialize(SpellData data, PauseMenuManager menuManager)
    {
        spellData = data;
        pauseMenuManager = menuManager;

        if (spellIcon != null && data.icon != null)
        {
            spellIcon.sprite = data.icon;
            spellIcon.enabled = true;
            spellIcon.color = Color.white;
            spellIcon.preserveAspect = true;
            spellIcon.gameObject.SetActive(true);
        }
        else if (spellIcon != null)
        {
            spellIcon.enabled = false;
            spellIcon.gameObject.SetActive(false);
        }

        if (spellNameText != null)
        {
            spellNameText.text = data.spellName;
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSpellClicked);
            
            ColorBlock colors = selectButton.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.8f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.colorMultiplier = 1f;
            selectButton.colors = colors;
        }
    }

    private void OnSpellClicked()
    {
        if (pauseMenuManager != null)
        {
            pauseMenuManager.DisplaySpellDetails(spellData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = hoverColor;
        }
        
        if (spellNameText != null)
        {
            spellNameText.color = new Color(1f, 1f, 0.7f, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
        
        if (spellNameText != null)
        {
            spellNameText.color = new Color(0.9f, 0.95f, 1f, 1f);
        }
    }
}
