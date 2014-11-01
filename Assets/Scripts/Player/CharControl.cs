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
        public float JumpForce;
        public bool AllowJump;
        public bool Jumping;
        public GameObject PowerUpDirection;
        public Vector3 Velocity;

        private float _currentJumpForce;
        private float _boostForce;
        private bool _allowWallJump;
        private bool _wallJumpOccured;

        private void Start()
        {
            AllowControl = true;

            AllowUserChangeDir = true;
            MoveDir = 1; // = right, -1 = left

            JumpForce = 8f; // Jump() specific.
            _currentJumpForce = JumpForce; // ^
            _boostForce = 2.25f; // ^

            AllowChangeDirection = true;
            AllowJump = true;
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

        public void HandleJumpPowerups()
        {
            var jumpPowerup = PowerUpManager.ActivePowerups.OfType<IJumpPowerup>().FirstOrDefault();

            if (jumpPowerup != null)
            {
                jumpPowerup.HandleJump(this);
            }
            else
            {
                HandleJump();
            }
        }

        private void HandleDirection()
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
            if (Input.GetAxis("Jump") == 1 && !Jumping && IsGrounded && AllowJump)
            {
                Jumping = true;
                AllowJump = false;
            }
            else if (Input.GetAxis("Jump") != 1 && !Jumping && IsGrounded && !AllowJump)
            {
                AllowJump = true;
            }

            if (Jumping) Jump();
        }

        private void HandleVelocity()
        {
            rigidbody.velocity += new Vector3(MoveDir*Acceleration, 0, 0);
                // _moveDir is the sign whether to accelerate to the right or left.

            Velocity = rigidbody.velocity;
            Velocity.x = Mathf.Clamp(Velocity.x, -MaxSpeed, MaxSpeed);
            rigidbody.velocity = Velocity;
        }

        public void Jump()
        {
            if ((Input.GetAxis("Jump") == 0 || _currentJumpForce <= 1.2f) && Jumping)
            {
                Jumping = false;
                _currentJumpForce = JumpForce;
                Debug.Log("Jump Ended");
                return;
            }
            rigidbody.velocity += new Vector3(0, _currentJumpForce, 0);
            _currentJumpForce -= JumpForce/6.66f; // TODO: Make this not shit.
        }

        public void ChangeDirection()
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
                        Jumping = false;
                        _currentJumpForce = JumpForce; //reset jump to prevent otherwise possible walljump+jump overlap, which results in an undesired super-jump

                        rigidbody.velocity += new Vector3(0, JumpForce*_boostForce, 0);
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