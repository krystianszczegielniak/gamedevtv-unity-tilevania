using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    float movementSpeed = 1.0f;

    private Rigidbody2D _rigidBody2D;

    private BoxCollider2D _boxCollider2D;

    private bool movingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float direction = movingRight ? 1 : -1;
        _rigidBody2D.velocity = new Vector2(movementSpeed * direction, _rigidBody2D.velocity.y);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the collided object is on the "Ground" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            ReverseDirection();
        }
    }

    void ReverseDirection()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
}
