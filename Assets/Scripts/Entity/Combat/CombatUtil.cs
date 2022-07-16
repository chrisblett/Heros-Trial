using UnityEngine;
using UnityEngine.AI;

namespace Entity.Combat
{
    class CombatUtil
    {
        public static bool IsFacingTarget(Transform subject, Vector3 targetPosition)
        {
            /*
             1 - facing target directly
             0 - perpendicular to target
            -1 - opposite direction to target
            */

            const float THRESHOLD = 1.00f;
			const float EPSILON = 0.001f;
				
			Vector3 toTarget = targetPosition - subject.position;
			
			// Remove the y-component of the vector as we do not want to consider vertical offsets.
			toTarget.y = 0.0f;
			toTarget.Normalize();

            Vector3 forward = subject.forward;

            float result = Vector3.Dot(toTarget, forward);            
      
			// Close enough
			return Mathf.Abs(THRESHOLD - result) < EPSILON;					
        }

        public static void FaceTarget(Transform subject, Vector3 targetPosition)
        {
            Vector3 toTarget = targetPosition - subject.position;

            // Zero out the y component because we only care about the difference
            // between the targets on the X-Z plane. This also prevents issues that may arise with rotations
            // if the target transform has a significant y difference
            toTarget.y = 0.0f;

            Quaternion targetRotation = Quaternion.LookRotation(toTarget, Vector3.up);

            NavMeshAgent navMeshAgent = subject.GetComponent<NavMeshAgent>();

            // Smoothly rotate to face the target
            subject.rotation = Quaternion.RotateTowards(subject.rotation,
                targetRotation, navMeshAgent.angularSpeed * Time.deltaTime);
        }

        public static bool InRange(Vector3 subject, Vector3 target, float range)
        {
            return Vector3.Distance(subject, target) <= range;
        }
    }
}
