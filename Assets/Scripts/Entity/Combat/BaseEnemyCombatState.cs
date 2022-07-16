using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity;
using Entity.Combat;

public class BaseEnemyCombatState : StateMachineBehaviour
{
    // The reference the an enemy NavMeshAgent
    protected NavMeshAgent agent;

    protected void SimulateCombat(BaseEntityController controller, BaseCombat combat)
    {        
        if (CombatUtil.IsFacingTarget(agent.gameObject.transform, controller.Target.transform.position))
        {
            if (combat.TimeUntilNextPossibleAttack <= 0.0f)
            {				
                combat.OnAttack();
            }
        }
    }
}
