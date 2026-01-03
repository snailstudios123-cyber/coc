using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanelSerialized;
    [SerializeField] private TextMeshProUGUI speakerNameTextSerialized;
    [SerializeField] private TextMeshProUGUI dialogueTextSerialized;
    [SerializeField] private Image speakerPortraitSerialized;
    [SerializeField] private GameObject continuePromptSerialized;
    
    private GameObject dialoguePanel;
    private TextMeshProUGUI speakerNameText;
    private TextMeshProUGUI dialogueText;
    private Image speakerPortrait;
    private GameObject continuePrompt;

    [Header("Settings")]
    [SerializeField] private float defaultTypingSpeed = 0.05f;
    [SerializeField] private KeyCode continueKey = KeyCode.E;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        FindUIReferences();
    }
    
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        FindUIReferences();
    }
    
    private void FindUIReferences()
    {
        if (dialoguePanelSerialized != null)
        {
            dialoguePanel = dialoguePanelSerialized;
            speakerNameText = speakerNameTextSerialized;
            dialogueText = dialogueTextSerialized;
            continuePrompt = continuePromptSerialized;
            speakerPortrait = speakerPortraitSerialized;
            Debug.Log("[DialogueManager] Using manually assigned references from Inspector");
            return;
        }
        
        dialoguePanel = null;
        speakerNameText = null;
        dialogueText = null;
        continuePrompt = null;
        speakerPortrait = null;
        
        GameObject sceneTransitionManager = GameObject.Find("SceneTransitionManager");
        GameObject panel = null;
        
        if (sceneTransitionManager != null)
        {
            Transform transitionCanvas = sceneTransitionManager.transform.Find("TransitionCanvas");
            if (transitionCanvas != null)
            {
                Transform dialoguePanelTransform = transitionCanvas.Find("DialoguePanel");
                if (dialoguePanelTransform != null)
                {
                    panel = dialoguePanelTransform.gameObject;
                    Debug.Log("[DialogueManager] Found DialoguePanel in SceneTransitionManager");
                }
            }
        }
        
        if (panel == null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                Transform dialoguePanelTransform = canvas.transform.Find("DialoguePanel");
                if (dialoguePanelTransform != null)
                {
                    panel = dialoguePanelTransform.gameObject;
                    Debug.Log("[DialogueManager] Found DialoguePanel in Canvas");
                }
            }
        }
        
        if (panel != null)
        {
            dialoguePanel = panel;
            Debug.Log($"[DialogueManager] ✓ Using DialoguePanel: {GetGameObjectPath(panel)}");
            
            Transform panelTransform = panel.transform;
            
            Transform speakerName = panelTransform.Find("SpeakerName");
            if (speakerName != null)
            {
                speakerNameText = speakerName.GetComponent<TextMeshProUGUI>();
                Debug.Log("[DialogueManager] ✓ Found SpeakerNameText");
            }
            else
            {
                Debug.LogError("[DialogueManager] ✗ SpeakerName not found!");
            }
            
            Transform dialogue = panelTransform.Find("DialogueText");
            if (dialogue != null)
            {
                dialogueText = dialogue.GetComponent<TextMeshProUGUI>();
                Debug.Log("[DialogueManager] ✓ Found DialogueText");
            }
            else
            {
                Debug.LogError("[DialogueManager] ✗ DialogueText not found!");
            }
            
            Transform prompt = panelTransform.Find("ContinuePrompt");
            if (prompt != null)
            {
                continuePrompt = prompt.gameObject;
                Debug.Log("[DialogueManager] ✓ Found ContinuePrompt");
            }
            else
            {
                Debug.LogError("[DialogueManager] ✗ ContinuePrompt not found!");
            }
            
            Transform portrait = panelTransform.Find("SpeakerPortrait");
            if (portrait != null)
            {
                speakerPortrait = portrait.GetComponent<Image>();
                Debug.Log("[DialogueManager] ✓ Found SpeakerPortrait");
            }
            else
            {
                Debug.LogWarning("[DialogueManager] ⚠ SpeakerPortrait not found - dialogue will work but portraits won't display");
            }
        }
        else
        {
            Debug.LogError("[DialogueManager] ✗ Could not find DialoguePanel in scene! Checked SceneTransitionManager and Canvas.");
        }
    }
    
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }

    private void Start()
    {
        FindUIReferences();
        
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    private void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(continueKey))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                ShowNextLine();
            }
        }

        if (Input.GetKeyDown(skipKey) && isTyping)
        {
            SkipTyping();
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null || dialogue.lines.Length == 0)
        {
            Debug.LogWarning("[DialogueManager] Cannot start dialogue - data is null or empty");
            return;
        }

        if (dialoguePanel == null)
        {
            Debug.LogError("[DialogueManager] Dialogue panel is null! Cannot start dialogue. Please assign the DialoguePanel in the Inspector.");
            return;
        }

        Debug.Log($"[DialogueManager] Starting dialogue with {dialogue.lines.Length} lines");

        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialogueActive = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }

        Time.timeScale = 0f;
        Debug.Log("[DialogueManager] Time.timeScale set to 0");

        dialoguePanel.SetActive(true);

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];

        if (speakerNameText != null)
        {
            speakerNameText.text = line.speakerName;
        }

        if (speakerPortrait != null)
        {
            if (line.speakerPortrait != null)
            {
                speakerPortrait.sprite = line.speakerPortrait;
                speakerPortrait.enabled = true;
            }
            else
            {
                speakerPortrait.enabled = false;
            }
        }

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }

        float speed = line.typingSpeed > 0 ? line.typingSpeed : defaultTypingSpeed;
        typingCoroutine = StartCoroutine(TypeTextDelayed(line.text, speed));
    }

    private IEnumerator TypeTextDelayed(string text, float speed)
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
        
        yield return null;
        
        yield return StartCoroutine(TypeText(text, speed));
    }

    private IEnumerator TypeText(string text, float speed)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(speed);
        }

        isTyping = false;

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(true);
        }
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];
        dialogueText.text = line.text;
        isTyping = false;

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(true);
        }
    }

    private void ShowNextLine()
    {
        currentLineIndex++;
        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        Debug.Log("[DialogueManager] Ending dialogue");
        dialogueActive = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        Debug.Log("[DialogueManager] Time.timeScale restored to 1");

        currentDialogue = null;
        currentLineIndex = 0;
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}
