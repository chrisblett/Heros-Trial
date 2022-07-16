using UnityEngine;
using UnityEngine.AI;
using Audio;

namespace Entity.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        // The array of points in its patrol path
        [field: SerializeField] public Transform[] Points { get; private set; }

        int destinationPoint;
        NavMeshAgent navMeshAgent;
        EnemyAudio enemyAudio;
        EnemyController controller;

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemyAudio = GetComponent<EnemyAudio>();
            controller = GetComponent<EnemyController>();

            OnStartedWander();
        }

        void Update()
        {
            if (controller.State == EnemyController.EnemyState.Wander)
            {
                //Debug.Log("Enemy destination is at position " + navMeshAgent.destination);

                if (navMeshAgent.remainingDistance <= 1.0f)
                {
                    MoveTowardsNextWaypoint();
                }
            }
        }

        public void OnStartedWander()
        {
            destinationPoint = 0;
            MoveTowardsNextWaypoint();
        }

        void MoveTowardsNextWaypoint()
        {
            //Debug.Log("moving towards next waypoint");

            navMeshAgent.destination = Points[destinationPoint].position;

            //Debug.LogWarning("New Enemy new destination set " + navMeshAgent.destination);

            // Allow for cycling through the waypoints
            destinationPoint = (destinationPoint + 1) % Points.Length;
        }

        // An animation event
        private void Footstep()
        {
            enemyAudio.PlayFootstepSound();
        }
    }
}

