using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Audio;
using Entity.Stats;

namespace Entity.Enemy.State
{
    public class EnemyChaseState : StateMachineBehaviour
    {
        public NavMeshAgent agent;
        public EnemyMovement movementScript;
        public EnemyController controllerScript;

        IEnumerator regenHealthCoroutine;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            agent = animator.GetComponent<NavMeshAgent>();
            movementScript = animator.GetComponent<EnemyMovement>();
            controllerScript = animator.GetComponent<EnemyController>();

            agent.speed = EnemyStats.Instance.chaseSpeed;
            agent.autoBraking = true;
            agent.stoppingDistance = 2.0f;

           
            // If not aggroed, then set aggro flag          
            EnemyCombat combat = animator.GetComponent<EnemyCombat>();
            if(!combat.isAggroed)
            {
                combat.isAggroed = true;

                GameController.Instance.OnEnemyAggroed();
                animator.GetComponent<EnemyAudio>().PlayAggroSound();
                combat.StopCoroutine(combat.RegenHealthCoroutine);                
            }
            
            //Debug.Log("Enemy entered chase state");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Verifies if the enemy is allowed to perform an action before it actually performs it
            // This can be useful for scenarios where you may not want the enemy to perform actions
            // Such as when the player dies, during a cutscene or transition, etc...
            if (GameController.Instance.IsAllowed(GameController.Action.ChasePlayer))
            {                
                if (controllerScript.CanSeePlayer() || controllerScript.CanHearPlayer() ||
                    controllerScript.WasRecentlyAttacked())
                {
                    controllerScript.SetDestination();

                    if (controllerScript.ReachedTarget())
                    {
                        controllerScript.SetState(EnemyController.EnemyState.Combat);
                    }
                }
                else
                {
                    controllerScript.SetState(EnemyController.EnemyState.Idle);
                }
            }
            else
            {                
                controllerScript.SetState(EnemyController.EnemyState.Idle);
            }
        }
    }
}

