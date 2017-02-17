using UnityEngine;

namespace Assets.Scripts.Player.Powerups
{
    public class MidairDirectionPowerup : Powerup, IDirectionPowerup //TODO: Probably want this to only allow one direction change in air
    {
        private PlayerController _playerCtrlr;
        public void HandleDirection(PlayerController charctrl)
        {
            _playerCtrlr = charctrl;
            // Determine if the direction may be changed (if player is grounded).
            if (_playerCtrlr.ChangeDirectionDesired && _playerCtrlr.AllowUserChangeDir)
            {
                _playerCtrlr.ChangeDirection();
                _playerCtrlr.AllowUserChangeDir = false;
            }
            else if (!_playerCtrlr.AllowUserChangeDir && !_playerCtrlr.ChangeDirectionDesired)
            {
                _playerCtrlr.AllowUserChangeDir = true;
            }
        }
    }
}