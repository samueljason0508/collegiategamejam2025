using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(100)]                       // run late so we see final velocity
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerMain : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField] public float autoScrollSpeed = 5f;   // >0 right, <0 left
    [SerializeField] private float verticalSpeed   = 5f;

    [Header("Input")]
    [SerializeField] private float deadZone = 0.01f;

    [Header("Animator")]
    [SerializeField] private Animator animator;            // assign if not on same GO
    [SerializeField] private bool useBlendTree = true;     // true = use float param; false = bools
    [SerializeField] private string yVelParam = "yVelocity";
    [SerializeField] private string upBoolParam = "isUp";
    [SerializeField] private string downBoolParam = "isDown";
    [SerializeField] private string idleBoolParam = "isIdle";
    [SerializeField] private int animSign = 1;             // set to -1 if your tree's sign is reversed

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // +1 = normal, -1 = inverted
    private int invertY = 1;
    private Coroutine invertRoutine;

    private bool animatorAnimatePhysics; // whether Animator updates in FixedUpdate

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();

        // Clean 2D movement baseline
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (animator)
            animatorAnimatePhysics = (animator.updateMode == AnimatorUpdateMode.Fixed);
    }

    private void FixedUpdate()
    {
        // Compute vy from current input & inversion every physics tick
        float rawY = moveInput.y;
        float vy   = (Mathf.Abs(rawY) > deadZone ? rawY : 0f) * verticalSpeed * invertY;

        rb.linearVelocity = new Vector2(autoScrollSpeed, vy);

        if (animator && animatorAnimatePhysics)
            DriveAnimatorFromVelocity(vy);

        if (debugLogs)
            Debug.Log($"[PCM] vy={vy:0.###} rawY={rawY:0.###} invertY={invertY}", this);
    }

    private void Update()
    {
        // If Animator updates in 'Normal' mode, set params here (after physics ran)
        if (animator && !animatorAnimatePhysics)
        {
            float vy = rb.linearVelocity.y; // authoritative final velocity
            DriveAnimatorFromVelocity(vy);
        }
    }

    private void DriveAnimatorFromVelocity(float vy)
    {
        float signedVy = vy * animSign;

        if (useBlendTree)
        {
            animator.SetFloat(yVelParam, signedVy);
        }
        else
        {
            bool isUp   = signedVy >  deadZone;
            bool isDown = signedVy < -deadZone;
            bool isIdle = !isUp && !isDown;

            animator.SetBool(upBoolParam,   isUp);
            animator.SetBool(downBoolParam, isDown);
            animator.SetBool(idleBoolParam, isIdle);
        }
    }

    // PlayerInput (Behavior: Send Messages) -> Action name must be "Move"
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    // Called by DrugOreOnTouch
    public void InvertVerticalFor(float seconds)
    {
        if (invertRoutine != null) StopCoroutine(invertRoutine);
        invertRoutine = StartCoroutine(InvertCo(seconds));
    }

    private System.Collections.IEnumerator InvertCo(float seconds)
    {
        invertY = -1;
        if (debugLogs) Debug.Log($"[PlayerControllerMain] INVERT ON for {seconds}s", this);
        yield return new WaitForSeconds(seconds);
        invertY = 1;
        if (debugLogs) Debug.Log("[PlayerControllerMain] INVERT OFF", this);
        invertRoutine = null;
    }
}
