using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 5f;

    [SerializeField]
    private float jumpPower = 10f;

    [SerializeField]
    private float climbSpeed = 5f;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private Transform gun;

    [SerializeField]
    private Vector2 deathKick = new Vector2(10f, 10f);

    private Animator animator;
    private Vector2 moveInput;
    private Rigidbody2D rb2d;
    private CapsuleCollider2D myBodyCollider2D;
    private BoxCollider2D myFeetCollider2D;
    private float startingGravity;
    private bool isAlive = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();
        startingGravity = rb2d.gravityScale;
    }

    void Update()
    {
        if (isAlive)
        {
            FlipSprite();
            UpdateAnimations();
        }
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            Run();
            ClimbLadder();
        }
    }

    private void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * movementSpeed, rb2d.velocity.y);
        rb2d.velocity = playerVelocity;
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb2d.velocity.x), 1f);
        }
    }

    private void UpdateAnimations()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", playerHasHorizontalSpeed);

        bool playerHasVerticalSpeed = Mathf.Abs(rb2d.velocity.y) > Mathf.Epsilon;
        animator.SetBool(
            "isClimbing",
            playerHasVerticalSpeed
                && myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing"))
        );
    }

    private void ClimbLadder()
    {
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb2d.gravityScale = startingGravity;
            return;
        }

        Vector2 climbVelocity = new Vector2(rb2d.velocity.x, moveInput.y * climbSpeed);
        rb2d.velocity = climbVelocity;
        rb2d.gravityScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            Debug.Log("Dying");
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;
        rb2d.velocity = deathKick;
        animator.SetTrigger("Dying");
    }

    void OnMove(InputValue value)
    {
        if (!isAlive)
            return;
        moveInput = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        if (!isAlive)
            return;

        Instantiate(bullet, gun.position, transform.rotation);
    }

    void OnJump(InputValue value)
    {
        if (!isAlive)
            return;

        if (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (value.isPressed)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
            }
        }
    }
}
