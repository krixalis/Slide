using UnityEngine;

public class CharControl : MonoBehaviour
{
    private CharacterController _controller;

    public float Acceleration;
    public float MaxSpeed;

    private float _moveDir;
    private float _jumpForce;
    private float _currentJumpForce;

    private bool _isGrounded;
    private bool _allowChangeDir;
    private bool _jumping;

    public Vector3 Velocity; 

    private void Start()
    {
        Acceleration = 8f;
        MaxSpeed = 12f;

        _allowChangeDir = true;
        _moveDir = 1; // = right, -1 = left

        _jumpForce = 12f; // Jump() specific.
        _currentJumpForce = _jumpForce; // ^
    }

    // FixedUpdate is called once per tick.
    private void FixedUpdate()
    {
        _isGrounded = IsGrounded();

        // Determine if the direction may be changed (if player is grounded).
        if (Input.GetAxis("Fire1") == 1 && _allowChangeDir && _isGrounded)
        {
            _allowChangeDir = false;
            _moveDir -= _moveDir * 2; // direction switch
        }
        else if (Input.GetAxis("Fire1") != 1)
        {
            _allowChangeDir = true;
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
        if (Input.GetAxis("Jump") == 0 || _currentJumpForce <= 2f)
        {
            _jumping = false;
            _currentJumpForce = _jumpForce;
            return;
        }
        rigidbody.velocity += new Vector3(0, _currentJumpForce, 0);
        //_currentJumpForce /= 1.66f;
        _currentJumpForce -= _jumpForce/2f; // TODO: Make this not shit.
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

    private void OnCollisionEnter(Collision collisionInfo) // TODO: When moving toward the edge on a flat surface right after landing on it, the player will switch direction. Why is that?
    {
        foreach (ContactPoint contact in collisionInfo.contacts) // TODO: And when the player decides to switch direction while he bounces off a wall, he will get stuck. Make function to prevent overlap?
        {
            if (contact.normal.x != 0 && contact.normal != Vector3.up)
            {
                _moveDir -= _moveDir * 2; // direction switch
                if (contact.thisCollider.rigidbody.velocity.y >= .2f)
                {
                    rigidbody.velocity += new Vector3(0, _jumpForce * 1.75f, 0);
                    Debug.Log("jumping");
                }
                break;
            }
        }
    }

    private bool IsGrounded()
    {
        return Velocity.y == 0f; // I should probably find a way to get rid of this, seems pretty silly.
    }
}