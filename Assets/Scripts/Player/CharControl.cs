using System;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Player.Powerups;
using Assets.Scripts.Weapons;

namespace Assets.Scripts.Player
{
    public class CharControl : MonoBehaviour
    {
        public bool AllowControl;
        public Vector3 Velocity;

        private Rigidbody _rigidbody;
        //weapon testing
        private IWeapon _blasterTest;
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            IsJumpDesired = false;
            JumpCount = 0;
            AllowControl = true;

            AllowUserChangeDir = true;
            MoveDir = 1; // = right, -1 = left
            
            JumpForce = 7f; // Jump() specific.
            CurrentJumpForce = JumpForce; // ^
            _wallJumpForce = 7f; // ^

            AllowChangeDirection = true;
            _allowWallJump = true;
            
            //weapon testing
            WeaponManager.Evaluate("Blaster");
            _blasterTest = WeaponManager.ActiveWeapons.OfType<IWeapon>().FirstOrDefault();
        }

        private void Update()
        {
            //weapon testing
            if(Input.GetButton("Fire2")) _blasterTest.Fire();

            //This effectively queues up a jump.
            IsJumpDesired = Input.GetButton("Jump");

            ChangeDirectionDesired = Input.GetButton("Fire1");
        }

        // FixedUpdate is called once per tick.
        private void FixedUpdate()
        {
            

            if (-MaxSpeed*0.9 > Velocity.x || Velocity.x > MaxSpeed*0.9) AllowChangeDirection = true; //Buffer for how quickly a user can switch directions. This also prevents them from being able to get stuck on walls. TODO: This should probably instead be prevented by implementing a timed buffer where direction may not be changed?

            if (AllowControl == false)
            {
                _rigidbody.velocity = new Vector3(0, 0, 0);
                return;
            }

            HandleDirectionPowerups();
            HandleJumpPowerups();

            HandleWallJump();
            HandleMovement(); //TODO: Handle Powerups
            IsGrounded = false; //reset IsGrounded
        }



        #region Jump

        public int JumpCount;
        public float JumpForce;
        public float CurrentJumpForce;
        public bool IsJumping;
        public bool IsJumpDesired;

        private IJumpPowerup _jumpPowerup;

        public void HandleJumpPowerups()
        {
            if (IsGrounded && JumpCount != 0)
            {
                JumpCount = 0;
            }

            _jumpPowerup = PowerUpManager.ActivePowerups.OfType<IJumpPowerup>().FirstOrDefault();
            
            if (_jumpPowerup != null)
            {
                _jumpPowerup.HandleJump(this);
            }
            else
            {
                HandleJump();
            }
        }

        private void HandleJump()
        {
            // Determine if the player may jump (or is already jumping/isn't grounded).
            if (IsJumpDesired && IsGrounded)
            {
                IsJumping = true;
            }

            if (IsJumping) Jump();
        }


        public void Jump()
        {
            if ((!IsJumpDesired || CurrentJumpForce <= 0.6f) && IsJumping)
            {
                IsJumping = false;
                CurrentJumpForce = JumpForce;
                return;
            }
            _rigidbody.velocity += new Vector3(0, CurrentJumpForce, 0);
            CurrentJumpForce -= JumpForce * 0.12f; // TODO: Make this not shit.
        }
        #endregion

        #region WallJump

        private float _wallJumpForce;
        private float _currentWallJumpForce;

        private bool _isWallJumpDesired;
        private bool _isWallJumping;
        private bool _allowWallJump;

        private void HandleWallJump()
        {
            if (!_isWallJumpDesired) return;
            if(!_isWallJumping) _rigidbody.velocity = Vector3.zero;
            _isWallJumping = true;
            IsJumping = false;

            if(_isWallJumping) WallJump();
        }

        private void WallJump()
        {
            if (_currentWallJumpForce <= 0.6 && _isWallJumping)
            {
                _isWallJumping = false;
                _isWallJumpDesired = false;
                _currentWallJumpForce = _wallJumpForce;
                return;
            }
            _rigidbody.velocity += new Vector3(0, _currentWallJumpForce, 0);
            _currentWallJumpForce -= _wallJumpForce * 0.12f; // TODO: Make this not shit.
        }
        #endregion

        #region Direction

        public bool AllowUserChangeDir;
        public bool AllowChangeDirection;
        public bool ChangeDirectionDesired;

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

        private void HandleDirection()
        {
            // Determine if the direction may be changed (if player is grounded).
            if (ChangeDirectionDesired && AllowUserChangeDir && IsGrounded)
            {
                ChangeDirection();
                AllowUserChangeDir = false;
            }
            else if (!AllowUserChangeDir && IsGrounded && !ChangeDirectionDesired)
            {
                AllowUserChangeDir = true;
            }
        }

        public void ChangeDirection()
        {
            if (AllowChangeDirection)
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0); //makes it much snapper
                MoveDir *= -1; // direction switch
            }
            AllowUserChangeDir = false;
            AllowChangeDirection = false;
        }
        #endregion

        #region Movement
        public float Acceleration;
        public float MaxSpeed;
        public int MoveDir;

        private void HandleMovement()
        {
            _rigidbody.velocity += new Vector3(MoveDir*Acceleration, 0, 0);
            // _moveDir is the sign whether to accelerate to the right or left.

            Velocity = _rigidbody.velocity;
            Velocity.x = Mathf.Clamp(Velocity.x, -MaxSpeed, MaxSpeed);
            _rigidbody.velocity = Velocity;
        }
        #endregion
        
        public bool IsFalling()
        {
            return Velocity.y < 0f;
        }

        #region Collisions

        public bool IsGrounded;
        private bool _playerGroundedWasDetermined;

        private void OnCollisionStay(Collision collisionInfo)
        {
            Velocity = _rigidbody.velocity;
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                if (0.75f <= contact.normal.y && contact.normal.y <= 1.25f) //boilerplate code to support slopes
                {
                    IsGrounded = true;
                    _playerGroundedWasDetermined = true;
                    break;
                }
                if (!_playerGroundedWasDetermined)IsGrounded = false;
            }
        }

       
        void OnCollisionEnter(Collision collisionInfo)
            // TODO: When moving toward the edge on a flat surface right after landing on it, the player will switch direction. Why is that? +++ Not sure if this is still true, hard to replicate
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                float normalX = Math.Abs(contact.normal.x);
                float normalY = Math.Abs(contact.normal.y);
                float des = 1.0f;


                if (normalX - des <= 0.1f && normalY - des <= -0.25f)
                    //(contact.normal.x != 0 && contact.normal != Vector3.up && contact.normal != Vector3.down)
                {
                    ChangeDirection();
                    AllowChangeDirection = false; //This prevents the player from getting stuck on walls.

                    if (!IsGrounded)_isWallJumpDesired = true;
                    break;
                }
            }
        }
        #endregion
    }
}