using System;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Player.Powerups;
using Assets.Scripts.Weapons;

namespace Assets.Scripts.Player
{
    public class CharControl : MonoBehaviour
    {
        private CharacterController _controller;

        public bool AllowControl;

        public float Acceleration;
        public float MaxSpeed;
        public int MoveDir;
        public bool IsGrounded;

        public bool AllowUserChangeDir;
        public bool AllowChangeDirection;

        public float JumpForce;
        public bool AllowJump;
        public bool Jumping;
        public int JumpCount;
        public float CurrentJumpForce;
        public bool JumpDesired;
        public Vector3 Velocity;

        private float _boostForce;
        private bool _allowWallJump;
        private bool _wallJumpOccured;

        //weapon testing
        private IWeapon _blasterTest;
        private void Start()
        {
            JumpDesired = false;
            JumpCount = 0;
            AllowControl = true;

            AllowUserChangeDir = true;
            MoveDir = 1; // = right, -1 = left

            JumpForce = 7f; // Jump() specific.
            CurrentJumpForce = JumpForce; // ^
            _boostForce = 18f; // ^

            AllowChangeDirection = true;
            AllowJump = true;
            _allowWallJump = true;



            //weapon testing
            WeaponManager.Evaluate("Blaster");
            _blasterTest = WeaponManager.ActiveWeapons.OfType<IWeapon>().FirstOrDefault();
            _blasterTest.Initialize();
        }

        private void Update()
        {
            //placeholder
            if(Input.GetButton("Fire2")) _blasterTest.Fire();
        }

        // FixedUpdate is called once per tick.
        private void FixedUpdate()
        {
            JumpDesired = Input.GetButton("Jump");
            //This effectively queues up a jump.
            //Using an Input-Axis would make things overly complicated.

            if (-MaxSpeed*0.9 > Velocity.x || Velocity.x > MaxSpeed*0.9) AllowChangeDirection = true; //Buffer for how quickly a user can switch directions. This also prevents them from being able to get stuck on walls. TODO: This should probably instead be prevented by implementing a timed buffer where direction may not be changed?

            if (AllowControl == false)
            {
                transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                return;
            }
            _wallJumpOccured = false; //reset every tick (why tho?)

            HandleDirectionPowerups();
            HandleJumpPowerups();


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
            if (IsGrounded && JumpCount != 0)
            {
                JumpCount = 0;
            }

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
            if (JumpDesired && IsGrounded && AllowJump)
            {
                Jumping = true;
                AllowJump = false;
            }

            if (Jumping)
            {
                Jump();
                Debug.Log("Jumping");
            }
            if (!JumpDesired) AllowJump = true;
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
            if ((!JumpDesired || CurrentJumpForce <= 0.6f) && Jumping)
            {
                Jumping = false;
                CurrentJumpForce = JumpForce;
                return;
            }
            GetComponent<Rigidbody>().velocity += new Vector3(0, CurrentJumpForce, 0);
            CurrentJumpForce -= JumpForce*0.12f; // TODO: Make this not shit.
        }

        public void ChangeDirection()
        {
            if (AllowChangeDirection)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0); //makes it much snapper
                MoveDir *= -1; // direction switch
            }
            AllowUserChangeDir = false;
            AllowChangeDirection = false;
        }
        
        
        public bool IsFalling()
        {
            return Velocity.y < 0f;
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            Velocity = GetComponent<Rigidbody>().velocity;
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                if (0.75f <= contact.normal.y && contact.normal.y <= 1.25f) //boilerplate code to support slopes
                {
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
                float normalX = Math.Abs(contact.normal.x);
                float normalY = Math.Abs(contact.normal.y);
                float des = 1.0f;


                if (normalX - des <= 0.1f && normalY - des <= -0.25f)
                    //(contact.normal.x != 0 && contact.normal != Vector3.up && contact.normal != Vector3.down)
                {
                    ChangeDirection();
                    AllowChangeDirection = false;
                    if (JumpDesired && _allowWallJump)
                    {
                        Jumping = false;

                        GetComponent<Rigidbody>().velocity += new Vector3(0, _boostForce, 0);

                        _allowWallJump = false;
                        _wallJumpOccured = true;
                    }
                    break;
                }
            }
        }
    }
}