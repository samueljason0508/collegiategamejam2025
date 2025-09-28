using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    [Header("Animation Settings")]
    public Image backgroundImage;    // UI Image covering the screen
    public Sprite[] frames;          // Array of background frames
    public float frameRate = 0.1f;   // Seconds per frame
    public bool loop = true;         // Should the animation repeat?

    private void Start()
    {
        if (frames.Length > 0 && backgroundImage != null)
        {
            StartCoroutine(PlayBackground());
        }
    }

    private IEnumerator PlayBackground()
    {
        do
        {
            foreach (Sprite frame in frames)
            {
                backgroundImage.sprite = frame;
                yield return new WaitForSeconds(frameRate);
            }
        }
        while (loop);
    }
}
