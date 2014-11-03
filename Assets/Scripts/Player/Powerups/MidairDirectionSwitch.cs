using UnityEngine;

namespace Assets.Scripts.Player.Powerups
{
    public class MidairDirectionPowerup : Powerup, IDirectionPowerup //TODO: Probably want this to only allow one direction change in air
    {
        private CharControl _charctrl;
        public void HandleDirection(CharControl charctrl)
        {
            _charctrl = charctrl;
            // Determine if the direction may be changed (if player is grounded).
            if (Input.GetAxis("Fire1") == 1 && _charctrl.AllowUserChangeDir)
            {
                _charctrl.ChangeDirection();
                _charctrl.AllowUserChangeDir = false;
            }
            else if (!_charctrl.AllowUserChangeDir && Input.GetAxis("Fire1") != 1)
            {
                _charctrl.AllowUserChangeDir = true;
            }
        }
    }
}