using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Stats;

namespace Entity.Enemy.State
{
    public class EnemyIdleState : StateMachineBehaviour
    {
        public NavMeshAgent agent;
        public EnemyMovement movementScript;
        public EnemyController controllerScript;

        public float timeToMove;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            agent = animator.GetComponent<NavMeshAgent>();
            movementScript = animator.GetComponent<EnemyMovement>();
            controllerScript = animator.GetComponent<EnemyController>();

            // Set the destination to itself because it is not moving anymore
            controllerScript.SetDestination(agent.transform.position);
            timeToMove = Time.time + EnemyStats.Instance.idleWaitTime;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("In idle state: " + animator.gameObject.name);

            if (agent.velocity == Vector3.zero)
            {
                //Debug.Log("Stopped in idle state");
                
                agent.isStopped = false;
            }

            if (Time.time >= timeToMove)
            {
                //Debug.Log("Waited in idle for " + controllerScript.idleWaitTime + " seconds, returning to patrol path");
                
                controllerScript.SetState(EnemyController.EnemyState.Wander);
            }

            if (controllerScript.CanSeePlayer() || controllerScript.CanHearPlayer())
            {
                if(GameController.Instance.IsAllowed(GameController.Action.ChasePlayer))
                {
                    controllerScript.SetState(EnemyController.EnemyState.Chase);
                }                
            }
        }
    }
}


