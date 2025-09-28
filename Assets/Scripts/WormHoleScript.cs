using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WormHoleScript : MonoBehaviour
{
    public Image cutsceneImage;          // UI Image component
    public Sprite[] frames;              // Array of sliced sprites
    public float frameRate = 0.05f;      // Seconds per frame
    public string nextScene = "FullDropCutScene"; // Scene to load after cutscene

    [Header("Audio Settings")]
    [SerializeField] private string resourcesPath = "Audio/swirl"; // Assets/Resources/Audio/swirl.wav
    [SerializeField, Range(0f,1f)] private float volume = 1f;

    private void Start()
    {
        if (frames.Length > 0 && cutsceneImage != null)
            StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // --- Play swirl.wav once at the start ---
        var clip = Resources.Load<AudioClip>(resourcesPath);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, volume);
        }
        else
        {
            Debug.LogError($"WormHoleScript: Could not find clip at Resources/{resourcesPath}");
        }

        // --- Play cutscene frames ---
        foreach (Sprite frame in frames)
        {
            cutsceneImage.sprite = frame;
            yield return new WaitForSeconds(frameRate);
        }

        // Optional: small delay before loading scene
        yield return new WaitForSeconds(0.1f);

        // Load the next scene
        SceneManager.LoadScene(nextScene);
    }
}