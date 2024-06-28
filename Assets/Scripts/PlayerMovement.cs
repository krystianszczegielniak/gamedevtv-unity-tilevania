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
    private CapsuleCollider2D myBodyCollider2D;
    private BoxCollider2D myFeetCollider2D;
    private float startingGravity;
    private bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();
        startingGravity = rb2d.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            Run();
            FlipSprite();
            ClimbLadder();
        }
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
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing")))
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
        if (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (value.isPressed)
            {
                rb2d.velocity += new Vector2(0f, jumpPower);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemies")))
        {
            Debug.Log("Dying");
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
