using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PixelFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public float delayBetween = 0.02f;        // time between each block appearing

    private List<Image> blocks = new List<Image>();

    void Awake()
    {
        // Collect all child blocks
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f); // start transparent
                blocks.Add(img);
            }
        }
    }

    void Start()
    {
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

        // Show each block instantly (jump alpha from 0 → 1)
        foreach (Image img in blocks)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0); 
            yield return new WaitForSeconds(delayBetween);
        }

        // Small delay before scene change
        yield return new WaitForSeconds(0.2f);

    }
}
