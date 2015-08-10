using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
        public int JumpCount;
        public float CurrentJumpForce;
        public bool JumpDesired;
        public GameObject PowerUpDirection;
        public Vector3 Velocity;

        private float _boostForce;
        private bool _allowWallJump;
        private bool _wallJumpOccured;

        private void Start()
        {
            JumpDesired = false;
            JumpCount = 0;
            AllowControl = true;

            AllowUserChangeDir = true;
            MoveDir = 1; // = right, -1 = left

            JumpForce = 8f; // Jump() specific.
            CurrentJumpForce = JumpForce; // ^
            _boostForce = 2.25f; // ^

            AllowChangeDirection = true;
            AllowJump = true;
            _allowWallJump = true;
        }

        private void Update()
        {

            if (!JumpDesired) JumpDesired = Input.GetButtonDown("Jump");
            //This effectively queues up a jump.
            //Using an Input-Axis would make things overly complicated.

            if (AllowControl == false)
            {
                transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                return;
            }
            AllowChangeDirection = true;
            _wallJumpOccured = false;

            HandleDirectionPowerups();

            HandleJumpPowerups();
            /*
            Debug.Log("Fire1: " + Input.GetButton("Fire1"));
            Debug.Log("Jump: " + Input.GetButton("Jump"));
            Debug.Log("isGrounded: " + IsGrounded);
             */
        }

        // FixedUpdate is called once per tick.
        private void FixedUpdate()
        {
            if (IsGrounded && JumpCount != 0)
            {
                JumpCount = 0;
            }


            HandleVelocity(); //TODO: Handle Powerups
            IsGrounded = false; //reset IsGrounded
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
            /*
            Debug.Log("Fire1: " + Input.GetAxis("Fire1") + 
                "AllowUserChangeDir: " + AllowUserChangeDir + 
                "isGrounded: " + IsGrounded);
             */

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
            GetComponent<Rigidbody>().velocity += new Vector3(MoveDir*Acceleration, 0, 0);
            // _moveDir is the sign whether to accelerate to the right or left.

            Velocity = GetComponent<Rigidbody>().velocity;
            Velocity.x = Mathf.Clamp(Velocity.x, -MaxSpeed, MaxSpeed);
            GetComponent<Rigidbody>().velocity = Velocity;
        }

        public void Jump()
        {
            if ((Input.GetAxis("Jump") == 0 || CurrentJumpForce <= 1.2f) && Jumping)
            {
                Jumping = false;
                CurrentJumpForce = JumpForce;
                return;
            }
            GetComponent<Rigidbody>().velocity += new Vector3(0, CurrentJumpForce, 0);
            CurrentJumpForce -= JumpForce/6.66f; // TODO: Make this not shit.
        }

        public void ChangeDirection()
        {
            if (AllowChangeDirection)
            {
                MoveDir *= -1; // direction switch
            }
            AllowUserChangeDir = false;
            AllowChangeDirection = false;
        }

        /*
        private bool OnGround()
        {
            return Velocity.y == 0f; // I should probably find a way to get rid of this, seems pretty silly.
        }
         */

        public bool IsFalling()
        {
            return Velocity.y < 0f;
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            Velocity = GetComponent<Rigidbody>().velocity;
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                float normalY = contact.normal.y;
                float desY = 1.0f;

                if (Mathf.Approximately(contact.normal.y, 1))
                {
                    IsGrounded = true;
                    break;
                }

                bool pls = contact.normal == Vector3.up;
                Debug.Log("isGrounded now false. " + "if: " + pls + ". contact.normal" + contact.normal +
                          " == Vector3.up" + Vector3.up);
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
                float normalX = Math.Abs(contact.normal.x);
                float normalY = Math.Abs(contact.normal.y);
                float des = 1.0f;


                if (normalX - des <= 0.1f && normalY - des <= -0.25f)
                    //(contact.normal.x != 0 && contact.normal != Vector3.up && contact.normal != Vector3.down)
                {
                    //Debug.Log("normalY: " + normalY);
                    ChangeDirection();
                    if (Input.GetAxis("Jump") == 1 && _allowWallJump)
                    {
                        Jumping = false;
                        CurrentJumpForce = JumpForce;
                            //reset jump to prevent otherwise possible walljump+jump overlap, which results in an undesired super-jump

                        GetComponent<Rigidbody>().velocity += new Vector3(0, JumpForce*_boostForce, 0);
                        _allowWallJump = false;
                        _wallJumpOccured = true;
                    }
                    break;
                }
            }
        }
    }
}