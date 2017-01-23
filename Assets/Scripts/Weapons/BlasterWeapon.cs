using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Weapons
{

    public class BlasterWeapon : Weapon, IWeapon
    {
        public GameObject BlasterPellet;
        public GameObject Player;
        public CharControl PlayerCtrl;

        public float PelletVelocity;
        private float _fireCooldown;
        private float _nextFire;

        public void Initialize()
        {
            _fireCooldown = 0.2f;
            _nextFire = 0.0f;

            PelletVelocity = 50f;

            BlasterPellet = Resources.Load("Ammo/BlasterPellet") as GameObject;
            
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerCtrl = Player.GetComponent<CharControl>();
        }

        public void Fire()
        {
            if (Time.time < _nextFire) return;
            var pellet = Object.Instantiate(BlasterPellet);
            pellet.transform.position = Player.transform.position;
            pellet.GetComponent<PelletBehaviour>().PelletVelocity = PlayerCtrl.MoveDir*PelletVelocity;
            _nextFire = Time.time + _fireCooldown;
        }
        
        public void Reload()
        {
        
        }
    }
}