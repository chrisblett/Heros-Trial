using UnityEngine;
using UnityEngine.AI;
using Entity.Player;
using Entity;
using Entity.Stats;

namespace Entity.Enemy
{
    public class EnemyController : BaseEntityController
    {
        // The visual object in the world that represents the NavMeshAgent's target position
        public GameObject targetMarker;

        [field: SerializeField] public EnemyState State { get; private set; }

        Animator animator;
        EnemyCombat combat;
        EnemyStats enemyStats;

        public enum EnemyState
        {
            Idle,   // 0
            Wander, // 1
            Chase,  // 2
            Combat, // 3
            Dead    // 4
        }

        protected override void Start()
        {
            base.Start();

            animator = GetComponent<Animator>();
            combat = GetComponent<EnemyCombat>();

            enemyStats = EnemyStats.Instance;

            // Begin in the wander state
            SetState(EnemyState.Wander);
        }

        public override void OnInteract()
        {
            //Debug.Log("Something is interacting with the enemy");

            PlayerCombat pc = Target.GetComponent<PlayerCombat>();
            if (!pc.InCombat)
            {
                pc.OnCombatStart();
            }
        }

        public override string GetName()
        {
            return "Reanimated Warrior";
        }

        public void SetState(EnemyState newState)
        {
            State = newState;
            animator.SetInteger("State", (int)State);
        }

        public bool CanSeePlayer()
        {
            float angle = GetAngleBetweenPlayer();

            //Debug.Log("Angle: " + angle + " <= " + enemyStats.fieldOfViewAngleDegrees / 2);

            return (angle <= enemyStats.fieldOfViewAngleDegrees / 2) && IsPlayerWithinLookRadius();
        }

        public bool CanHearPlayer()
        {
            return Vector3.Distance(transform.position, Target.transform.position) < enemyStats.listenRadius;
        }

        bool IsPlayerWithinLookRadius()
        {
            float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
            if (distanceToTarget > enemyStats.lookRadius)
            {
                /*
                Debug.Log("Target is out of range at " + distanceToTarget
                    + " units, look radius is " + enemyStats.lookRadius);
                */

                return false;
            }
            return true;
        }

        float GetAngleBetweenPlayer()
        {
            Vector3 toTarget = (Target.transform.position - transform.position).normalized;
            Vector3 forward = transform.forward.normalized;

            // Clamp the dot result as sometimes it can return a value slightly outside of the range of [-1, 1]
            // This results in a NaN value being produced by Mathf.Acos since the function only
            // accepts parameters in the range [-1, 1]
            float dotResult = Vector3.Dot(toTarget, forward);
            dotResult = Mathf.Clamp(dotResult, -1.0f, 1.0f);

            Debug.Assert(dotResult <= 1.0f);

            float angleBetweenPlayer = Mathf.Rad2Deg * Mathf.Acos(dotResult);
            return angleBetweenPlayer;
        }

        public bool WasRecentlyAttacked() { return combat.recentlyAttacked; }

        public GameObject GetTarget() { return combat.GetTarget(); }

        public float GetHealth() { return combat.Health; }

        public bool IsDead() { return combat.IsDead; }
    }
}


