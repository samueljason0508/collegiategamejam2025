using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Speeds")]
    public float autoScrollSpeed = 5f;
    public float verticalSpeed = 5f;

    [Header("Input")]
    private Vector2 moveInput;

    public bool IsMoving { get; private set; }

    private Rigidbody2D rb;

    // 1 = normal, -1 = inverted (W goes down, S goes up)
    private int verticalInvert = 1;
    private Coroutine invertRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Always scroll right; vertical controlled by input * invert
        rb.Velocity = new Vector2(autoScrollSpeed, moveInput.y * verticalSpeed * verticalInvert);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = !Mathf.Approximately(moveInput.y, 0f);
    }

    // --- Inversion controls ---

    /// <summary>Manually set inverted state (true = invert).</summary>
    public void SetVerticalInverted(bool inverted)
    {
        verticalInvert = inverted ? -1 : 1;
    }

    /// <summary>Invert vertical controls for a duration, then restore.</summary>
    public void InvertVerticalFor(float seconds)
    {
        // Optional: debug to verify the value coming from the pickup
        // Debug.Log($"InvertVerticalFor called with seconds={seconds}");

        if (invertRoutine != null) StopCoroutine(invertRoutine);
        invertRoutine = StartCoroutine(InvertCo(Mathf.Max(0f, seconds)));
    }

    private System.Collections.IEnumerator InvertCo(float seconds)
    {
        verticalInvert = -1;
        yield return new WaitForSeconds(seconds);
        verticalInvert = 1;
        invertRoutine = null;
    }
}