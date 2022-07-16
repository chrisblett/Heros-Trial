using System.Collections;
using UnityEngine;
using Entity.Stats;

namespace Entity.Combat
{
    public abstract class BaseCombat : MonoBehaviour, IKillable
    {
        // The GameObject which is being targeted
        protected GameObject target;        

        [field : SerializeField] public bool IsDead { get; protected set; }

        [field: SerializeField] public float Health { get; protected set; }

        // The time remaining until an attack can occur at the earliest
        public float TimeUntilNextPossibleAttack { get; protected set; }

        public IEnumerator RegenHealthCoroutine { get; protected set; }

        public abstract EntityCombatStats GetCombatStats();

		// Each concrete class must provide an implementation for the Unity message 'Start'
		protected abstract void Start();

        public abstract void OnAttack();
        public abstract void UpdateCooldown();
        public abstract void Die();

        public abstract IEnumerator GenerateNewRegenCoroutine();

        public virtual IEnumerator RegenerateHealth(float maximumHealth, float regenRate)
        {            
            while (Health < maximumHealth)
            {                
                Health += regenRate * Time.deltaTime;

                yield return null;
            }

            // Clamp the value if it has exceeded the maximum value
            Health = maximumHealth;
            RegenHealthCoroutine = GenerateNewRegenCoroutine();
        }

        protected virtual void Update()
        {
            // Even if the subject has left combat, the timers need to continue to update
            UpdateCombatTimer();
        }

        public virtual void OnAttacked(float amount)
        {
            Damage(amount);

            if (Health <= 0)
            {
                Health = 0;
                Die();                
            }
        }

        public void Damage(float amount)
        {
            Health -= amount;
        }
                  
        void UpdateCombatTimer()
        {
            TimeUntilNextPossibleAttack -= Time.deltaTime; 
            TimeUntilNextPossibleAttack = Mathf.Max(0, TimeUntilNextPossibleAttack);
        }
    }
}