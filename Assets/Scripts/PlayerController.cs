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

    private void FixedUpdate()
    {
        // always scroll right; vertical controlled by input * invert
        rb.linearVelocity = new Vector2(autoScrollSpeed, moveInput.y * verticalSpeed * verticalInvert);
        // (Use rb.velocity, not rb.linearVelocity)
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = !Mathf.Approximately(moveInput.y, 0f);
    }

    // --- Inversion controls ---

    public void SetVerticalInverted(bool inverted)
    {
        verticalInvert = inverted ? -1 : 1;
    }

    public void InvertVerticalFor(float seconds)
    {
        if (invertRoutine != null) StopCoroutine(invertRoutine);
        invertRoutine = StartCoroutine(InvertCo(seconds));
    }

    private System.Collections.IEnumerator InvertCo(float seconds)
    {
        verticalInvert = -1;
        yield return new WaitForSeconds(seconds);
        verticalInvert = 1;
        invertRoutine = null;
    }
}
