using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    
    public class WeaponManager : MonoBehaviour
    {
        public static List<Weapon> ActiveWeapons= new List<Weapon>();
        public static List<Weapon> ActiveLeftWeapons = new List<Weapon>(); //3? active 
        public static List<Weapon> ActiveRightWeapons = new List<Weapon>();
        
        private static IWeapon _leftWeapon;
        private static IWeapon _rightWeapon;
        
        public static void AddWeapon(string pWeaponName)
        {
            switch (pWeaponName)
            {
                case "Blaster":
                    var blaster = new BlasterWeapon();
                    ActiveWeapons.Add(blaster);
                    break;
                default:
                    Debug.Log("Invalid weapon name");
                    break;
            }
        }

        void FixedUpdate()
        {
            //put this here to aim weapons later down the line
        }
        
        public static void FireLeftWeapon()
        {
            _leftWeapon = ActiveLeftWeapons.OfType<IWeapon>().FirstOrDefault();

            if (_leftWeapon != null)
            {
                _leftWeapon.Fire();
            }
        }

        public static void ReloadLeftWeapon()
        {
            _leftWeapon = ActiveLeftWeapons.OfType<IWeapon>().FirstOrDefault();

            if (_leftWeapon != null)
            {
                _leftWeapon.Reload();
            }
        }

        public static void FireRightWeapon()
        {
            _rightWeapon = ActiveRightWeapons.OfType<IWeapon>().FirstOrDefault();

            if (_rightWeapon != null)
            {
                _rightWeapon.Fire();
            }
        }

        public static void ReloadRightWeapon()
        {
            _rightWeapon = ActiveRightWeapons.OfType<IWeapon>().FirstOrDefault();

            if (_rightWeapon != null)
            {
                _rightWeapon.Reload();
            }
        }
    }
    

    public class Weapon
    {
        //Flag
    }
}