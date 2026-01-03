using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject spellsMenuPanel;
    [SerializeField] private GameObject spellDetailPanel;
    [SerializeField] private GameObject inventoryMenuPanel;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button spellsButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Spells Menu")]
    [SerializeField] private Transform spellsListContainer;
    [SerializeField] private GameObject spellSlotPrefab;
    [SerializeField] private Button backButton;

    [Header("Spell Detail Panel")]
    [SerializeField] private TextMeshProUGUI spellNameText;
    [SerializeField] private TextMeshProUGUI spellDescriptionText;
    [SerializeField] private TextMeshProUGUI manaCostText;
    [SerializeField] private Image spellIconImage;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private TextMeshProUGUI equipStatusText;

    private bool isPaused = false;
    private SpellData selectedSpell;
    private List<GameObject> spawnedSpellSlots = new List<GameObject>();
    private InventoryUI inventoryUI;

    public static PauseMenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetupButtons();
        HideAllPanels();
        
        inventoryUI = GetComponent<InventoryUI>();
        if (inventoryUI == null)
        {
            inventoryUI = FindObjectOfType<InventoryUI>(true);
        }
        
        if (inventoryUI != null)
        {
            Debug.Log("[PauseMenuManager] InventoryUI found successfully!");
        }
        else
        {
            Debug.LogError("[PauseMenuManager] InventoryUI not found in scene! Make sure InventoryPanel has the InventoryUI component.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
            {
                Debug.Log("[PauseMenuManager] Blocked pause - dialogue is active");
                return;
            }
            
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void SetupButtons()
    {
        Debug.Log("[PauseMenuManager] Setting up button listeners...");

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
            Debug.Log($"[PauseMenuManager] ✓ Resume button listener added. Interactable: {resumeButton.interactable}");
        }
        else
        {
            Debug.LogError("[PauseMenuManager] ❌ resumeButton is NULL!");
        }

        if (spellsButton != null)
        {
            spellsButton.onClick.RemoveAllListeners();
            spellsButton.onClick.AddListener(OpenSpellsMenu);
            Debug.Log($"[PauseMenuManager] ✓ Spells button listener added. Interactable: {spellsButton.interactable}");
        }
        else
        {
            Debug.LogError("[PauseMenuManager] ❌ spellsButton is NULL!");
        }

        if (inventoryButton != null)
        {
            inventoryButton.onClick.RemoveAllListeners();
            inventoryButton.onClick.AddListener(OpenInventoryMenu);
            Debug.Log($"[PauseMenuManager] ✓ Inventory button listener added. Interactable: {inventoryButton.interactable}");
        }
        else
        {
            Debug.LogError("[PauseMenuManager] ❌ inventoryButton is NULL!");
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(OpenOptions);
            Debug.Log($"[PauseMenuManager] ✓ Options button listener added. Interactable: {optionsButton.interactable}");
        }
        else
        {
            Debug.LogError("[PauseMenuManager] ❌ optionsButton is NULL!");
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
            Debug.Log($"[PauseMenuManager] ✓ Quit button listener added. Interactable: {quitButton.interactable}");
        }
        else
        {
            Debug.LogError("[PauseMenuManager] ❌ quitButton is NULL!");
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(BackToPauseMenu);
            Debug.Log("[PauseMenuManager] ✓ Back button listener added");
        }

        if (equipButton != null)
        {
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(EquipSelectedSpell);
        }

        if (unequipButton != null)
        {
            unequipButton.onClick.RemoveAllListeners();
            unequipButton.onClick.AddListener(UnequipSelectedSpell);
        }

        Debug.Log("[PauseMenuManager] Button setup complete!");
    }

    private void PauseGame()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            Debug.Log("[PauseMenuManager] Cannot pause - dialogue is active");
            return;
        }
        
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (spellsMenuPanel != null)
            spellsMenuPanel.SetActive(false);

        if (spellDetailPanel != null)
            spellDetailPanel.SetActive(false);
    }

    private void ResumeGame()
    {
        Debug.Log("[PauseMenuManager] ResumeGame() called!");
        isPaused = false;
        Time.timeScale = 1f;
        HideAllPanels();
    }

    private void HideAllPanels()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (spellsMenuPanel != null)
            spellsMenuPanel.SetActive(false);

        if (spellDetailPanel != null)
            spellDetailPanel.SetActive(false);

        if (inventoryUI != null)
            inventoryUI.CloseInventory();
    }

    private void OpenInventoryMenu()
    {
        Debug.Log("[PauseMenuManager] OpenInventoryMenu called");
        Debug.Log($"[PauseMenuManager] inventoryUI reference is: {(inventoryUI != null ? "VALID" : "NULL")}");
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (inventoryUI != null)
        {
            Debug.Log("[PauseMenuManager] Calling inventoryUI.OpenInventory()");
            inventoryUI.OpenInventory();
        }
        else
        {
            Debug.LogError("[PauseMenuManager] inventoryUI is NULL! Trying to find it...");
            inventoryUI = FindObjectOfType<InventoryUI>(true);
            if (inventoryUI != null)
            {
                Debug.Log("[PauseMenuManager] Found InventoryUI! Opening now...");
                inventoryUI.OpenInventory();
            }
            else
            {
                Debug.LogError("[PauseMenuManager] Could not find InventoryUI in scene!");
            }
        }
    }

    private void OpenSpellsMenu()
    {
        Debug.Log("[PauseMenuManager] OpenSpellsMenu() called!");
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (spellsMenuPanel != null)
            spellsMenuPanel.SetActive(true);

        if (spellDetailPanel != null)
            spellDetailPanel.SetActive(false);

        FixSpellListVisibility();
        PopulateSpellsList();
    }

    private void FixSpellListVisibility()
    {
        if (spellsListContainer == null)
            return;

        VerticalLayoutGroup vlg = spellsListContainer.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.childControlHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;
        }

        ContentSizeFitter csf = spellsListContainer.GetComponent<ContentSizeFitter>();
        if (csf != null)
        {
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        Transform viewport = spellsListContainer.parent;
        if (viewport != null)
        {
            Mask mask = viewport.GetComponent<Mask>();
            if (mask != null)
            {
                mask.enabled = false;
            }
        }
    }

    private void PopulateSpellsList()
    {
        ClearSpellsList();

        if (SpellManager.Instance == null)
        {
            Debug.LogError("[PauseMenuManager] SpellManager.Instance is null! Cannot populate spells list.");
            return;
        }

        if (spellSlotPrefab == null)
        {
            Debug.LogError("[PauseMenuManager] spellSlotPrefab is null! Assign the SpellSlot prefab.");
            return;
        }

        if (spellsListContainer == null)
        {
            Debug.LogError("[PauseMenuManager] spellsListContainer is null! Cannot populate spells list.");
            return;
        }

        List<SpellData> allSpells = SpellManager.Instance.GetAllSpells();
        
        if (allSpells == null || allSpells.Count == 0)
        {
            return;
        }

        foreach (SpellData spell in allSpells)
        {
            if (spell == null)
            {
                Debug.LogWarning("[PauseMenuManager] Skipping null spell in list");
                continue;
            }

            GameObject spellSlot = Instantiate(spellSlotPrefab, spellsListContainer);
            SpellSlotUI slotUI = spellSlot.GetComponent<SpellSlotUI>();

            if (slotUI != null)
            {
                slotUI.Initialize(spell, this);
            }

            spawnedSpellSlots.Add(spellSlot);
        }
    }

    private void ClearSpellsList()
    {
        foreach (GameObject slot in spawnedSpellSlots)
        {
            Destroy(slot);
        }
        spawnedSpellSlots.Clear();
    }

    public void DisplaySpellDetails(SpellData spell)
    {
        selectedSpell = spell;

        if (spellDetailPanel != null)
            spellDetailPanel.SetActive(true);

        if (spellNameText != null)
            spellNameText.text = spell.spellName;

        if (spellDescriptionText != null)
            spellDescriptionText.text = spell.description;

        if (manaCostText != null)
            manaCostText.text = $"Mana Cost: {spell.manaCost:F2}";

        if (spellIconImage != null && spell.icon != null)
        {
            spellIconImage.sprite = spell.icon;
            spellIconImage.enabled = true;
            spellIconImage.color = Color.white;
        }
        else if (spellIconImage != null)
        {
            spellIconImage.enabled = false;
        }

        UpdateEquipButtons();
    }

    private void UpdateEquipButtons()
    {
        if (SpellManager.Instance == null || selectedSpell == null)
            return;

        bool isEquipped = SpellManager.Instance.IsSpellEquipped(selectedSpell);

        if (equipButton != null)
            equipButton.gameObject.SetActive(!isEquipped);

        if (unequipButton != null)
            unequipButton.gameObject.SetActive(isEquipped);

        if (equipStatusText != null)
        {
            equipStatusText.text = isEquipped ? "Equipped" : "Not Equipped";
        }
    }

    private void EquipSelectedSpell()
    {
        if (SpellManager.Instance == null || selectedSpell == null)
            return;

        bool success = SpellManager.Instance.EquipSpell(selectedSpell);

        if (success)
        {
            UpdateEquipButtons();
        }
        else
        {
            Debug.Log("Cannot equip more spells or spell is already equipped!");
        }
    }

    private void UnequipSelectedSpell()
    {
        if (SpellManager.Instance == null || selectedSpell == null)
            return;

        SpellManager.Instance.UnequipSpell(selectedSpell);
        UpdateEquipButtons();
    }

    private void BackToPauseMenu()
    {
        if (spellsMenuPanel != null)
            spellsMenuPanel.SetActive(false);

        if (spellDetailPanel != null)
            spellDetailPanel.SetActive(false);

        if (inventoryUI != null)
            inventoryUI.CloseInventory();

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        ClearSpellsList();
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    private void OpenOptions()
    {
        Debug.Log("Options menu not yet implemented");
    }

    private void QuitGame()
    {
        Debug.Log("[PauseMenuManager] QuitGame() called!");
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
