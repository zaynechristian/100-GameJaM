using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    PlayerStateBase _currentState;
    public MovingState movingState = new MovingState();
    public HurtState hurtState = new HurtState();
    public SkillState skillState = new SkillState();

    public LayerMask groundLayer;

    public Rigidbody2D rb;
    public float speed = 12f;
    public float jumpingPower = 50f;
    public float dashingPower = 25f;

    public float groundCheckHeight = 2.9f;

    public Animator playerAnimator;
    public SpriteRenderer playerSpriteRenderer;
    public BoxCollider2D playerBoxCollider2D;




    void Start()
    {
        _currentState = movingState;
        movingState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState(this);
    }

    void FixedUpdate() {
        _currentState.FixedUpdateState(this);
    }

    void LateUpdate() {
        _currentState.LateUpdateState(this);
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckHeight, groundLayer); 
        return hit.collider != null;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * groundCheckHeight));
    }
}
