using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class SpiderOnClick : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;        // assign your Player transform in Inspector
    public Transform rocketPrefab;  // optional: prefab for the rocket
    public Transform splatPrefab;   // optional: decal/splat prefab

    [Header("Rocket")]
    public float rocketSpeed = 10f;
    public float lifetime = 3f;     // used for splat lifetime

    [Header("Scoring")]
    public int pointsToAdd = 10;
    public string prefix = "Points: ";
    public TextMeshProUGUI scoreText;

    // Input System: hook this action from PlayerInput (UI/Button) or call manually
    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var cam = Camera.main;
        if (cam == null || player == null) return;

        // Support mouse & touch
        Vector2 screenPos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        var ray = cam.ScreenPointToRay(screenPos);
        var hit = Physics2D.GetRayIntersection(ray);

        // Only act if THIS spider was clicked
        if (hit.collider == null || hit.collider.transform != transform) return;

        // spawn the rocket from the player, aimed at THIS spider
        var rocketT = SpawnRocketAt(player.position);
        var mover = rocketT.gameObject.AddComponent<RocketMover>();
        mover.Init(
            target: transform,
            speed: rocketSpeed,
            splatPrefab: splatPrefab,
            splatLifetime: lifetime,
            scoreText: scoreText,
            prefix: prefix,
            pointsToAdd: pointsToAdd
        );
    }

    private Transform SpawnRocketAt(Vector3 position)
    {
        Transform r;

        // (Optional) play a spawn sfx here with your own system if desired

        if (rocketPrefab != null)
        {
            r = Instantiate(rocketPrefab, position, Quaternion.identity);
        }
        else
        {
            // build a simple visible rocket at runtime if no prefab is provided
            var go = new GameObject("Rocket");
            go.transform.position = position;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1),
                                      new Vector2(0.5f, 0.5f), 100);
            go.transform.localScale = new Vector3(0.2f, 0.6f, 1f);
            r = go.transform;
        }

        // physics (needed for trigger events)
        var rb = r.GetComponent<Rigidbody2D>();
        if (rb == null) rb = r.gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        var col = r.GetComponent<Collider2D>();
        if (col == null) col = r.gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        return r;
    }
}

// ---------------------------------------------

public class RocketMover : MonoBehaviour
{
    Transform target;
    float speed;
    Transform splatPrefab;
    float splatLifetime;
    Collider2D targetCol;

    // scoring refs (passed in from SpiderOnClick)
    TextMeshProUGUI scoreText;
    string prefix;
    int pointsToAdd;

    // change this path if your clip is elsewhere
    private const string DeathClipPath = "Audio/spider_death"; // Assets/Resources/Audio/spider_death.(wav|ogg|mp3)

    public void Init(
        Transform target,
        float speed,
        Transform splatPrefab,
        float splatLifetime,
        TextMeshProUGUI scoreText,
        string prefix,
        int pointsToAdd)
    {
        this.target = target;
        this.speed = Mathf.Max(0f, speed);
        this.splatPrefab = splatPrefab;
        this.splatLifetime = splatLifetime;

        this.scoreText = scoreText;
        this.prefix = prefix ?? string.Empty;
        this.pointsToAdd = pointsToAdd;

        targetCol = target ? target.GetComponent<Collider2D>() : null;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // move toward target; point the "nose" (local up) at it
        Vector3 dir = (target.position - transform.position);
        if (dir.sqrMagnitude > 0.0001f)
        {
            dir.Normalize();
            transform.up = dir;
            transform.position += dir * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (target == null) return;

        // only react when we touch THIS rocket's intended spider
        if (other.transform == target || other == targetCol)
        {
            Vector3 pos = target.position;

            // destroy spider
            Destroy(target.gameObject);

            // splat (optional)
            if (splatPrefab != null)
            {
                var s = Instantiate(splatPrefab, pos, Quaternion.identity);
                Destroy(s.gameObject, splatLifetime);
            }

            // update score safely
            if (scoreText != null)
            {
                int currentScore = 0;

                if (!string.IsNullOrEmpty(scoreText.text) &&
                    !string.IsNullOrEmpty(prefix) &&
                    scoreText.text.StartsWith(prefix))
                {
                    string numberPart = scoreText.text.Substring(prefix.Length);
                    int.TryParse(numberPart, out currentScore);
                }
                else
                {
                    int.TryParse(scoreText.text, out currentScore);
                }

                int newScore = currentScore + pointsToAdd;
                scoreText.text = (prefix ?? string.Empty) + newScore;
            }

            // play death sound (2D so it's always audible)
            var deathClip = Resources.Load<AudioClip>(DeathClipPath); // no extension
            if (deathClip != null)
            {
                AudioUtil.Play2DOneShot(deathClip, 1f);
            }
            else
            {
                Debug.LogError($"RocketMover: Could not find AudioClip at Resources/{DeathClipPath}");
            }

            // destroy rocket
            Destroy(gameObject);
        }
    }
}

// ---------------------------------------------

public static class AudioUtil
{
    /// <summary>
    /// Play a guaranteed-audible 2D one-shot (no 3D falloff, no mixer dependency).
    /// </summary>
    public static void Play2DOneShot(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;

        var go = new GameObject("OneShot2D");
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f; // 2D
        src.volume = Mathf.Clamp01(volume);
        src.clip = clip;
        src.Play();

        Object.Destroy(go, clip.length / Mathf.Max(0.01f, src.pitch));
    }
}
