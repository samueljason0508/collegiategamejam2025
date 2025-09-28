using UnityEngine;
using TMPro;

public class OilTracker : MonoBehaviour
{
    public int ratePerSecond = 10;

    TMP_Text label;
    int current;
    float carry;        
    bool running;
    bool finished;

    void Awake()
    {
        label = GetComponent<TMP_Text>();
        if (!int.TryParse(label.text, out current)) current = 0;
        running = current > 0;
        carry = 0f;
        finished = false;
        label.text = current.ToString();
    }

    void Update()
    {
        // pick up external changes 
        int parsed;
        if (int.TryParse(label.text, out parsed) && parsed != current)
        {
            current = Mathf.Max(0, parsed);
            running = current > 0;
            if (current == 0 && !finished) { finished = true; OnFinish(); }
        }

        if (!running) return;

        carry += Time.unscaledDeltaTime * Mathf.Max(0, ratePerSecond);
        if (carry < 1f) return;

        int dec = Mathf.FloorToInt(carry);
        carry -= dec;

        int before = current;
        current = Mathf.Max(0, current - dec);

        if (current != before)
            label.text = current.ToString();

        if (current == 0)
        {
            running = false;
            if (!finished)
            {
                finished = true;
                OnFinish();
            }
                
        }
    }

void OnFinish()
{
    Debug.Log("Run Out");

    // Trigger the fade out instead of immediately loading the scene
ScreenFadeOut fade = Object.FindAnyObjectByType<ScreenFadeOut>();
    if (fade != null)
    {
        fade.StartFadeOut();
    }
    else
    {
        Debug.LogWarning("No ScreenFadeOut found in the scene!");
        // fallback: load scene directly
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}
}
