using UnityEngine;
using UnityEngine.AI;
using Entity.Combat;
using Entity.Stats;

namespace Entity.Enemy.State
{
    public class EnemyCombatState : BaseEnemyCombatState
    {
        EnemyController controller;
        EnemyCombat combat;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            agent = animator.GetComponent<NavMeshAgent>();
            controller = animator.GetComponent<EnemyController>();
            combat = animator.GetComponent<EnemyCombat>();            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {            
            GameObject target = GameController.Instance.Player;

            if (GameController.Instance.IsPlayerDead())
            {
                controller.SetState(EnemyController.EnemyState.Idle);
            }
            else
            {                
                CombatUtil.FaceTarget(controller.transform, target.transform.position);

                if (CombatUtil.InRange(agent.transform.position, target.transform.position,
                    EnemyStats.Instance.attackRadius))
                {
                    SimulateCombat(controller, combat);
                }
                else
                {
                    // Reset the attack cooldown so the enemy doesn't wait if they catch
                    // up to the player before the cooldown is finished
                    combat.ResetNextPossibleAttackTime();

                    controller.SetState(EnemyController.EnemyState.Chase);
                }
            }           
        }
    }
}
