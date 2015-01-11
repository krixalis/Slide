using Assets.Scripts.Player;
using Assets.Scripts.Player.Powerups;
using UnityEngine;

public class DoubleJump : Powerup, IJumpPowerup
{
    private CharControl _charctrl;



    public void HandleJump(CharControl charctrl)
    {
        if (_charctrl == null) _charctrl = charctrl;

        if (_charctrl.JumpDesired && _charctrl.JumpCount < 1) // 1 because the JumpCount becomes 2
        {
            _charctrl.JumpDesired = false;
            _charctrl.Jumping = true;
            _charctrl.CurrentJumpForce = _charctrl.JumpForce;

            var curVel = _charctrl.rigidbody.velocity;
            curVel.y = 0f;
            _charctrl.rigidbody.velocity = curVel;

            _charctrl.JumpCount += 1;
        }

        if (charctrl.Jumping) Jump();
    }

    private void Jump()
    {
        if ((Input.GetAxis("Jump") == 0 || _charctrl.CurrentJumpForce <= 1.2f) && _charctrl.Jumping)
        {
            _charctrl.Jumping = false;
            _charctrl.CurrentJumpForce = _charctrl.JumpForce;
            return;
        }

        _charctrl.rigidbody.velocity += new Vector3(0, _charctrl.CurrentJumpForce, 0);
        _charctrl.CurrentJumpForce -= _charctrl.JumpForce / 6.66f; // TODO: Make this not shit.
    }
}