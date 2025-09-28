using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float verticalInput;
    float moveSpeed = 5f;
    bool isGoingDown = false;

    Rigidbody2D rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");

        FlipSprite();


    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(verticalInput * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void FlipSprite()
    {
        if(isGoingDown && verticalInput < 0f || !isGoingDown && verticalInput > 0f)
        {
            isGoingDown = !isGoingDown;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }
}
