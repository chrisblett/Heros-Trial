using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Combat;

namespace Entity.Boss.State
{
    public class BossChaseState : StateMachineBehaviour
    {
        BossController controller;
        BossCombat combat;
        NavMeshAgent agent;

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
            if(CombatUtil.InRange(controller.Target.transform.position, agent.transform.position,
                combat.RangedAttackRadius))
            {
                //Debug.Log("Boss in ranged attacking range");
                controller.SetState(BossController.BossState.RangedCombat);
            }
            else
            {
                controller.SetDestination(controller.Target.transform.position);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Stop moving towards the target, because the boss is now in ranged attacking range
            controller.SetDestination(agent.transform.position);
        }
    }
}


