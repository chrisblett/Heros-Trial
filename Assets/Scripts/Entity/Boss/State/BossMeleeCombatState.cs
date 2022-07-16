using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Boss;
using Entity.Combat;
using Entity.Player;

public class BossMeleeCombatState : BaseEnemyCombatState
{
    BossController controller;
    BossCombat combat;
    PlayerMovement playerMovement;

    public float meleeAttackDist;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponent<BossController>();
        combat = animator.GetComponent<BossCombat>();
        playerMovement = controller.Target.GetComponent<PlayerMovement>();
        agent = animator.GetComponent<NavMeshAgent>();

        // TODO: Fix bug related to the boss moving whilst in the ranged state due to the stopping
        // distance changing back to the original value
        agent.stoppingDistance = meleeAttackDist;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CombatUtil.FaceTarget(controller.transform, controller.Target.transform.position);
   
        if (CombatUtil.InRange(controller.Target.transform.position, agent.transform.position,
                meleeAttackDist))
        {
            //Debug.Log("Melee attacking range!");

            SimulateCombat(controller, combat);            
        }      
        else if (CombatUtil.InRange(controller.Target.transform.position, agent.transform.position,
            combat.MeleeAttackRadius))
        {            
            //Debug.Log("Within attack radius but not attack range!");
            
            if (!playerMovement.IsMoving())
            {
                controller.SetDestination(controller.Target.transform.position);
            }             
        }
        else
        {            
            controller.SetState(BossController.BossState.RangedCombat);                       
        }        
    }
}