using UnityEngine;

public class SpiderChaseDebug : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;        // drag Player here, or leave null to auto-find by tag
    [SerializeField] private string playerTag = "Player";

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 0.3f;
    [SerializeField] private bool lockZ = true;       // keep spider’s Z the same as its current Z

    [Header("Debug (read-only)")]
    [SerializeField] private float currentDistance;
    [SerializeField] private bool canChase;

    private void Start()
    {
        if (target == null)
            target = GameObject.FindWithTag(playerTag)?.transform;

        if (target == null)
            Debug.LogError($"[SpiderChaseDebug2D] No target found. Tag your player '{playerTag}' or assign the Transform.", this);

        if (Time.timeScale == 0f)
            Debug.LogWarning("[SpiderChaseDebug2D] Time.timeScale is 0 (paused). Movement won’t update.", this);

        if (speed <= 0f)
            Debug.LogWarning("[SpiderChaseDebug2D] Speed is <= 0. Set a positive speed.", this);

        if (stopDistance < 0f)
            Debug.LogWarning("[SpiderChaseDebug2D] stopDistance is negative. Set to >= 0.", this);
    }

    private void Update()
    {
        if (target == null) { canChase = false; return; }

        Vector3 pos = transform.position;
        Vector3 tpos = target.position;

        if (lockZ) tpos.z = pos.z; // keep depth constant for 2D

        currentDistance = Vector2.Distance(pos, tpos);

        // Early outs with clear reasons:
        if (speed <= 0f) { canChase = false; return; }
        if (currentDistance <= stopDistance) { canChase = false; return; }

        canChase = true;
        transform.position = Vector2.MoveTowards(pos, tpos, speed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.DrawLine(transform.position, new Vector3(target.position.x, target.position.y, lockZ ? transform.position.z : target.position.z));
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
