using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlanetButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Image Settings")]
    public Image planetImage;        // Drag the planet Image component here
    public Sprite normalSprite;      // Default planet sprite
    public Sprite hoverSprite;       // Sprite when hovered over

    [Header("Fade Settings")]
    public Image fadeOverlay;        // Fullscreen black Image (UI) for fade out
    public float fadeDuration = 1f;  // Duration of fade

    [Header("Scene Settings")]
    public string nextSceneName = "WormholeCutscene"; // Scene to load after click

    private bool isTransitioning = false;

    // Hover enter
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isTransitioning && planetImage != null && hoverSprite != null)
            planetImage.sprite = hoverSprite;
    }

    // Hover exit
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isTransitioning && planetImage != null && normalSprite != null)
            planetImage.sprite = normalSprite;
    }

    // Called when the planet is clicked
    public void OnClick()
    {
        if (!isTransitioning)
            StartCoroutine(FadeAndLoadScene());
    }

    // Coroutine for fading and loading scene
    private IEnumerator FadeAndLoadScene()
    {
        isTransitioning = true;

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.color = new Color(0, 0, 0, 0); // start transparent

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

        // Wait a tiny moment after fade
        yield return new WaitForSeconds(0.1f);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
