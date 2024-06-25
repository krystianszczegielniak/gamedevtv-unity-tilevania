using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 1000.0f;

    [SerializeField]
    private float jumpPower = 25f;

    [SerializeField]
    private float climbSpeed = 300.0f;

    private Animator animator;
    private Vector2 moveInput;
    private Rigidbody2D rb2d;
    private CapsuleCollider2D myCapsuleCollider2D;
    private float startingGravity;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        myCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        startingGravity = rb2d.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    private void Run()
    {
        Vector2 playerVelocity = new(moveInput.x * movementSpeed * Time.deltaTime, rb2d.velocity.y);
        rb2d.velocity = playerVelocity;
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb2d.velocity.x), 1f);
        }
        animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    private void ClimbLadder()
    {
        if (!myCapsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb2d.gravityScale = startingGravity;
            animator.SetBool("isClimbing", false);
            return;
        }
        Vector2 climbVelocity = new(rb2d.velocity.x, moveInput.y * climbSpeed * Time.deltaTime);
        rb2d.velocity = climbVelocity;
        rb2d.gravityScale = 0f;
        bool playerHasVerticalSpeed = Mathf.Abs(rb2d.velocity.y) > Mathf.Epsilon;
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void OnJump(InputValue value)
    {
        if (myCapsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (value.isPressed)
            {
                rb2d.velocity += new Vector2(0f, jumpPower);
            }
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
