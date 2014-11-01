using Assets.Scripts.Player;
using Assets.Scripts.Player.Powerups;
using UnityEngine;

public class DoubleJump : Powerup, IJumpPowerup
{
    private CharControl _charctrl;
    public void HandleJump(CharControl charctrl)
    {
        // Determine if the player may jump (or is already jumping/isn't grounded).
        if (Input.GetAxis("Jump") == 1 && !_charctrl.Jumping && _charctrl.IsGrounded && _charctrl.AllowJump)
        {
            _charctrl.Jumping = true;
            _charctrl.AllowJump = false;
        }
        else if (Input.GetAxis("Jump") != 1 && !_charctrl.Jumping && _charctrl.IsGrounded && !_charctrl.AllowJump)
        {
            _charctrl.AllowJump = true;
        }

        if (_charctrl.Jumping) _charctrl.Jump();
    }
}