using System.Linq;
using UnityEngine;
using Assets.Scripts.Player.Powerups;
using Assets.Scripts.Weapons;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public bool AllowControl;
        public Vector3 Velocity;
        
        //weapon testing
        private IWeapon _blasterTest;
        private CharacterController _charCtrlr;

        private void Start()
        {
            _movCtrl = GetComponent<MovementController>();
            
            IsJumpDesired = false;
            JumpCount = 0;
            AllowControl = true;

            AllowUserChangeDir = true;
            MoveDir = 1; // = right, -1 = left
            //TODO: Get rid of magic numbers, or at least get rid of them being applied without context (i.e. turn into percentage or something)
            JumpForce = 0.175f; // Jump() specific.
            MaxAppliedJumpForce = 0.2f; // Another magic number
            CurrentJumpForce = 0f; // ^
            AllowJump = true;
            _wallJumpForce = 0.075f; // ^
            MaxWallJumpVel = 0.5f;
            WallJumpDeceleration = 0.12f;

            AllowChangeDirection = true;
            _allowWallJump = false;
            IsJumping = false;

            //weapon testing
            WeaponManager.AddWeapon("Blaster");
            _blasterTest = WeaponManager.ActiveWeapons.OfType<IWeapon>().FirstOrDefault();
            
            _charCtrlr = GetComponent<CharacterController>();
        }

        private void Update()
        {
            //weapon testing
            if(Input.GetButton("Fire2")) _blasterTest.Fire();

            //This effectively queues up a jump.
            IsJumpDesired = Input.GetButton("Jump");

            ChangeDirectionDesired = Input.GetButton("Fire1");
            GetComponent<Renderer>().material.color = StickyGrounded ? Color.white : Color.red;
        }

        public bool StickyGrounded;

        // FixedUpdate is called once per tick. TODO: Wait is that actually true though?
        private void FixedUpdate()
        {
            //if (-MaxSpeed*0.9 > Velocity.x || Velocity.x > MaxSpeed*0.9) AllowChangeDirection = true; 
            //Buffer for how quickly a user can switch directions. This also prevents them from being able to get stuck on walls. 
            //TODO: This should probably instead be prevented by implementing a timed buffer where direction may not be changed?
            AllowChangeDirection = true;
            if (AllowControl == false)
            {
                _movCtrl.Vel = Vector3.zero;
                return;
            }

            HandleDirectionPowerups();
            HandleJumpPowerups();

            HandleWallJump();            

            HandleMovement(); //TODO: Handle Powerups
            _movCtrl.ApplyGrav = !StickyGrounded;

            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, Vector3.down, out rayHit, 1.1f))
            {
                if (rayHit.normal == Vector3.up && _charCtrlr.isGrounded) StickyGrounded = true; //isGrounded shenanigans
            }
            else
            {
                StickyGrounded = false;
            }
        }

        #region Jump
        public int JumpCount;
        public float JumpForce;
        public float CurrentJumpForce;
        public float MaxAppliedJumpForce;
        public bool IsJumping;
        public bool IsJumpDesired;
        public bool AllowJump;

        private IJumpPowerup _jumpPowerup;

        public void HandleJumpPowerups()
        {
            if (StickyGrounded && JumpCount != 0)
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

            _movCtrl.AccelerateBy(new Vector3(0, CurrentJumpForce, 0));
            AllowJump = IsJumpDesired == false; // needs to be updated after the jump, because both this and IsJumpDesired need to be true at the same time.
        }

        private void HandleJump()
        {
            // Determine if the player may jump (or is already jumping/isn't grounded).
            if (IsJumpDesired && StickyGrounded && AllowJump)
            {
                CurrentJumpForce = JumpForce;
                StickyGrounded = false;
                IsJumping = true;
            }

            // CANNOT JUMP WITH STICKYGROUND
            if (IsJumping) Jump();
            else CurrentJumpForce = 0f;
        }


        public void Jump()
        {
            if ((!IsJumpDesired || CurrentJumpForce <= 0.00001f) && IsJumping)
            {              
                IsJumping = false;
                CurrentJumpForce = 0f;
                return;
            }

            Debug.Log(CurrentJumpForce);

            //_rigidbody.velocity += new Vector3(0, CurrentJumpForce, 0);

            //_movCtrl.AccelerateBy(new Vector3(0, CurrentJumpForce, 0));

            CurrentJumpForce -= JumpForce * 0.12f; // TODO: Make this not shit.
        }
        #endregion

        #region WallJump

        private float _wallJumpForce;
        public float _currentWallJumpForce;
        
        public bool _isWallJumping;
        public bool _allowWallJump;

        private void HandleWallJump()
        {
            if (IsJumpDesired && _allowWallJump)
            {
                _isWallJumping = true;
                _currentWallJumpForce = _wallJumpForce;
                IsJumping = false;
                _allowWallJump = false;
            }
            if (_currentWallJumpForce != 0) Debug.Log(_currentWallJumpForce);
            if (_isWallJumping) WallJump();
            else _currentWallJumpForce = 0f;
            
        }

        public float MaxWallJumpVel;
        public float WallJumpDeceleration;

        private void WallJump() //TODO: Make it so the upward boost amount resets after a WJ - - - Looks like it already does this, as it should. the problem was that when walls are too far apart the angle at which you hit a wall becomes too shallow (?) so you actually just don't walljump anymore, but the upward momentum still takes time to wind down.
        {
            if (_currentWallJumpForce <= 0.00001f && _isWallJumping)
            {
                _isWallJumping = false;
                _currentWallJumpForce = 0f;
                return;
            }

            if (_movCtrl.Vel.y + _currentWallJumpForce >= MaxWallJumpVel)
                _currentWallJumpForce = _movCtrl.Vel.y- MaxWallJumpVel; 

            else if (_movCtrl.Vel.y >= MaxWallJumpVel)
                _currentWallJumpForce = 0f;
            
            _movCtrl.AccelerateBy(new Vector3(0, _currentWallJumpForce, 0));

            _currentWallJumpForce -= _wallJumpForce * WallJumpDeceleration; 
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
            if (ChangeDirectionDesired && AllowUserChangeDir && StickyGrounded)
            {
                ChangeDirection();
                AllowUserChangeDir = false;
            }
            else if (!AllowUserChangeDir && StickyGrounded && !ChangeDirectionDesired)
            {
                AllowUserChangeDir = true;
            }
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
        #endregion

        #region Movement
        public float Acceleration;
        public float MaxSpeed;
        public int MoveDir;
        private MovementController _movCtrl;

        private void HandleMovement()
        {
            _movCtrl.AccelerateBy(new Vector3(MoveDir*Acceleration, 0, 0));
        }

        public void CancelYVelocity()
        {
            _movCtrl.Vel.y = 0f;
            CurrentJumpForce = 0f;
        }        
        
        public bool IsFalling()
        {
            return Velocity.y < 0f;
        }
        #endregion

        #region Collisions

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            
            if (hit.normal.x < -0.9 && MoveDir == 1) // Checking if the player ran into a wall (Comparing the normal to 0.9 due to slight imprecision)
            {
                ChangeDirection();
                if (!StickyGrounded && IsJumpDesired && _movCtrl.Vel.y > 0)
                {
                    _allowWallJump = true;
                }
            }
            else if (hit.normal.x > 0.9 && MoveDir == -1)
            {
                ChangeDirection();
                if (!StickyGrounded && IsJumpDesired && _movCtrl.Vel.y > 0)
                {
                    _allowWallJump = true;
                }

            }

            if (hit.normal.y < -0.9) // If the player bonks his head, do this
            {
                IsJumping = false;
                _isWallJumping = false;
                CancelYVelocity();
                _movCtrl.AccelerateBy(new Vector3(0, -0.05f, 0));
            }
        }
        #endregion
    }
}