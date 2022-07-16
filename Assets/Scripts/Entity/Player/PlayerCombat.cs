using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Combat;
using Entity.Stats;
using Audio;

namespace Entity.Player
{
    public class PlayerCombat : BaseCombat
    {
        // Whether the player is currently in combat
        [field : SerializeField] public bool InCombat { get; private set; }

        // Whether the player is currently attacking a target
        public bool IsAttacking { get; private set; }

        // A reference to the player's Animator component
        Animator animator;

        // A reference to the player's PlayerAudio component
        PlayerAudio playerAudio;        

        // A reference to the player's PlayerTargeting component
        PlayerTargeting targeting;

        protected override void Start()
        {         
            animator = GetComponent<Animator>();
            targeting = GetComponent<PlayerTargeting>();
            playerAudio = GetComponent<PlayerAudio>();

            Health = PlayerStats.Instance.maximumHealth;

            RegenHealthCoroutine = GenerateNewRegenCoroutine();
        }

        protected override void Update()
        {
            base.Update();

            if (InCombat)
            {
                // Make sure the player's target is up to date
                target = targeting.target;
         
                if (CombatUtil.InRange(transform.position, target.transform.position,
                PlayerStats.Instance.attackRadius))
                {
                    if (CombatUtil.IsFacingTarget(transform, target.transform.position))
                    {
                        if (TimeUntilNextPossibleAttack <= 0.0f)
                        {
                            OnAttack();
                        }
                    }
                    else
                    {                        
                        CombatUtil.FaceTarget(transform, target.transform.position);
                    }
                }                
                else
                {
                    InCombat = false;
                }
            }
        }

        public override IEnumerator GenerateNewRegenCoroutine()
        {
            return RegenerateHealth(PlayerStats.Instance.maximumHealth,
                PlayerStats.Instance.healthRegenerationRate);
        }
        
        public override IEnumerator RegenerateHealth(float maximumHealth, float regenRate)
        {            
            while (Health < maximumHealth)
            {
                Health += regenRate * Time.deltaTime;                
                GameController.Instance.OnPlayerHealthChanged();

                yield return null;
            }
            
            Health = maximumHealth;
            RegenHealthCoroutine = GenerateNewRegenCoroutine();         
        }

        public override EntityCombatStats GetCombatStats()
        {
            return PlayerStats.Instance;
        }

        public override void UpdateCooldown()
        {
            // Add the cooldown value after each attack
            TimeUntilNextPossibleAttack += PlayerStats.Instance.attackCooldown;
        }

        public void OnCombatStart()
        {
            InCombat = true;

            //Debug.LogWarning("Player started combat");
        }

        public void OnCombatEnd()
        {
            target = null;
            InCombat = false;
            
            //Debug.LogWarning("Player left combat");
        }

        public override void OnAttack()
        {
            IsAttacking = true;
            UpdateCooldown();

            animator.SetTrigger("Attack");
        }

        public override void OnAttacked(float damageAmount)
        {
            //Debug.Log("Player took " + damageAmount + " damage!");

            base.OnAttacked(damageAmount);

            GameController.Instance.OnPlayerHealthChanged();
        }
           
        // This is where the attack actually happens                        
        public void AttackSwingAnimEventBegin()
        {            
            if (IsDead) return;

            IKillable killable = target.GetComponent<IKillable>();
            float damage = PlayerStats.Instance.weaponDamage;            
            killable.OnAttacked(damage);

            playerAudio.PlayAttackSound();           
        }

        public void AttackSwingAnimEventEnd()
        {
            IsAttacking = false;
        }

        public void AttackedAnimEventBegin() { }

        public void AttackedAnimEventEnd() { }

        // Called on player death
        public override void Die()
        {
            IsDead = true;
            InCombat = false;

            // Stop the player from moving
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent)
            {
                if(agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }
                else
                {
                    Debug.LogWarning("Agent for GameObject: " + gameObject + " is no longer on the NavMesh!");
                }
            }

            // TODO: Play an animation, trigger game over, etc...
            animator.SetBool("isDead", true);

            //Debug.Log("Player died!");

            GameController.Instance.OnPlayerDeath();
        }

        // Sets the player's health to the value passed in by 'amount'
        public void SetHealth(float amount)
        {
            Health = amount;
            GameController.Instance.OnPlayerHealthChanged();
        }     
    }
}

