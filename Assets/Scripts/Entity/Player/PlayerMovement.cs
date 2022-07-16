using UnityEngine;
using UnityEngine.AI;
using Audio;
using Entity.Stats;

namespace Entity.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public GameObject target;
        public bool approaching;
        public float interactableStoppingDistance;
        public float groundTargetStoppingDistance;

        Animator animator;
        NavMeshAgent navMeshAgent;
        PlayerAudio playerAudio;
        PlayerTargeting targeting;

        public float DeathAnimationClipLength { get; private set; }

        void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            playerAudio = GetComponent<PlayerAudio>();
            targeting = GetComponent<PlayerTargeting>();

            Debug.Assert(interactableStoppingDistance != 0, "Interactable stopping distance should not be 0, this may " +
                "cause issues with the NavMeshAgent");

            navMeshAgent.speed = PlayerStats.Instance.MovementSpeed;

            // Find the length of the death animation clip
            DeathAnimationClipLength = GetAnimationClipLength("player_death");
        }

        void Update()
        {
            // Make sure the target is the same
            target = targeting.target;

            if (target)
            {
                // Check to see if we are still approaching
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && approaching)
                {
                    approaching = false;

                    //Debug.Log("We are no longer approaching");

                    IInteractable interactable = target.GetComponent<IInteractable>();
                    if (interactable == null)
                    {
                        Debug.LogError("Target is not interactable!");
                    }
                    else
                    {
                        interactable.OnInteract();
                    }
                }

                // If we are still approaching, make sure we have an up to date position
                if (approaching)
                {
                    navMeshAgent.SetDestination(target.transform.position);
                }
            }

            // Handle animation changes
            bool isMoving = navMeshAgent.velocity != Vector3.zero;
            animator.SetBool("isMoving", isMoving);
        }

        public void MoveTowards(GameObject target)
        {
            this.target = target;
            navMeshAgent.stoppingDistance = interactableStoppingDistance;
            approaching = true;

            navMeshAgent.SetDestination(target.transform.position);
        }
        public void MoveTowardsGroundTarget(Vector3 target)
        {
            navMeshAgent.stoppingDistance = groundTargetStoppingDistance;
            approaching = false;

            navMeshAgent.SetDestination(target);
        }

        public bool IsMoving()
        {
            return navMeshAgent.velocity.magnitude > 0.0f;
        }

        private void Footstep()
        {
            playerAudio.PlayFootstepSound();
        }

        // Should be moved to some utility file...
        private float GetAnimationClipLength(string animClipName)
        {
            float length = 0;
            
            // Obtain the animator's animator controller
            RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;

            // Find the clip by name
            foreach(AnimationClip animClip in runtimeAnimatorController.animationClips)
            {
                if(animClipName == animClip.name)
                {
                    length = animClip.length;
                    break;
                }                
            }

            return length;
        }
    }
}