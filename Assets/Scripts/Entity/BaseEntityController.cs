using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Entity
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class BaseEntityController : MonoBehaviour, IInteractable, IIdentifiable
    {
        protected NavMeshAgent agent;

        
        public GameObject Target { get; private set; }

        protected virtual void Start()
        {
            Target = GameObject.FindGameObjectWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
        }

        public abstract void OnInteract();

        public abstract string GetName();

        public void SetDestination(Vector3 dest)
        {
            agent.SetDestination(dest);
        }
        public void SetDestination()
        {
            Vector3 dest = Target.transform.position;
            agent.SetDestination(dest);
        }
        public bool ReachedTarget()
        {
            return agent.velocity == Vector3.zero &&
                   agent.remainingDistance <= agent.stoppingDistance;
        }

        public static bool IsTargetWithinRadius(Vector3 subjectPos, Vector3 targetPos, float radius)
        {
            float distanceToTarget = Vector3.Distance(subjectPos, targetPos);
            return distanceToTarget < radius;
        }

    }
}


