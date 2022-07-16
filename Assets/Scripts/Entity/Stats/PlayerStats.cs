using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Stats
{
    public class PlayerStats : EntityCombatStats
    {
        // This is a Singleton
        public static PlayerStats Instance { get; private set; }

        public float MovementSpeed { get { return movementSpeed; } }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            maximumHealth = 100;
        }

        [Header("Other")]
        [SerializeField] private float movementSpeed;
    }

}

