using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PixelFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public float delayBetween = 0.02f;        // time between each block appearing

    [Header("Audio Settings")]
    [SerializeField] private string resourcesPath = "Audio/game_over"; // Assets/Resources/Audio/game_over.wav
    [SerializeField, Range(0f,1f)] private float volume = 1f;

    private List<Image> blocks = new List<Image>();
    private AudioSource src;

    void Awake()
    {
        // Collect all child blocks
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f); // fully opaque at start
                blocks.Add(img);
            }
        }

        // Ensure an AudioSource exists
        src = GetComponent<AudioSource>();
        if (src == null) src = gameObject.AddComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D sound
        src.volume = volume;
    }

    void Start()
    {
        // Load and start looping game_over.wav
        var clip = Resources.Load<AudioClip>(resourcesPath);
        if (clip != null)
        {
            src.clip = clip;
            src.Play();
        }
        else
        {
            Debug.LogError($"PixelFade: Could not find AudioClip at Resources/{resourcesPath}");
        }

        StartCoroutine(FadeInBlocks());
    }

    IEnumerator FadeInBlocks()
    {
        // Shuffle blocks for random appearance
        for (int i = 0; i < blocks.Count; i++)
        {
            int rand = Random.Range(i, blocks.Count);
            var temp = blocks[i];
            blocks[i] = blocks[rand];
            blocks[rand] = temp;
        }

        // Show each block instantly (alpha 0 → 1)
        foreach (Image img in blocks)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
            yield return new WaitForSeconds(delayBetween);
        }

        yield return new WaitForSeconds(0.2f);
    }

    // --- Call this from the Play Again button's OnClick() ---
    public void StopGameOverMusic()
    {
        if (src != null && src.isPlaying)
        {
            src.Stop();
        }
    }
}