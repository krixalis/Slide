using Assets.Scripts.Player;
using Assets.Scripts.Player.Powerups;
using UnityEngine;

public class DoubleJump : Powerup, IJumpPowerup
{
    private CharControl _charctrl;



    public void HandleJump(CharControl charctrl)
    {
        if (_charctrl == null) _charctrl = charctrl;

        if (_charctrl.JumpDesired && _charctrl.JumpCount == 0) // 1 because the JumpCount becomes 2
        {
            _charctrl.JumpDesired = false;
            _charctrl.Jumping = true;
            _charctrl.CurrentJumpForce = _charctrl.JumpForce;

            var curVel = _charctrl.GetComponent<Rigidbody>().velocity;
            curVel.y = 0f;
            _charctrl.GetComponent<Rigidbody>().velocity = curVel;

            _charctrl.JumpCount += 1;
        }

        if (_charctrl.Jumping) Jump();
    }

    private void Jump()
    {
        if (_charctrl.CurrentJumpForce <= 1.2f && _charctrl.Jumping)
        {
            _charctrl.Jumping = false;
            _charctrl.CurrentJumpForce = _charctrl.JumpForce;
            return;
        }

        _charctrl.GetComponent<Rigidbody>().velocity += new Vector3(0, _charctrl.CurrentJumpForce, 0);
        _charctrl.CurrentJumpForce -= _charctrl.JumpForce / 6.66f; // TODO: Make this not shit.
    }
}