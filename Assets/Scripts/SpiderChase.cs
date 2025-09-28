using UnityEngine;

public class SpiderChase : MonoBehaviour
{
    public Transform target;          
    public string playerTag = "Player";
    public float speed = 3f;
    public float stopDistance = 0.3f;
    public bool lockZ = true;

        private bool gameOverTriggered = false;


    void Start()
    {
        if (target == null)
            target = GameObject.FindWithTag(playerTag)?.transform;

        if (target == null) { enabled = false; }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 pos = transform.position;
        Vector3 tpos = target.position;
        if (lockZ) tpos.z = pos.z;

        if (Vector2.Distance(pos, tpos) <= stopDistance) return;

        transform.position = Vector2.MoveTowards(pos, tpos, speed * Time.deltaTime);
    }

    // Triggered if colliders are marked "Is Trigger"
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameOverTriggered && other.CompareTag(playerTag))
            TriggerGameOver();
    }

    // Triggered if using non-trigger colliders
void OnCollisionEnter2D(Collision2D other)
{
    if (!gameOverTriggered && other.gameObject.CompareTag(playerTag))
        TriggerGameOver();
}

    private void TriggerGameOver()
    {
        gameOverTriggered = true;
        Debug.Log("Game Over");



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


        // Option 2: Or trigger your pixel fade/transition system instead
        // Example:
        // PixelFadeManager.Instance.StartFadeOutThenGameOver();
    }
}
