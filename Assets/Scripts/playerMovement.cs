using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float move;
    public float speed; // move speed
    public float jump; // jump force
    private bool canJump; // flag for can or cannot jump
    private bool isGrounded; // grounded flag
    public Transform groundCheck; // ground checker gizmo
    public LayerMask whatIsGround; // ground layer
    public float checkRadius; // ground checking radius

    public float coyoteTime; // allowed coyote time
    private float currCoyoteTime; // coyote timer

    public float bufferDistance; // allowed buffer distance for jump


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround); // bool for grounded check

        if(isGrounded == true)
        {
            currCoyoteTime = 0.0f;
            canJump = true;
        }
        else
        {
            currCoyoteTime += Time.deltaTime;
        }

        move = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(speed * move, rb.velocity.y);

        if(Input.GetButtonDown("Jump") && canJump == true && currCoyoteTime <= coyoteTime)
        {
            rb.AddForce(new Vector2(rb.velocity.x, jump));
            canJump = false;
        }
    }
}
