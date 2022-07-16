using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Stats
{
    // Stores the general stats that an entity will likely have
    public class EntityCombatStats : MonoBehaviour
    {
        [Header("Combat")]

        // The maximum health an entity can have
        public float maximumHealth;

        // The damage dealt by their weapon
        public float weaponDamage;

        // How long the entity must wait between each attack
        public float attackCooldown;

        // The range in which the entity can perform an attack
        public float attackRadius;

        // How quick an entity's health regenerates every second
        public float healthRegenerationRate;
    }
}


