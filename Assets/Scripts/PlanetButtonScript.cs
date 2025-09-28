using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlanetButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Image Settings")]
    public Image planetImage;
    public Sprite normalSprite;
    public Sprite hoverSprite;

    [Header("Fade Settings")]
    public Image fadeOverlay;
    public float fadeDuration = 1f;

    [Header("Scene Settings")]
    public string nextSceneName = "WormholeCutscene";

    [Header("Audio Settings")]
    [SerializeField] private string resourcesPath = "Audio/Planet"; // Assets/Resources/Audio/Planet.wav
    [SerializeField, Range(0f,1f)] private float volume = 0.7f;

    private bool isTransitioning = false;
    private AudioSource src;

    void Awake()
    {
        // Set up AudioSource
        src = GetComponent<AudioSource>();
        if (!src) src = gameObject.AddComponent<AudioSource>();

        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;
        src.volume = volume;

        // Load and play Planet.wav
        var clip = Resources.Load<AudioClip>(resourcesPath);
        if (clip != null)
        {
            src.clip = clip;
            src.Play();
        }
        else
        {
            Debug.LogError($"PlanetButtonScript: Could not find clip at Resources/{resourcesPath}");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isTransitioning && planetImage && hoverSprite)
            planetImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isTransitioning && planetImage && normalSprite)
            planetImage.sprite = normalSprite;
    }

    public void OnClick()
    {
        if (!isTransitioning)
            StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        isTransitioning = true;

        if (fadeOverlay)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.color = new Color(0, 0, 0, 0);

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsed / fadeDuration);
                fadeOverlay.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            fadeOverlay.color = Color.black;
        }

        yield return new WaitForSeconds(0.1f);

        // Stop music before loading
        if (src && src.isPlaying) src.Stop();

        SceneManager.LoadScene(nextSceneName);
    }
}