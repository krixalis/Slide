using System.Linq;
using UnityEngine;
using Assets.Scripts.Player.Powerups;

namespace Assets.Scripts.Player
{
    public class CharControl : MonoBehaviour
    {
        private CharacterController _controller;

        public bool AllowControl;
        public float Acceleration;
        public float MaxSpeed;
        public float MoveDir;
        public bool IsGrounded;
        public bool AllowUserChangeDir;
        public bool AllowChangeDirection;
        public GameObject PowerUpDirection;
        public Vector3 Velocity;

        private float _jumpForce;
        private float _currentJumpForce;
        private float _boostForce;
        private bool _allowJump;
        private bool _allowWallJump;
        private bool _wallJumpOccured;
        private bool _jumping;

        private void Start()
        {
            AllowControl = true;

            AllowUserChangeDir = true;
            MoveDir = 1; // = right, -1 = left

            _jumpForce = 8f; // Jump() specific.
            _currentJumpForce = _jumpForce; // ^
            _boostForce = 2.25f; // ^

            AllowChangeDirection = true;
            _allowJump = true;
            _allowWallJump = true;
        }

        // FixedUpdate is called once per tick.
        private void FixedUpdate()
        {
            if (AllowControl == false) return;
            AllowChangeDirection = true;
            _wallJumpOccured = false;
            IsGrounded = OnGround();

            HandleDirectionPowerups();

            HandleJump(); //TODO: Handle Powerups

            HandleVelocity(); //TODO: Handle Powerups
        }

        public void HandleDirectionPowerups()
        {
            var directionPowerup = PowerUpManager.ActivePowerups.OfType<IDirectionPowerup>().FirstOrDefault();

            if (directionPowerup != null)
            {
                directionPowerup.HandleDirection(this);
            }
            else
            {
                HandleDirection();
            }
        }

        public void HandleDirection()
        {
            // Determine if the direction may be changed (if player is grounded).
            if (Input.GetAxis("Fire1") == 1 && AllowUserChangeDir && IsGrounded)
            {
                ChangeDirection();
                AllowUserChangeDir = false;
            }
            else if (!AllowUserChangeDir && IsGrounded && Input.GetAxis("Fire1") != 1)
            {
                AllowUserChangeDir = true;
            }
        }

        private void HandleJump()
        {
            // Determine if the player may jump (or is already jumping/isn't grounded).
            if (Input.GetAxis("Jump") == 1 && !_jumping && IsGrounded && _allowJump)
            {
                _jumping = true;
                _allowJump = false;
            }
            else if (Input.GetAxis("Jump") != 1 && !_jumping && IsGrounded && !_allowJump)
            {
                _allowJump = true;
            }

            if (_jumping) Jump();
        }

        private void HandleVelocity()
        {
            rigidbody.velocity += new Vector3(MoveDir*Acceleration, 0, 0);
                // _moveDir is the sign whether to accelerate to the right or left.

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
                Debug.Log("Jump Ended");
                return;
            }
            rigidbody.velocity += new Vector3(0, _currentJumpForce, 0);
            _currentJumpForce -= _jumpForce/6.66f; // TODO: Make this not shit.
        }

        private void ChangeDirection()
        {
            if (AllowChangeDirection)
            {
                MoveDir *= -1; // direction switch
                Debug.Log("User changed direction");
            }
            AllowUserChangeDir = false;
            AllowChangeDirection = false;
        }

        private bool OnGround()
        {
            return Velocity.y == 0f; // I should probably find a way to get rid of this, seems pretty silly.
        }

        private bool IsFalling()
        {
            return Velocity.y < 0f;
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            Velocity = rigidbody.velocity;
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                if (contact.normal == Vector3.up)
                {
                    Velocity.y = 0;
                    IsGrounded = true;
                    break;
                }
                IsGrounded = false;
            }
        }

        private void OnCollisionEnter(Collision collisionInfo)
            // TODO: When moving toward the edge on a flat surface right after landing on it, the player will switch direction. Why is that? +++ Not sure if this is still true, hard to replicate
        {
            if (IsFalling())
            {
                _allowWallJump = false; // Only allow the player to walljump if he hasn't started falling yet.
            }
            else if (!_wallJumpOccured && !IsFalling())
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
                        rigidbody.velocity += new Vector3(0, _jumpForce*_boostForce, 0);
                        _allowWallJump = false;
                        _wallJumpOccured = true;
                        Debug.Log("wall-jumping");
                    }
                    break;
                }
            }
        }
    }
}