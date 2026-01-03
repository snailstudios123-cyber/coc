using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        }
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void TransitionToScene(string sceneName, Vector2 spawnPosition)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneName, spawnPosition));
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneName, Vector2.zero));
        }
    }

    private IEnumerator TransitionCoroutine(string sceneName, Vector2 spawnPosition)
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeOut());

        if (!SceneExists(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' not found in Build Settings! Add it via File -> Build Settings.");
            yield return StartCoroutine(FadeIn());
            isTransitioning = false;
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        if (asyncLoad == null)
        {
            Debug.LogError($"Failed to load scene '{sceneName}'. Make sure it's added to Build Settings.");
            yield return StartCoroutine(FadeIn());
            isTransitioning = false;
            yield break;
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && spawnPosition != Vector2.zero)
        {
            player.transform.position = spawnPosition;
        }

        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameFromPath == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color startColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        Color endColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = endColor;
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color startColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);
        Color endColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = endColor;
    }
}
