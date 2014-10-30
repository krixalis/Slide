using System;
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
                case "MidairPowerup":
                    MidairDirectionPowerup midair = new MidairDirectionPowerup();
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
