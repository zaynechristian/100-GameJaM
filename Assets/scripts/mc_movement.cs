using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mc_movement : MonoBehaviour
{
    public float horizontal;
    public float speed = 8f;
    
    public float jumpingpower = 16f;
    public bool jumpPending = false;
    public float jumpBufferT = 0.25f;
    public float jumpBufferCounter;
    
    public bool isfacingright = true;

    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;

    public float dashDir;
    public bool candash = true;
    public bool isdashing;
    public float dashingpower = 24f;
    public float dashingtime = 0.2f;
    public float dashingcooldown = 1f;

    public float groundCheckHeight = 2.9f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundcheck;
    [SerializeField] private LayerMask groundlayer;


    // Update is called once per frame
    void Update()
    {
        if (isdashing)
        {
            return;
        }

        horizontal = Input.GetAxis("Horizontal");

        if (IsGrounded()) {
            coyoteTimeCounter = coyoteTime;
        } else {
            coyoteTimeCounter -= Time.deltaTime;    
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpPending = true;
            jumpBufferCounter = jumpBufferT;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;

        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && candash)
        {
            StartCoroutine(Dash());
        }


        if (jumpPending) {
            jumpExec();
        }

        if (horizontal < 0f) {
            dashDir = -1f;
        } 
        if (horizontal > 0f) {
            dashDir = 1f;
        }
    }


    private void jumpExec() {
        if (coyoteTimeCounter > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, jumpingpower);
            jumpPending = false;
        } else {
            jumpBufferCounter -= Time.deltaTime;
            if (jumpBufferCounter <= 0f) {
                jumpPending = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isdashing)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        Debug.Log(IsGrounded());
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckHeight, groundlayer); 
        return hit.collider != null;
    }



    private IEnumerator Dash()
    {
        candash = false;
        isdashing = true;
        float originalgravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(dashDir * dashingpower, 0f);
        yield return new WaitForSeconds(dashingtime);
        rb.gravityScale = originalgravity;
        isdashing = false;
        yield return new WaitForSeconds(dashingcooldown);
        candash = true;
    }

}