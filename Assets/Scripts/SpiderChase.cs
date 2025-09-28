using UnityEngine;

public class SpiderChase : MonoBehaviour
{
    public Transform target;          
    public string playerTag = "Player";
    public float speed = 3f;
    public float stopDistance = 0.3f;
    public bool lockZ = true;

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

    // Fires if either collider is marked "Is Trigger"
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
            Debug.Log("Game Over");
    }

    // Fires if using non-trigger colliders
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "Player")
            Debug.Log("Game Over");
    }
}
