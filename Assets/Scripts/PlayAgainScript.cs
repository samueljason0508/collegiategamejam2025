using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayAgainScript : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private string resourcesPath = "Audio/Menu-1"; // Assets/Resources/Audio/Menu-1.wav
    [SerializeField, Range(0f,1f)] private float volume = 0.7f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool fadeOutOnPlay = false;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Scene Settings")]
    [SerializeField] private string sceneName = "MainMenu";

    private AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        if (!src) src = gameObject.AddComponent<AudioSource>();

        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;  // 2D sound
        src.volume = volume;

        var clip = Resources.Load<AudioClip>(resourcesPath);
        if (!clip)
        {
            Debug.LogError($"PlayButtonScript: Could not find AudioClip at Resources/{resourcesPath}");
            return;
        }

        src.clip = clip;
        if (playOnStart) src.Play();
    }

    // Called when the Play button is clicked
    public void LoadGameplayScene()
    {
        Debug.Log("Play button clicked!");

        if (fadeOutOnPlay && src && src.isPlaying)
        {
            StartCoroutine(FadeOutThenLoad(sceneName, fadeDuration));
        }
        else
        {
            if (src && src.isPlaying) src.Stop();
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator FadeOutThenLoad(string targetScene, float duration)
    {
        float startVol = src.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        src.Stop();
        src.volume = startVol; // reset volume for next time
        SceneManager.LoadScene(targetScene);
    }
}