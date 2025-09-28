using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LandingScript : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public Image cutsceneImage;      // Fullscreen UI Image to display frames
    public Sprite[] frames;          // Array of sliced sprites
    public float frameRate = 0.05f;  // Seconds per frame

    [Header("Scene Settings")]
    public string nextSceneName = "Current"; // Scene to load after cutscene

    [Header("Audio Settings")]
    [SerializeField] private string resourcesPath = "Audio/retro_rocket"; // Assets/Resources/Audio/swirl.wav
    [SerializeField, Range(0f,1f)] private float volume = 1f;

    private void Start()
    {
        if (frames.Length > 0 && cutsceneImage != null)
        {
            StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene()
    {
        // --- Play swirl.wav once at the start ---
        var clip = Resources.Load<AudioClip>(resourcesPath);
        if (clip != null)
        {
            // PlayOneShot = plays once, does not loop, doesn’t overwrite other clips
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, volume);
        }
        else
        {
            Debug.LogError($"LandingScript: Could not find clip at Resources/{resourcesPath}");
        }

        // --- Show frames ---
        foreach (Sprite frame in frames)
        {
            cutsceneImage.sprite = frame;
            yield return new WaitForSeconds(frameRate);
        }

        // Optional short delay
        yield return new WaitForSeconds(0.1f);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}