using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Stats;

namespace Entity.Enemy.State
{
    public class EnemyWanderState : StateMachineBehaviour
    {
        public NavMeshAgent agent;
        public EnemyMovement movementScript;
        public EnemyController controllerScript;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Entered wander state");

            agent = animator.GetComponent<NavMeshAgent>();
            movementScript = animator.GetComponent<EnemyMovement>();
            controllerScript = animator.GetComponent<EnemyController>();

            agent.isStopped = false;
            //agent.speed = movementScript.WanderSpeed;
            agent.speed = EnemyStats.Instance.wanderSpeed;
            agent.autoBraking = false;
            agent.stoppingDistance = 0.0f;

            movementScript.OnStartedWander();

            // Reset aggro flag
            EnemyCombat combat = animator.GetComponent<EnemyCombat>();
            if(combat.isAggroed)
            {
                combat.isAggroed = false;                
                GameController.Instance.OnEnemyLostPlayer(combat);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {           
            if (controllerScript.CanSeePlayer() || controllerScript.CanHearPlayer())
            {
                if(GameController.Instance.IsAllowed(GameController.Action.ChasePlayer))
                {
                    //Debug.Log("Going to chase state");

                    controllerScript.SetState(EnemyController.EnemyState.Chase);
                }           
            }  
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Exiting Wander State");
            agent.speed = EnemyStats.Instance.chaseSpeed;
            agent.autoBraking = true;
            agent.stoppingDistance = 2.0f;
        }
    }
}


