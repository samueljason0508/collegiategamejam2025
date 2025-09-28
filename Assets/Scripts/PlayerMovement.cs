using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;      // vertical speed
    [SerializeField] float gravityScale = 0f;   // 0 for no gravity side-scroller

    [Header("Blend Tree Param")]
    [SerializeField] string yVelParam = "yVelocity";

    [Header("Down Freeze (hysteresis)")]
    [Tooltip("When yVelocity <= this, and we're in Down, freeze on last frame.")]
    [SerializeField] float downFreezeThreshold = -0.75f;
    [Tooltip("When yVelocity >= this, unfreeze and resume normal anim.")]
    [SerializeField] float downReleaseThreshold = -0.55f;

    [Header("Animator State Names")]
    [SerializeField] string downStateName = "Down"; // exact state name on your 'Movement' layer
    [SerializeField] int animLayer = 0;             // usually 0 unless you moved the blend tree to another layer

    Rigidbody2D rb;
    Animator animator;
    PlayerInput playerInput;
    InputAction moveAction;

    float verticalInput;
    bool downFrozen;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];

        rb.gravityScale = gravityScale;
    }

    void OnEnable()  => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void Update()
    {
        // Read input (new Input System)
        Vector2 move = moveAction.ReadValue<Vector2>();
        verticalInput = move.y;

        // Drive the Blend Tree with vertical velocity (normalized to your speed)
        float yVel = verticalInput * moveSpeed;
        animator.SetFloat(yVelParam, yVel);

        HandleDownFreeze(yVel);
    }

    void FixedUpdate()
    {
        // Keep your horizontal value (auto-scroll elsewhere) and apply vertical
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * moveSpeed);
    }

    void HandleDownFreeze(float yVel)
    {
        var state = animator.GetCurrentAnimatorStateInfo(animLayer);
        bool inDown = state.IsName(downStateName);

        // Enter freeze: we're in Down, sufficiently negative y, and not already frozen
        if (!downFrozen && inDown && yVel <= downFreezeThreshold)
        {
            animator.Play(downStateName, animLayer, 1f); // jump to last frame
            animator.Update(0f);                         // apply immediately
            animator.speed = 0f;                         // freeze there
            downFrozen = true;
            return;
        }

        // Exit freeze: leave when y rises above release threshold OR we left Down
        if (downFrozen && (!inDown || yVel >= downReleaseThreshold))
        {
            animator.speed = 1f; // resume normal playback
            downFrozen = false;
        }

        // If frozen and still in Down, keep sampling the last frame each Update
        if (downFrozen && inDown)
        {
            animator.Play(downStateName, animLayer, 1f);
            animator.Update(0f);
        }
    }
}
