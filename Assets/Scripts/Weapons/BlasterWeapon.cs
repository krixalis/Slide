using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Weapons
{

    public class BlasterWeapon : Weapon, IWeapon
    {
        public GameObject BlasterPellet;
        public GameObject BlasterWeaponAttachment;

        public GameObject Player;
        public CharControl PlayerCtrl;

        
        private bool _isInitialized;

        public void Initialize()
        {
            _fireCooldown = 0.2f;
            _nextFire = 0.0f;

            _pelletVelocity = 50f;

            BlasterWeaponAttachment = Resources.Load("Weapons/Blaster") as GameObject;
            BlasterPellet = Resources.Load("Ammo/BlasterPellet") as GameObject;
            
            Player = GameObject.Find("Player");
            PlayerCtrl = Player.GetComponent<CharControl>();

            _isInitialized = true;
        }

        public void Aim()
        {
            //Not going to implement yet, need assets. Else this is a waste of time.
        }

        private float _pelletVelocity;
        private float _fireCooldown;
        private float _nextFire;

        public void Fire()
        {
            if(!_isInitialized) Initialize(); //feels hacky
            
            if (Time.time < _nextFire) return;
            
            var pellet = Object.Instantiate(BlasterPellet);
            pellet.transform.position = Player.transform.position;
            pellet.GetComponent<PelletBehaviour>().PelletVelocity = PlayerCtrl.MoveDir*_pelletVelocity;

            _nextFire = Time.time + _fireCooldown;
        }
        
        public void Reload()
        {
            //Not sure if this is even needed if I go through with initial design plan for weapons.
        }
    }
}