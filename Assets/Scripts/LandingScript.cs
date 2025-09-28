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
    public string nextSceneName = "Current";     // Scene to load after cutscene

    private void Start()
    {
        if (frames.Length > 0 && cutsceneImage != null)
        {
            StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene()
    {
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
