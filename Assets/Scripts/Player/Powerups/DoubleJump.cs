using Assets.Scripts.Player;
using Assets.Scripts.Player.Powerups;
using UnityEngine;

public class DoubleJump : Powerup, IJumpPowerup
{
    private CharControl _charctrl;
    private bool _allowJump = false;


    public void HandleJump(CharControl charctrl)
    {
        if (_charctrl == null) _charctrl = charctrl;

        if (_charctrl.JumpDesired && _charctrl.JumpCount < 1 && _allowJump) // 1 because the JumpCount becomes 2
        {
            _charctrl.Jumping = true;
            _charctrl.CurrentJumpForce = _charctrl.JumpForce;
            
            var curVel = _charctrl.GetComponent<Rigidbody>().velocity;
            curVel.y = 0f;
            _charctrl.GetComponent<Rigidbody>().velocity = curVel;

            _charctrl.JumpCount += 1;
        }

        if (_charctrl.Jumping) _charctrl.Jump();

        _allowJump = _charctrl.JumpDesired == false;
    }
}