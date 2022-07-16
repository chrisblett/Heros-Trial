using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Entity.Combat;
using Entity.Player;
using Entity.Stats;

namespace Entity.Item
{
    public class HealthPotion : IConsumable
    {
        public void Consume()
        {            
            // Get the PlayerStats Singleton
            PlayerStats instance = PlayerStats.Instance;

            // Get a reference to the player's combat component
            PlayerCombat playerCombat = GameController.Instance.Player.GetComponent<PlayerCombat>();
       
            // Set the new health
            playerCombat.SetHealth(instance.maximumHealth);

            //Debug.Log("Effect applied!");
        }
    }
}
