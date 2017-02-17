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
            
            JumpForce = 0.175f; // Jump() specific.
            CurrentJumpForce = JumpForce; // ^
            _wallJumpForce = 0.175f; // ^

            AllowChangeDirection = true;
            _allowWallJump = true;
            
            //weapon testing
            WeaponManager.Evaluate("Blaster");
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
        }
        
        // FixedUpdate is called once per tick.
        private void FixedUpdate()
        {
            //if (-MaxSpeed*0.9 > Velocity.x || Velocity.x > MaxSpeed*0.9) AllowChangeDirection = true; //Buffer for how quickly a user can switch directions. This also prevents them from being able to get stuck on walls. TODO: This should probably instead be prevented by implementing a timed buffer where direction may not be changed?
            AllowChangeDirection = true;
            if (AllowControl == false)
            {
                _movCtrl.Velocity = Vector3.zero;
                return;
            }

            HandleDirectionPowerups();
            HandleJumpPowerups();

            HandleWallJump();
            HandleMovement(); //TODO: Handle Powerups
            _movCtrl.ApplyGravity = !_charCtrlr.isGrounded;
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
            if (_charCtrlr.isGrounded && JumpCount != 0)
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
            if (IsJumpDesired && _charCtrlr.isGrounded)
            {
                IsJumping = true;
            }

            if (IsJumping) Jump();
        }


        public void Jump()
        {
            if ((!IsJumpDesired || CurrentJumpForce <= 0.00001f) && IsJumping)
            {
                IsJumping = false;
                CurrentJumpForce = JumpForce;
                return;
            }
            //_rigidbody.velocity += new Vector3(0, CurrentJumpForce, 0);
            _movCtrl.AccelerateBy(new Vector3(0, CurrentJumpForce, 0));
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
            if (ChangeDirectionDesired && AllowUserChangeDir && _charCtrlr.isGrounded)
            {
                ChangeDirection();
                AllowUserChangeDir = false;
            }
            else if (!AllowUserChangeDir && _charCtrlr.isGrounded && !ChangeDirectionDesired)
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
        #endregion
        
        public bool IsFalling()
        {
            return Velocity.y < 0f;
        }

        #region Collisions

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            
            if (hit.normal.x < -0.9 && MoveDir == 1) //Comparing the normal to 0.9 due to slight imprecision
            {
                ChangeDirection();
            }
            else if (hit.normal.x > 0.9 && MoveDir == -1)
            {
                ChangeDirection();
                
            }
        }
        #endregion
    }
}