using UnityEngine;

namespace Assets.Scripts.Player.Powerups
{
    public class MidairDirectionPowerup : Powerup, IDirectionPowerup 
    {
        private CharControl _charctrl;
        public void HandleDirection(CharControl charctrl)
        {
            _charctrl = charctrl;
            // Determine if the direction may be changed (if player is grounded).
            if (Input.GetAxis("Fire1") == 1 && _charctrl.AllowUserChangeDir)
            {
                ChangeDirection();
                _charctrl.AllowUserChangeDir = false;
            }
            else if (!_charctrl.AllowUserChangeDir && Input.GetAxis("Fire1") != 1)
            {
                _charctrl.AllowUserChangeDir = true;
            }
        }

        private void ChangeDirection()
        {
            if (_charctrl.AllowChangeDirection)
            {
                _charctrl.MoveDir *= -1; // direction switch
                Debug.Log("User changed direction");
            }
            _charctrl.AllowUserChangeDir = false;
            _charctrl.AllowChangeDirection = false;
        }
    }
}
