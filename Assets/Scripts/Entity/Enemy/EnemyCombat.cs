using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Entity.Combat;
using Entity.Enemy;
using Audio;
using EnemyState = Entity.Enemy.EnemyController.EnemyState;
using Entity.Stats;

namespace Entity.Enemy
{
    public class EnemyCombat : BaseCombat
    {
        [Header("Enemy Specific")]
        public bool recentlyAttacked;
        public bool isAggroed;

        // How long the enemy will stay focused on the player once initially aggroed   
        public float aggroTime;        

        // A reference to the enemy's Animator component
        Animator animator;        

        EnemyAudio enemyAudio;

        protected override void Start()
        {
            // The enemy will always be aware of the player
            target = GameObject.FindGameObjectWithTag("Player");
            animator = GetComponent<Animator>();
            enemyAudio = GetComponent<EnemyAudio>();

            Health = EnemyStats.Instance.maximumHealth;

            RegenHealthCoroutine = GenerateNewRegenCoroutine();

            Debug.Assert(animator, "Animator is null");
            Debug.Assert(aggroTime > 0, "Aggro time must be greater than 0");
        }

        public override void Die()
        {
            Debug.Log("Enemy died");

            IsDead = true;                        
                                         
            GetComponent<EnemyController>().SetState(EnemyState.Dead);

            // Instantly disable the NavMeshAgent to stop all collisions
            GetComponent<NavMeshAgent>().enabled = false;

            // Play sound
            enemyAudio.PlayDeathSound();

            // The game controller will handle the rest of the tasks that need to happen
            // when an enemy dies
            GameController.Instance.OnEnemyDeath(this, enemyAudio.GetDeathSoundClipLength());
        }
                
        public override void UpdateCooldown()
        {            
            TimeUntilNextPossibleAttack += EnemyStats.Instance.attackCooldown;
        }

        public override void OnAttack()
        {
            UpdateCooldown();

            // Play an attack animation
            animator.Play("Attack");
        }

        public override void OnAttacked(float damageAmount)
        {
            base.OnAttacked(damageAmount);

            // Play a sound when wounded
            if(!IsDead)
            {
                enemyAudio.PlayAttackedSound();
            }
            
            recentlyAttacked = true;
            StartCoroutine(AggroTime());
        }

        // A new IEnumerator object needs to be created each time after it finishes
        public override IEnumerator GenerateNewRegenCoroutine()
        {
            return RegenerateHealth(EnemyStats.Instance.maximumHealth,
                EnemyStats.Instance.healthRegenerationRate);
        }

        IEnumerator AggroTime()
        {
            yield return new WaitForSeconds(aggroTime);

            recentlyAttacked = false;
        }


        public void AttackSwingAnimEventBegin()
        {
            IKillable targetKillable = target.GetComponent<IKillable>();
            if(targetKillable != null)
            {
                //Debug.Log("Found target's IKillable");                
                targetKillable.OnAttacked(EnemyStats.Instance.weaponDamage);

                enemyAudio.PlayAttackSound();
            }          
        }

        public void ResetNextPossibleAttackTime() { TimeUntilNextPossibleAttack = 0.0f; }

        public void AttackSwingAnimEventEnd() { }

        public void AttackedAnimEventBegin() { }

        public void AttackedAnimEventEnd() { }

        public override EntityCombatStats GetCombatStats()
        {
            return EnemyStats.Instance;
        }

        public GameObject GetTarget() { return target; }       
        
        public bool IsFullHealth() { return Health == EnemyStats.Instance.maximumHealth; }
    }
}

