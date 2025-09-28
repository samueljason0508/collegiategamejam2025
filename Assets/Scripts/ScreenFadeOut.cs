using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenFadeOut : MonoBehaviour
{
    public float fadeDuration = 1f;         // Duration of fade
    public string nextSceneName = "GameOver"; // Scene to load after fade

    private Image fadeImage;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        if (fadeImage == null)
            Debug.LogError("ScreenFadeOut requires an Image component!");
        fadeImage.color = new Color(0, 0, 0, 0); // start transparent
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = Color.black;

        // Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
