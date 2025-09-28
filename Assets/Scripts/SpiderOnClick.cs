using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class SpiderOnClick : MonoBehaviour
{
    public Transform player;        // set your Player transform
    public Transform rocketPrefab;  // prefab or leave null to spawn a simple capsule at runtime
    public float rocketSpeed = 10f;

    public Transform splatPrefab;   // optional
    public float lifetime = 3f;

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var cam = Camera.main;
        if (cam == null || player == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        var ray = cam.ScreenPointToRay(screenPos);
        var hit = Physics2D.GetRayIntersection(ray);

        // Only act if THIS spider was clicked
        if (hit.collider == null || hit.collider.transform != transform) return;

        // spawn the rocket from the player, aimed at THIS spider
        var rocketT = SpawnRocketAt(player.position);
        var mover = rocketT.gameObject.AddComponent<RocketMover>();
        mover.Init(target: transform, speed: rocketSpeed, splatPrefab: splatPrefab, splatLifetime: lifetime);
    }

    Transform SpawnRocketAt(Vector3 position)
    {
        Transform r;
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
        rb.isKinematic = true;
        rb.gravityScale = 0f;

        var col = r.GetComponent<Collider2D>();
        if (col == null) col = r.gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        return r;
    }
}

// Helper mover (same file)
class RocketMover : MonoBehaviour
{
    Transform target;
    float speed;
    Transform splatPrefab;
    float splatLifetime;

    Collider2D targetCol;

    public void Init(Transform target, float speed, Transform splatPrefab, float splatLifetime)
    {
        this.target = target;
        this.speed = Mathf.Max(0f, speed);
        this.splatPrefab = splatPrefab;
        this.splatLifetime = splatLifetime;
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
        Vector3 dir = (target.position - transform.position).normalized;
        if (dir.sqrMagnitude > 0f)
            transform.up = dir; // orient "nose" toward spider

        transform.position += dir * speed * Time.deltaTime;
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

            // destroy rocket
            Destroy(gameObject);
        }
    }
}
