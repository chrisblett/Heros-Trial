using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Stats
{
    public class EnemyStats : EntityCombatStats
    {
        // This is a Singleton, all enemies share a reference to this object
        public static EnemyStats Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        [Header("Movement")]
        // The speed at which the enemy moves while in the 'Wander' state
        public float wanderSpeed;

        // The speed at which the enemy moves while in the 'Chase' state
        public float chaseSpeed;

        [Header("Sensing")]
        public float lookRadius;

        // The range in which an enemy can hear the player
        public float listenRadius;

        // How long the enemy will wait to transition from idle to wander
        public float idleWaitTime;

        // The area where the enemy can see
        public float fieldOfViewAngleDegrees;
    }
}