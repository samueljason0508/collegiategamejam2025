using UnityEngine;

public class CombinedBackgroundScroller : MonoBehaviour
{
    public Transform[] backgroundSprites; // 4 sprites: BL, BR, TL, TR
    public Camera cam;

    float tileW, tileH;

    void Start()
    {
        var sr = backgroundSprites[0].GetComponent<SpriteRenderer>();
        tileW = sr.bounds.size.x;
        tileH = sr.bounds.size.y;

        // Align the grid so the camera begins inside the bottom-left tile
        Vector3 c = cam.transform.position;
        float baseX = Mathf.Floor(c.x / tileW) * tileW;
        float baseY = Mathf.Floor(c.y / tileH) * tileH;

        // Place 2×2 tiles
        backgroundSprites[0].position = new Vector3(baseX,          baseY,          0); // BL
        backgroundSprites[1].position = new Vector3(baseX + tileW,  baseY,          0); // BR
        backgroundSprites[2].position = new Vector3(baseX,          baseY + tileH,  0); // TL
        backgroundSprites[3].position = new Vector3(baseX + tileW,  baseY + tileH,  0); // TR
    }

    void Update()
    {
        Vector3 camPos = cam.transform.position;

        for (int i = 0; i < backgroundSprites.Length; i++)
        {
            var bg = backgroundSprites[i];
            Vector3 p = bg.position;

            // Wrap horizontally
            if (p.x < camPos.x - tileW)       p.x += tileW * 2f;
            else if (p.x > camPos.x + tileW)  p.x -= tileW * 2f;

            // Wrap vertically
            if (p.y < camPos.y - tileH)       p.y += tileH * 2f;
            else if (p.y > camPos.y + tileH)  p.y -= tileH * 2f;

            bg.position = p;
        }
    }
}
