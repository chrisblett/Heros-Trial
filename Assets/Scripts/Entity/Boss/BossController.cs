using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Player;

namespace Entity.Boss
{
    public class BossController : BaseEntityController
    {       
        // The range in which the boss can see the player if within the field of view
        [field: SerializeField] public float LookRadius { get; private set; }

        Animator animator;
        public enum BossState
        {
            Idle,         // 0 
            Chase,        // 1
            RangedCombat, // 2
            MeleeCombat,  // 3
            Dead          // 4
        }
               
        [field: SerializeField] public BossState State { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            animator = GetComponent<Animator>();
            State = BossState.Idle;
        }        
 
        public void SetState(BossState newState)
        {
            State = newState;
            animator.SetInteger("State", (int)State);
        }

        public override void OnInteract()
        {
            Debug.Log("Something is interacting with the boss");

            PlayerCombat pc = Target.GetComponent<PlayerCombat>();
            if (!pc.InCombat)
            {
                pc.OnCombatStart();
            }
        }

        public override string GetName()
        {
            return "Grand Necromancer";
        }
    }
}

