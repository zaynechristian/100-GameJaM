using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : PlayerStateBase
{
    Rigidbody2D _rb;
    Transform _transform;
    LayerMask _groundLayer;
    float _speed;
    float _horizontal;
    bool _isGrounded;

    Vector2 _dashDir = Vector2.right;
    float _dashingPower;
    bool _canDash = true;
    bool _isDashing;
    float _dashT;
    float _dashTimeLimit = 0.25f;
    float _dashCooldownT;
    float _dashCooldown = 0.5f;

    float _coyoteT;
    float _coyoteLimit = 0.2f;

    bool _jumpPending;
    float _jumpBufferT;
    float _jumpBuffer;
    bool _isJumping;

    Animator _animator;
    SpriteRenderer _playerSpriteRenderer;
    BoxCollider2D _playerBoxCollider2D;
    string _currentState;
    float _animLockT = 20f;
    bool _lockState = false;
    bool toTheLeft = false;
    public Vector2 colOffset = Vector2.right;
    int _currentJumpState;


    public override void EnterState(PlayerStateManager _PSM) {
        _rb = _PSM.rb;
        _transform = _PSM.transform;
        _groundLayer = _PSM.groundLayer;

        _dashingPower = _PSM.dashingPower;
        _speed = _PSM.speed;

        _coyoteT = _coyoteLimit;
        _dashCooldownT = _dashCooldown;

        _animator = _PSM.playerAnimator;
        _playerBoxCollider2D = _PSM.playerBoxCollider2D;
        _playerSpriteRenderer = _PSM.playerSpriteRenderer;
        _currentState = "Player_Idle";
        _currentJumpState = 1;

    }

    public override void UpdateState(PlayerStateManager _PSM)
    {
        var state = GetAnimState();

        if (state != _currentState) {
            _animator.Play(state);
            _currentState = state;
        }

        _horizontal = Input.GetAxisRaw("Horizontal");

        if (_horizontal < 0f) {
            _dashDir = Vector2.left;
            toTheLeft = true;
        } 
        if (_horizontal > 0f) {
            _dashDir = Vector2.right;
            toTheLeft = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            _dashT = _dashTimeLimit;
            _isDashing = true;
            _canDash = false;
        }

        if (toTheLeft) {
            _playerSpriteRenderer.flipX = true;
            colOffset = Vector2.left;
        } else {
            _playerSpriteRenderer.flipX = false;
            colOffset = Vector2.right;
        }
        colOffset = _playerBoxCollider2D.offset;


        if (_isDashing) {
            _dashT -= Time.deltaTime;
            return;
        } else if (_dashT < 0f) {
            _dashCooldownT -= Time.deltaTime;
        }

        if (_dashCooldownT < 0f) {
            _dashCooldownT = _dashCooldown;
            _dashT = _dashTimeLimit;
            _canDash = true;
        }

        _isGrounded = _PSM.IsGrounded();

        if (_isGrounded) {
            _coyoteT = _coyoteLimit;
        } else {
            _coyoteT -= Time.deltaTime;    
        }

        if (Input.GetButtonDown("Jump"))
        {
            _jumpPending = true;
            _jumpBufferT = _jumpBuffer;
        }

        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y - 10f);
            _coyoteT = 0f;
        }

        if (_jumpPending) {
            ExecuteJump(_PSM);
        }

    }

    public override void FixedUpdateState(PlayerStateManager _PSM)
    {
        if (_isDashing) {
            Dash();
            return;
        }

        _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);
    }

    public override void LateUpdateState(PlayerStateManager _PSM) {
        if (colOffset.x > 0f && toTheLeft) {
            colOffset.x *= -1f;
        }  else if (colOffset.x < 0f && !toTheLeft) {
            colOffset.x *= -1f;
        }

        _playerBoxCollider2D.offset = colOffset;
    }

    private void ExecuteJump(PlayerStateManager _PSM) {
        if (_coyoteT > 0f) {
            _rb.velocity = new Vector2(_rb.velocity.x, _PSM.jumpingPower);
            _isJumping = true;
            _jumpPending = false;
        } else {
            _jumpBufferT -= Time.deltaTime;
            if (_jumpBufferT < 0f) {
                _jumpPending = false;
            }
        }
    }

    private void Dash() {
        if (_dashT > 0f) {
            _rb.velocity = _dashDir * _dashingPower;
            return;
        } else {
            _isDashing = false;
        }
    }


    string GetAnimState() {
        if (_lockState) {
            LockState();
            return _currentState;
        }

        if (_isDashing) {
            _animLockT = _animator.GetCurrentAnimatorStateInfo(0).length;
            LockState();
            return "Player_Dash";
        }   


        if (_isJumping) {
            return JumpAnimate();
        }   

        if (!_isGrounded) {
            _isJumping = true;
            _currentJumpState = 2;
        }
        if (_horizontal != 0f && _isGrounded) return "Player_Move";

        return "Player_Idle";
    }

    string JumpAnimate() {
        if (_currentJumpState == 1) {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                return "Player_Jump";
            }
            _currentJumpState++;
        }
        if (_currentJumpState == 2) {
            if (ToLand()) {
                _currentJumpState = 1;
                _isJumping = false;
                return "Player_JumpLand";
            }
        }
            return "Player_JumpPeak";
    }

    bool ToLand() {
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down, 3f, _groundLayer); 
        if (hit.collider != null) {
            return true;
        }
        return false;
    }

    void LockState() {
        _animLockT -= Time.deltaTime;

        if (_animLockT < 0f) {
            _lockState = true;
            return;
        }

        _lockState = false;
    }
}
