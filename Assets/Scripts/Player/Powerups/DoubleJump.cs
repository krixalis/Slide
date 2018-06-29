using Assets.Scripts.Player;
using Assets.Scripts.Player.Powerups;
using UnityEngine;

public class DoubleJump : Powerup, IJumpPowerup
{
    private PlayerController _charctrl;


    public void HandleJump(PlayerController charctrl)
    {      
        if (_charctrl == null) _charctrl = charctrl;

        if (_charctrl.StickyGrounded) _charctrl.JumpCount = 0;

        if (_charctrl.IsJumpDesired && _charctrl.JumpCount < 2 && _charctrl.AllowJump)
        {
            if (_charctrl.CurrentJumpForce == 0f) _charctrl.CurrentJumpForce = _charctrl.JumpForce;

            _charctrl.StickyGrounded = false;
            _charctrl.IsJumping = true;

            Debug.Log(_charctrl.JumpCount);
            if (_charctrl.JumpCount == 1)
            {
                _charctrl.CancelYVelocity();
                _charctrl.CurrentJumpForce = _charctrl.JumpForce;
            }

            _charctrl.JumpCount += 1;            
        }

        if (_charctrl.IsJumping) _charctrl.Jump();
        else _charctrl.CurrentJumpForce = 0f;
    }
}