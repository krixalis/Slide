using Assets.Scripts.Player;
using Assets.Scripts.Player.Powerups;
using UnityEngine;

public class DoubleJump : Powerup, IJumpPowerup
{
    private PlayerController _charctrl;
    private bool _allowJump;


    public void HandleJump(PlayerController charctrl)
    {
        if (_charctrl == null) _charctrl = charctrl;

        if (_charctrl.IsJumpDesired && _charctrl.JumpCount < 1 && _allowJump) // 1 because the JumpCount becomes 2
        {
            _charctrl.IsJumping = true;
            _charctrl.CurrentJumpForce = _charctrl.JumpForce;
            
            var curVel = _charctrl.GetComponent<Rigidbody>().velocity;
            curVel.y = 0f;
            _charctrl.GetComponent<Rigidbody>().velocity = curVel;

            _charctrl.JumpCount += 1;
        }

        if (_charctrl.IsJumping) _charctrl.Jump();

        _allowJump = _charctrl.IsJumpDesired == false;
    }
}