using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Boss;

namespace Entity.Boss.State
{
    public class BossIdleState : StateMachineBehaviour
    {
        BossController controller;
        NavMeshAgent agent;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            controller = animator.GetComponent<BossController>();
            agent = animator.GetComponent<NavMeshAgent>();

            //Debug.Log("Boss entered idle state");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (BaseEntityController.IsTargetWithinRadius(agent.transform.position, controller.Target.transform.position,
                controller.LookRadius))
            {
                //Debug.Log("Target is within look radius!");

                controller.SetState(BossController.BossState.Chase);
            }
        }
    }
}


