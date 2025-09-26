using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float autoScrollSpeed = 5f; 
    public float verticalSpeed = 5f;   
    Vector2 moveInput;

    public bool IsMoving { get; private set; }

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // will always move right but can control W and S
        rb.linearVelocity = new Vector2(autoScrollSpeed, moveInput.y * verticalSpeed);

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // IsMoving = moveInput != Vector2.zero;
        IsMoving = moveInput.y != 0f;
    }
}
 