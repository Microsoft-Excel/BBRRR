using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float baseMoveAcceleration = 0.5f;
    [SerializeField] private float airControlFactor = 0.1f;
    [SerializeField] float jumpApexControlFactor = 2f;
    [SerializeField] float jumpForce = 8f;
    [SerializeField] float floatSeconds = 0.1f;
    [SerializeField] float jumpBuffer = 0.1f;
    [SerializeField] float jumpCooldown = 0.1f;
    [SerializeField] float wallJumpTime = 0.1f;

    private Transform _transform;
    private Rigidbody2D _rb;

    private bool grounded = false;
    private float lastGrounded = 0f;
    private float lastJumpInput = 0f;
    private float lastJump = 0f;
    private float lastWallJump = 0f;
    private float moveAccelerationMultiplier = 1f;
    private float xVelocity = 0f;
    private Vector2 lastContactNormal = Vector2.zero;

    private class Layers
    {
        public static int Level = 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // print(moveAcceleration * moveAccelerationMultiplier);
        if (!grounded && lastGrounded > 0) { lastGrounded -= Time.deltaTime; }
        if (!grounded && lastJumpInput > 0) { lastJumpInput -= Time.deltaTime; }
        if (lastWallJump > 0) { lastWallJump -= Time.deltaTime; }
        if (lastJump > 0) { lastJump -= Time.deltaTime; }

        // print(lastWallJump);

        if (lastWallJump <= 0)
        {
            float xAxis = Input.GetAxis("Horizontal");
            float moveAcceleration = baseMoveAcceleration * moveAccelerationMultiplier;
            _rb.velocity += (xAxis * moveSpeed - _rb.velocity.x) * moveAcceleration * Vector2.right;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (CanJump())
            {
                Jump();
            }
            else
            {
                lastJumpInput = jumpBuffer;
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        lastContactNormal = collision.GetContact(0).normal;
        // float contactAngle = Mathf.Atan2(contactNormal.y, contactNormal.x);
        if (collision.gameObject.layer == Layers.Level)
        {
            grounded = true;
            lastGrounded = floatSeconds;
            if (lastJumpInput > 0 || Input.GetButton("Jump")) { Jump(); }
            _rb.gravityScale = 0.5f;
            moveAccelerationMultiplier = 1f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Layers.Level)
        {
            grounded = false;
            _rb.gravityScale = 1f;
            moveAccelerationMultiplier = airControlFactor;
        }
    }

    private bool CanJump()
    {
        return grounded || lastGrounded > 0;
    }

    private void Jump()
    {
        if (lastJump <= 0)
        {
            lastJump = jumpCooldown;
            _rb.velocity = Vector2.zero;
            _rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            print(lastContactNormal);
            if (lastContactNormal.Equals(Vector2.left) || lastContactNormal.Equals(Vector2.right))
            {
                lastWallJump = wallJumpTime;
                _rb.velocity += lastContactNormal * moveSpeed;
            }
        }
        
    }
}
