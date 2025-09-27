using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WormHoleScript : MonoBehaviour
{
    public Image cutsceneImage;          // UI Image component
    public Sprite[] frames;              // Array of sliced sprites
    public float frameRate = 0.05f;      // Seconds per frame
    public string nextScene = "FullDropCutScene"; // Scene to load after cutscene

    private void Start()
    {
        if (frames.Length > 0 && cutsceneImage != null)
            StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        foreach (Sprite frame in frames)
        {
            cutsceneImage.sprite = frame;
            yield return new WaitForSeconds(frameRate);
        }

        // Optional: small fade or delay
        yield return new WaitForSeconds(0.1f);

        // Load the next scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
}
