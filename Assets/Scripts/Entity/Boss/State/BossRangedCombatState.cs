using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Combat;

namespace Entity.Boss.State
{
    public class BossRangedCombatState : BaseEnemyCombatState
    {
        BossController controller;
        BossCombat combat;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {            
            controller = animator.GetComponent<BossController>();
            combat = animator.GetComponent<BossCombat>();
            agent = animator.GetComponent<NavMeshAgent>();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CombatUtil.FaceTarget(controller.transform, controller.Target.transform.position);
            
            // Is the target close enough that the boss should transition to the melee state?
            if (CombatUtil.InRange(controller.Target.transform.position, agent.transform.position,
                combat.MeleeAttackRadius))
            {
                //Debug.Log("Boss going into melee state");

                controller.SetState(BossController.BossState.MeleeCombat);
            }
            // Not in melee range, is the boss within ranged attacking range?
            else if(CombatUtil.InRange(controller.Target.transform.position, agent.transform.position,
               combat.RangedAttackRadius))
            {
                // If so, proceed with combat
                SimulateCombat(controller, combat);
            }
            else
            {
                //Debug.Log("Boss going into chase state");

                controller.SetState(BossController.BossState.Chase);
            }
        }
    }
}

