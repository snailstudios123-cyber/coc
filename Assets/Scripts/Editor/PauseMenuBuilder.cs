using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenuBuilder : EditorWindow
{
    private const string MENU_PATH = "Tools/🔧 Rebuild Pause Menu System";
    
    [MenuItem(MENU_PATH)]
    public static void RebuildPauseMenu()
    {
        if (!EditorUtility.DisplayDialog("Rebuild Pause Menu", 
            "This will rebuild the entire pause menu system. Make sure you have a backup of your scene.\n\nContinue?", 
            "Yes", "Cancel"))
        {
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene! Please create a Canvas first.");
            return;
        }

        GameObject oldPausePanel = GameObject.Find("PauseMenuPanel");
        GameObject oldSpellsPanel = GameObject.Find("SpellsMenuPanel");
        GameObject oldPauseManager = GameObject.Find("PauseMenuManager");

        if (oldPausePanel != null)
        {
            DestroyImmediate(oldPausePanel);
            Debug.Log("Removed old PauseMenuPanel");
        }
        if (oldSpellsPanel != null)
        {
            DestroyImmediate(oldSpellsPanel);
            Debug.Log("Removed old SpellsMenuPanel");
        }
        if (oldPauseManager != null)
        {
            DestroyImmediate(oldPauseManager);
            Debug.Log("Removed old PauseMenuManager");
        }

        EnsureEventSystem();

        GameObject pauseMenuPanel = CreatePauseMenuPanel(canvas.transform);
        GameObject spellsMenuPanel = CreateSpellsMenuPanel(canvas.transform);
        GameObject pauseMenuManager = CreatePauseMenuManager();

        SetupPauseMenuManager(pauseMenuManager, pauseMenuPanel, spellsMenuPanel);

        EditorUtility.SetDirty(canvas.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("<color=green>✓ Pause Menu System rebuilt successfully!</color>");
        Debug.Log("Press ESC in Play mode to open the pause menu.");
        
        Selection.activeGameObject = pauseMenuManager;
        EditorGUIUtility.PingObject(pauseMenuManager);
    }

    private static void EnsureEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("Created EventSystem");
        }
        else
        {
            StandaloneInputModule inputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (inputModule == null)
            {
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                Debug.Log("Added StandaloneInputModule to EventSystem");
            }
        }
    }

    private static GameObject CreatePauseMenuPanel(Transform canvasTransform)
    {
        GameObject panel = new GameObject("PauseMenuPanel");
        panel.transform.SetParent(canvasTransform, false);
        panel.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.04f, 0.05f, 0.08f, 0.96f);
        img.raycastTarget = true;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(panel.transform, false);
        content.layer = LayerMask.NameToLayer("UI");

        RectTransform contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0.5f, 0.5f);
        contentRt.anchorMax = new Vector2(0.5f, 0.5f);
        contentRt.sizeDelta = new Vector2(400, 500);
        contentRt.anchoredPosition = Vector2.zero;

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 15;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(20, 20, 20, 20);

        CreateTitle(content.transform, "PAUSE MENU");
        CreateButton(content.transform, "ResumeButton", "Resume");
        CreateButton(content.transform, "SpellsButton", "Spells");
        CreateButton(content.transform, "InventoryButton", "Inventory");
        CreateButton(content.transform, "OptionsButton", "Options");
        CreateButton(content.transform, "QuitButton", "Quit");

        panel.SetActive(false);
        Debug.Log("Created PauseMenuPanel");
        return panel;
    }

    private static GameObject CreateSpellsMenuPanel(Transform canvasTransform)
    {
        GameObject panel = new GameObject("SpellsMenuPanel");
        panel.transform.SetParent(canvasTransform, false);
        panel.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.04f, 0.05f, 0.08f, 0.96f);
        img.raycastTarget = true;

        GameObject spellsListPanel = CreateSpellsListPanel(panel.transform);
        GameObject spellDetailPanel = CreateSpellDetailPanel(panel.transform);
        GameObject backButton = CreateBackButton(panel.transform);

        panel.SetActive(false);
        Debug.Log("Created SpellsMenuPanel");
        return panel;
    }

    private static GameObject CreateSpellsListPanel(Transform parent)
    {
        GameObject listPanel = new GameObject("SpellsListPanel");
        listPanel.transform.SetParent(parent, false);
        listPanel.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = listPanel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.offsetMin = new Vector2(20, 80);
        rt.offsetMax = new Vector2(-10, -20);

        CreateTitle(listPanel.transform, "SPELLS");

        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(listPanel.transform, false);
        scrollView.layer = LayerMask.NameToLayer("UI");

        RectTransform scrollRt = scrollView.AddComponent<RectTransform>();
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = Vector2.one;
        scrollRt.offsetMin = new Vector2(0, 0);
        scrollRt.offsetMax = new Vector2(0, -60);

        Image scrollImg = scrollView.AddComponent<Image>();
        scrollImg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);

        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 20;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        viewport.layer = LayerMask.NameToLayer("UI");

        RectTransform viewportRt = viewport.AddComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.sizeDelta = Vector2.zero;
        viewportRt.anchoredPosition = Vector2.zero;

        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        Image viewportImg = viewport.AddComponent<Image>();
        viewportImg.color = Color.white;

        GameObject spellsListContainer = new GameObject("SpellsListContainer");
        spellsListContainer.transform.SetParent(viewport.transform, false);
        spellsListContainer.layer = LayerMask.NameToLayer("UI");

        RectTransform containerRt = spellsListContainer.AddComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(0, 1);
        containerRt.anchorMax = new Vector2(1, 1);
        containerRt.pivot = new Vector2(0.5f, 1);
        containerRt.anchoredPosition = Vector2.zero;
        containerRt.sizeDelta = new Vector2(0, 0);

        VerticalLayoutGroup vlg = spellsListContainer.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(5, 5, 5, 5);

        ContentSizeFitter csf = spellsListContainer.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        scrollRect.viewport = viewportRt;
        scrollRect.content = containerRt;

        return listPanel;
    }

    private static GameObject CreateSpellDetailPanel(Transform parent)
    {
        GameObject detailPanel = new GameObject("SpellDetailPanel");
        detailPanel.transform.SetParent(parent, false);
        detailPanel.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = detailPanel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(10, 80);
        rt.offsetMax = new Vector2(-20, -20);

        Image img = detailPanel.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        GameObject content = new GameObject("Content");
        content.transform.SetParent(detailPanel.transform, false);
        content.layer = LayerMask.NameToLayer("UI");

        RectTransform contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = Vector2.zero;
        contentRt.anchorMax = Vector2.one;
        contentRt.offsetMin = new Vector2(20, 20);
        contentRt.offsetMax = new Vector2(-20, -20);

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 15;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        CreateSpellIcon(content.transform);
        CreateDetailText(content.transform, "SpellNameText", 24, FontStyles.Bold);
        CreateDetailText(content.transform, "SpellDescriptionText", 16, FontStyles.Normal);
        CreateDetailText(content.transform, "ManaCostText", 18, FontStyles.Normal);
        CreateDetailText(content.transform, "EquipStatusText", 16, FontStyles.Italic);
        CreateDetailButton(content.transform, "EquipButton", "Equip Spell");
        CreateDetailButton(content.transform, "UnequipButton", "Unequip Spell");

        detailPanel.SetActive(false);
        return detailPanel;
    }

    private static void CreateSpellIcon(Transform parent)
    {
        GameObject icon = new GameObject("SpellIconImage");
        icon.transform.SetParent(parent, false);
        icon.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = icon.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);

        Image img = icon.AddComponent<Image>();
        img.color = Color.white;
        img.preserveAspect = true;

        LayoutElement le = icon.AddComponent<LayoutElement>();
        le.preferredHeight = 100;
        le.preferredWidth = 100;
    }

    private static GameObject CreateDetailText(Transform parent, string name, int fontSize, FontStyles style)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        textObj.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 30);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = name.Replace("Text", "");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.9f, 0.95f, 1f, 1f);

        LayoutElement le = textObj.AddComponent<LayoutElement>();
        le.preferredHeight = fontSize + 10;

        return textObj;
    }

    private static GameObject CreateDetailButton(Transform parent, string name, string label)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        buttonObj.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = buttonObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 50);

        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.8f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = img;
        
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.5f, 0.8f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.6f, 0.9f, 1f);
        colors.pressedColor = new Color(0.15f, 0.4f, 0.7f, 1f);
        button.colors = colors;

        LayoutElement le = buttonObj.AddComponent<LayoutElement>();
        le.preferredHeight = 50;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("UI");

        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        textRt.anchoredPosition = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 18;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        return buttonObj;
    }

    private static GameObject CreateBackButton(Transform parent)
    {
        GameObject buttonObj = new GameObject("BackButton");
        buttonObj.transform.SetParent(parent, false);
        buttonObj.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = buttonObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        rt.anchoredPosition = new Vector2(20, 20);
        rt.sizeDelta = new Vector2(150, 50);

        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = img;
        
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        button.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("UI");

        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        textRt.anchoredPosition = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "← Back";
        text.fontSize = 18;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        return buttonObj;
    }

    private static void CreateTitle(Transform parent, string titleText)
    {
        GameObject title = new GameObject("Title");
        title.transform.SetParent(parent, false);
        title.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = title.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 60);

        TextMeshProUGUI text = title.AddComponent<TextMeshProUGUI>();
        text.text = titleText;
        text.fontSize = 32;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(1f, 0.9f, 0.5f, 1f);

        LayoutElement le = title.AddComponent<LayoutElement>();
        le.preferredHeight = 60;
    }

    private static GameObject CreateButton(Transform parent, string name, string label)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        buttonObj.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = buttonObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 60);

        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        img.raycastTarget = true;

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = img;
        button.interactable = true;
        
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        colors.selectedColor = new Color(0.96f, 0.96f, 0.96f, 1f);
        colors.disabledColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        Navigation nav = button.navigation;
        nav.mode = Navigation.Mode.Automatic;
        button.navigation = nav;

        LayoutElement le = buttonObj.AddComponent<LayoutElement>();
        le.preferredHeight = 60;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("UI");

        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        textRt.anchoredPosition = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 24;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        return buttonObj;
    }

    private static GameObject CreatePauseMenuManager()
    {
        GameObject managerObj = new GameObject("PauseMenuManager");
        managerObj.AddComponent<PauseMenuManager>();
        Debug.Log("Created PauseMenuManager");
        return managerObj;
    }

    private static void SetupPauseMenuManager(GameObject managerObj, GameObject pausePanel, GameObject spellsPanel)
    {
        PauseMenuManager manager = managerObj.GetComponent<PauseMenuManager>();
        if (manager == null) return;

        SerializedObject so = new SerializedObject(manager);

        so.FindProperty("pauseMenuPanel").objectReferenceValue = pausePanel;
        so.FindProperty("spellsMenuPanel").objectReferenceValue = spellsPanel;
        
        Transform spellDetailPanel = spellsPanel.transform.Find("SpellDetailPanel");
        if (spellDetailPanel != null)
        {
            so.FindProperty("spellDetailPanel").objectReferenceValue = spellDetailPanel.gameObject;
        }

        GameObject inventoryPanel = GameObject.Find("InventoryPanel");
        if (inventoryPanel != null)
        {
            so.FindProperty("inventoryMenuPanel").objectReferenceValue = inventoryPanel;
        }

        Transform pauseContent = pausePanel.transform.Find("Content");
        if (pauseContent != null)
        {
            AssignButton(so, "resumeButton", pauseContent.Find("ResumeButton"));
            AssignButton(so, "spellsButton", pauseContent.Find("SpellsButton"));
            AssignButton(so, "inventoryButton", pauseContent.Find("InventoryButton"));
            AssignButton(so, "optionsButton", pauseContent.Find("OptionsButton"));
            AssignButton(so, "quitButton", pauseContent.Find("QuitButton"));
        }

        Transform spellsListContainer = spellsPanel.transform.Find("SpellsListPanel/ScrollView/Viewport/SpellsListContainer");
        if (spellsListContainer != null)
        {
            so.FindProperty("spellsListContainer").objectReferenceValue = spellsListContainer;
        }

        string[] spellSlotPaths = AssetDatabase.FindAssets("SpellSlot t:Prefab");
        if (spellSlotPaths.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(spellSlotPaths[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            so.FindProperty("spellSlotPrefab").objectReferenceValue = prefab;
        }

        Transform backButton = spellsPanel.transform.Find("BackButton");
        if (backButton != null)
        {
            AssignButton(so, "backButton", backButton);
        }

        if (spellDetailPanel != null)
        {
            Transform detailContent = spellDetailPanel.Find("Content");
            if (detailContent != null)
            {
                AssignTextMeshPro(so, "spellNameText", detailContent.Find("SpellNameText"));
                AssignTextMeshPro(so, "spellDescriptionText", detailContent.Find("SpellDescriptionText"));
                AssignTextMeshPro(so, "manaCostText", detailContent.Find("ManaCostText"));
                AssignTextMeshPro(so, "equipStatusText", detailContent.Find("EquipStatusText"));
                AssignImage(so, "spellIconImage", detailContent.Find("SpellIconImage"));
                AssignButton(so, "equipButton", detailContent.Find("EquipButton"));
                AssignButton(so, "unequipButton", detailContent.Find("UnequipButton"));
            }
        }

        so.ApplyModifiedProperties();
        Debug.Log("PauseMenuManager configured with all references");
    }

    private static void AssignButton(SerializedObject so, string propertyName, Transform transform)
    {
        if (transform != null)
        {
            Button button = transform.GetComponent<Button>();
            if (button != null)
            {
                so.FindProperty(propertyName).objectReferenceValue = button;
            }
        }
    }

    private static void AssignTextMeshPro(SerializedObject so, string propertyName, Transform transform)
    {
        if (transform != null)
        {
            TextMeshProUGUI text = transform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                so.FindProperty(propertyName).objectReferenceValue = text;
            }
        }
    }

    private static void AssignImage(SerializedObject so, string propertyName, Transform transform)
    {
        if (transform != null)
        {
            Image image = transform.GetComponent<Image>();
            if (image != null)
            {
                so.FindProperty(propertyName).objectReferenceValue = image;
            }
        }
    }
}
