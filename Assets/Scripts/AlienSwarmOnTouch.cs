using UnityEngine;
using System.Collections.Generic;

public class AlienSwarmOnTouch : MonoBehaviour
{
    public Transform pov;   // must have a SpriteRenderer
    public Sprite image;    // sprite to spawn
    public int count = 5;   // how many to spawn
    public float lifetime = 5f; // seconds before each despawns
    public float vibrateIntensity = 0.1f; // how much to vibrate
    public float vibrateSpeed = 10f; // how fast to vibrate

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.gameObject.name == "Player")) return;

        var povSR = pov.GetComponent<SpriteRenderer>();

        var size = povSR.bounds.size;
        float povWidth = size.x, povHeight = size.y;

        int targetOrder = povSR.sortingOrder + 1;

        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("SwarmImage_" + i);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = image;

            // Match POV's sorting layer, and set order = POV's + 1
            sr.sortingLayerName = povSR.sortingLayerName;
            sr.sortingOrder = targetOrder;

            float x = Random.Range(-povWidth * 0.5f, povWidth * 0.5f);
            float y = Random.Range(-povHeight * 0.5f, povHeight * 0.5f);
            go.transform.position = pov.position + new Vector3(x, y, 0f);
            go.transform.SetParent(pov, true);

            // Add vibration component
            var vibrator = go.AddComponent<Vibrator>();
            vibrator.Initialize(vibrateIntensity, vibrateSpeed);

            Destroy(go, lifetime);
        }

        Destroy(gameObject);
    }
}

public class Vibrator : MonoBehaviour
{
    private Vector3 originalPosition;
    private float intensity;
    private float speed;

    public void Initialize(float vibrateIntensity, float vibrateSpeed)
    {
        intensity = vibrateIntensity;
        speed = vibrateSpeed;
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        float offsetX = Mathf.Sin(Time.time * speed) * intensity * Random.Range(0.8f, 1.2f);
        float offsetY = Mathf.Cos(Time.time * speed * 1.1f) * intensity * Random.Range(0.8f, 1.2f);
        
        transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);
    }
}