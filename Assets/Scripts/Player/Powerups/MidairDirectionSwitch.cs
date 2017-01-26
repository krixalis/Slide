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
            if (_charctrl.ChangeDirectionDesired && _charctrl.AllowUserChangeDir)
            {
                _charctrl.ChangeDirection();
                _charctrl.AllowUserChangeDir = false;
            }
            else if (!_charctrl.AllowUserChangeDir && !_charctrl.ChangeDirectionDesired)
            {
                _charctrl.AllowUserChangeDir = true;
            }
        }
    }
}