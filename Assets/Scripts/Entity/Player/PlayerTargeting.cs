using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entity.Combat;

namespace Entity.Player
{
    public class PlayerTargeting : MonoBehaviour
    {
        public GameObject target;

        Camera cam;
        GameObject targetMarker;
        PlayerCombat combat;
        PlayerMovement movement;

        void Start()
        {
            targetMarker = GameObject.Find("Target Marker");
            cam = Camera.main;
            combat = GetComponent<PlayerCombat>();
            movement = GetComponent<PlayerMovement>();
        }

        void Update()
        {         
            if (target)
            {
                BaseCombat targetCombat = target.GetComponent<BaseCombat>();
                if (targetCombat.IsDead)
                {
                    Debug.Log("Player target is dead");
                    combat.OnCombatEnd();
                    target = null;

                    GameController.Instance.OnEnemyTargetLost();
                }

                UpdateTargetMarker();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if(GameController.Instance.CanPlayerTarget())
                {
                    // Cast a ray from the camera into the world
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                    // Stores information about the Raycast
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        LayerMask layerMask = LayerMask.GetMask("Targetable");

                        // Given a layer mask value, get back to the layer number
                        int layerMaskLayerNumber = (int)Mathf.Log(layerMask, 2);
                       
                        Vector3 destination;

                        // Do not allow the player to change target whilst in the process
                        // of attacking a target
                        if (combat.IsAttacking)
                        {
                            Debug.Log("You cannot change target while attacking!");
                            return;
                        }

                        // The GameObject that the ray hit
                        GameObject hitObject = hit.collider.gameObject;
                        if (hitObject.Equals(target))
                        {
                            Debug.Log("You are already targeting that object!");
                            return;
                        }

                        //Debug.Log("Hit object layer is " + hitObject.layer);

                        // Only process ray hits that occurred on the current LayerMask layer
                        if (hitObject.layer == layerMaskLayerNumber)
                        {
                            bool interactable = hitObject.GetComponent<IInteractable>() != null;
                            if (interactable)
                            {
                                destination = hitObject.transform.position;
                                OnTargetedInteractable(hitObject);
                            }
                            else
                            {
                                destination = hit.point;
                                OnTargetedGround(hit.point);
                            }

                            // If the target is not an interactable object, set it to null
                            target = interactable ? hitObject : null;

                            // Update the visual target marker to better visualise the destination
                            targetMarker.transform.position = destination;
                        }
                    }
                }
                else
                {
                    Debug.Log("You cannot target anything right now!");
                }
            }          
        }

        private void OnTargetedInteractable(GameObject target)
        {            
            // Check if it is identifiable
            IIdentifiable identifiable = target.GetComponent<IIdentifiable>();
            if(identifiable != null)
            {
                // Attempt to find the target's combat script
                BaseCombat combat = target.GetComponent<BaseCombat>();
                if(combat != null)
                {
                    // The target has combat information that can be used for the UI
                    GameController.Instance.OnEnemyTargetAcquired(combat, identifiable.GetName());
                }                
            }

            this.target = target;
            movement.MoveTowards(target);

            // Update the visual target marker to better visualise the destination
            targetMarker.gameObject.GetComponent<Renderer>().material.color = Color.red;
            targetMarker.transform.position = target.transform.position;
        }

        private void OnTargetedGround(Vector3 hitPoint)
        {
            // If we are targeting the ground, then we no longer have an enemy target
            GameController.Instance.OnEnemyTargetLost();

            targetMarker.gameObject.GetComponent<Renderer>().material.color = Color.green;

            GetComponent<PlayerMovement>().MoveTowardsGroundTarget(hitPoint);

            // If the player is in combat, inform the combat script that the player
            // is no longer fighting
            if (combat.InCombat)
            {
                combat.OnCombatEnd();

                //Debug.Log("Player left combat, reason: Selected ground target");
            }
        }

        public bool HasTarget()
        {
            return target;
        }

        public bool HasEnemyTarget()
        {
            return HasTarget() && target.CompareTag("Enemy");
        }

        private void UpdateTargetMarker()
        {
            Vector3 destination = GetComponent<NavMeshAgent>().destination;

            // Update the visual target marker to better visualise the destination
            targetMarker.transform.position = destination;

            //Debug.Log("Updating target marker");
        }
    }
}


