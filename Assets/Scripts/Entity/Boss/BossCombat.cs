using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Combat;
using Entity.Player;
using Entity.Stats;

namespace Entity.Boss
{
    public class BossCombat : BaseCombat
    {
		public float attackCooldown;

		[Header("Boss Specific")]
		public Projectile projectile;

		// The range in which a ranged attack can happen
		[field: SerializeField] public float RangedAttackRadius { get; private set; }

		// The range in which a melee attack can happen
		[field: SerializeField] public float MeleeAttackRadius { get; private set; }

		BossController controller;
		Animator animator;
		Transform projectileSpawn;
		Vector3 rangedTargetOffset;

		protected override void Start()
		{
			controller = GetComponent<BossController>();
			target = GameObject.FindGameObjectWithTag("Player");
			animator = GetComponent<Animator>();

			rangedTargetOffset = Vector3.up;

			// Get a reference to the projectile spawn object to obtain its position
			projectileSpawn = transform.GetChild(0).transform;
		}		
		
        public override void Die()
        {
			Debug.LogWarning("Boss died");

			IsDead = true;
		
			controller.SetState(BossController.BossState.Dead);

			// Prevents the player from targeting it
			GetComponent<SphereCollider>().enabled = false;

			// Disables collision avoidance for player and other agents
			GetComponent<NavMeshAgent>().enabled = false;

			GameController.Instance.OnBossKilled(gameObject);
		}

        public override void UpdateCooldown()
        {
			TimeUntilNextPossibleAttack += attackCooldown;
        }

        public override void OnAttack()
        {
			UpdateCooldown();

			BossController.BossState state = controller.State;			
			if(state == BossController.BossState.MeleeCombat)
            {
				//Debug.Log("Boss will perform a melee attack");
				animator.Play("MeleeAttack");
			}
			else
            {
				//Debug.Log("Boss will perform a ranged attack");
				animator.Play("RangedAttack");
			}		           
        }

		public void MeleeAttackAnimEvent()
        {
			Debug.Log("Melee attack!");

			IKillable targetKillable = target.GetComponent<IKillable>();
			if (targetKillable != null)
			{
				targetKillable.OnAttacked(BossStats.Instance.weaponDamage);

				GetComponent<BossAudio>().PlayMeleeAttackSound();				
			}
		}

		public void RangedAttackAnimEvent()
        {
			//Debug.Log("Ranged attack launched");

			Projectile clone = Instantiate(projectile, projectileSpawn.position, Quaternion.identity);
			clone.velocity = (target.transform.position + rangedTargetOffset - projectileSpawn.position).normalized;
			clone.PlayAudioClip(GetComponent<BossAudio>().GetProjectileSound());
		}

        public override EntityCombatStats GetCombatStats()
        {
			return BossStats.Instance;
        }

        public override IEnumerator GenerateNewRegenCoroutine()
        {
            throw new System.NotImplementedException();
        }
    }
}