using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class SpiderOnClick : MonoBehaviour
{
    public Transform player;        // set your Player transform
    public Transform rocketPrefab;  // prefab or leave null to spawn a simple capsule at runtime
    public Transform splatPrefab;   // optional

    public float rocketSpeed = 10f;
    public float lifetime = 3f;

    public int pointsToAdd = 10;
    public string prefix = "Points: ";
    public TextMeshProUGUI scoreText;

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var cam = Camera.main;
        if (cam == null || player == null) return;

        // Support mouse & touch; fall back if Mouse.current is null
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

    Transform SpawnRocketAt(Vector3 position)
    {
        Transform r;

        // SPAWN ROCKET SOUND HERE
        if (rocketPrefab != null)
        {
            r = Instantiate(rocketPrefab, position, Quaternion.identity);
        }
        else
        {
            // build a simple rocket at runtime if no prefab is provided
            var go = new GameObject("Rocket");
            go.transform.position = position;

            // visuals (optional)
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1), new Vector2(0.5f,0.5f), 100);
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

// Helper mover (same file)
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

        // face and move toward target
        Vector3 dir = (target.position - transform.position);
        float distSq = dir.sqrMagnitude;

        if (distSq > 0.0001f)
        {
            dir.Normalize();
            transform.up = dir; // orient "nose" toward spider
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

            // splat
            if (splatPrefab != null)
            {
                var s = Instantiate(splatPrefab, pos, Quaternion.identity);
                Destroy(s.gameObject, splatLifetime);
            }

            // update score safely
            if (scoreText != null)
            {
                int currentScore = 0;

                // Extract number after the prefix if present
                if (!string.IsNullOrEmpty(scoreText.text) &&
                    !string.IsNullOrEmpty(prefix) &&
                    scoreText.text.StartsWith(prefix))
                {
                    string numberPart = scoreText.text.Substring(prefix.Length);
                    int.TryParse(numberPart, out currentScore);
                }
                else
                {
                    // Try parse entire text if no prefix match
                    int.TryParse(scoreText.text, out currentScore);
                }

                int newScore = currentScore + pointsToAdd;
                scoreText.text = (prefix ?? string.Empty) + newScore;
            }

            // EXPLOSION SOUND HERE 
            // EXPLOSION SOUND HERE 
            var deathClip = Resources.Load<AudioClip>("Audio/spider_death"); // no extension
            if (deathClip != null) {
                AudioSource.PlayClipAtPoint(deathClip, pos, 1f); // play at spider position, volume 1
            }
            else
            {
            Debug.LogError("RocketMover: Could not find AudioClip at Resources/Audio/spider_death");
            }
            // destroy rocket
            Destroy(gameObject);
        }
    }
}
