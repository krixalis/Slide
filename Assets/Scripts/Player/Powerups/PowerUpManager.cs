using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.Powerups
{
    public static class PowerUpManager
    {
        public static List<Powerup> ActivePowerups = new List<Powerup>();
        
        public static void Evaluate(string pUpName)
        {
            switch (pUpName)
            {
                case "DoubleJumpPowerup":
                    var doubleJump = new DoubleJump();
                    ActivePowerups.Add(doubleJump);
                    break;
                case "MidairPowerup":
                    var midair = new MidairDirectionPowerup();
                    ActivePowerups.Add(midair);
                    break;
                default:
                    Debug.Log("Invalid powerup name");
                    break;
            }
        }
    }
    
    public class Powerup
    {
        //Flag
    }
}
