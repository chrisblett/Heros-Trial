using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Stats
{
    public class BossStats : EntityCombatStats
    {
        public static BossStats Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public float rangedAttackRadius;

        [Header("Projectile")]
        public float projectileSpeed;
        public float projectileLifetime;
        public float projectileDamage;
    }
}


