using System.Security;
using System.Threading;
using UnityEngine;

public class CharControl : MonoBehaviour
{
    private CharacterController _controller;

    public float Acceleration;
    public float MaxSpeed;

    private float _moveDir;
    private float _jumpForce;
    private float _currentJumpForce;
    private float _boostForce;

    private bool _isGrounded;
    private bool _allowUserChangeDir;
    private bool _allowWallJump;
    private bool _jumping;

    private bool _allowChangeDirection;
    
    public Vector3 Velocity; 

    private void Start()
    {
        Acceleration = 8f;
        MaxSpeed = 12f;

        _allowUserChangeDir = true;
        _moveDir = 1; // = right, -1 = left

        _jumpForce = 8f; // Jump() specific.
        _currentJumpForce = _jumpForce; // ^
        _boostForce = 2.25f; // ^

        _allowChangeDirection = true;
        _allowWallJump = true;
    }

    // FixedUpdate is called once per tick.
    private void FixedUpdate()
    {
        _allowChangeDirection = true;
        _isGrounded = IsGrounded();

        // Determine if the direction may be changed (if player is grounded).
        if (Input.GetAxis("Fire1") == 1 && _allowUserChangeDir && _isGrounded)
        {
            ChangeDirection();
            _allowUserChangeDir = false;
        }
        else if (!_allowUserChangeDir && _isGrounded && Input.GetAxis("Fire1") != 1)
        {
            _allowUserChangeDir = true;
        }
        
        // Determine if the player may jump (or is already jumping/isn't grounded).
        if (Input.GetAxis("Jump") == 1 && !_jumping && _isGrounded)
        {
            _jumping = true;
        }

        if (_jumping) Jump();

        rigidbody.velocity += new Vector3(_moveDir*Acceleration, 0, 0); // _moveDir is the sign whether to accelerate to the right or left.

        Velocity = rigidbody.velocity;
        Velocity.x = Mathf.Clamp(Velocity.x, -MaxSpeed, MaxSpeed);
        rigidbody.velocity = Velocity;
    }

    private void Jump()
    {
        if (Input.GetAxis("Jump") == 0 || _currentJumpForce <= 1.2f)
        {
            _jumping = false;
            _currentJumpForce = _jumpForce;
            return;
        }
        rigidbody.velocity += new Vector3(0, _currentJumpForce, 0);
        //_currentJumpForce /= 1.66f;
        _currentJumpForce -= _jumpForce/6.66f; // TODO: Make this not shit.
    }

    private Vector3 _lPos;
    private Vector3 _rPos;


    private void OnCollisionStay(Collision collisionInfo)
    {
        Velocity = rigidbody.velocity;
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            if (contact.normal == Vector3.up)
            {
                Velocity.y = 0;
                _isGrounded = true;
                break;
            }
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collisionInfo) // TODO: When moving toward the edge on a flat surface right after landing on it, the player will switch direction. Why is that? +++ Not sure if this is still true, hard to replicate
    {
        if (IsFalling()) _allowWallJump = false; // Only allow the player to walljump if he hasn't started falling yet.
        else
        {
            _allowWallJump = true;
        }
        
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            if (contact.normal.x != 0 && contact.normal != Vector3.up && contact.normal != Vector3.down)
            {
                ChangeDirection();
                if (Input.GetAxis("Jump") == 1 && _allowWallJump)
                {
                    rigidbody.velocity += new Vector3(0, _jumpForce * _boostForce, 0);
                    Debug.Log("jumping");
                }
                break;
            }
        }
    }


    private void ChangeDirection()
    {
        if (_allowChangeDirection)
        {
            _moveDir *= -1; // direction switch
            Debug.Log("User changed direction");
        }
        _allowUserChangeDir = false;
        _allowChangeDirection = false;
    }


    private bool IsGrounded()
    {
        return Velocity.y == 0f; // I should probably find a way to get rid of this, seems pretty silly.
    }

    private bool IsFalling()
    {
        return Velocity.y < 0f;
    }
}