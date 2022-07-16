using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public float smoothTime;
    public float targetZOffset;
    public float horizontalLookAheadStrength;
    public float verticalLookAheadStrength;

    // The Transform the camera should look at during its animation
    public Transform animationTarget;

    public Vector3 velocity;

    NavMeshAgent targetAgent;
    Transform target;
    Vector3 offset;

    // Whether the camera is currently in progress with its animation
    bool inAnimation;

    void Start()
    {        
        // The offset the camera is from its target at all times
        offset = new Vector3(0, 15, -5);

        // If it is the start of the forest level
        if(SceneManager.GetActiveScene().buildIndex == (int)GameScene.Forest)
        {
            target = GameObject.Find("CamTarget").transform;
            StartCoroutine(DoAnimation());
        }
        else
        {
            SetPlayerAsTarget();
        }

        transform.position = target.transform.position + offset;
    }

    private void SetPlayerAsTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetAgent = target.GetComponent<NavMeshAgent>();
        if (!targetAgent)
        {
            Debug.LogError("NavMeshAgent could not found. This script requires the target to have a NavMeshAgent");
        }
    }

    private void Animation_ForestStart()
    {
        inAnimation = true;
        //Debug.Log("Target GameObject: " + target.gameObject.name);
    }

    // Animation shows the target where the player should head for
    private IEnumerator DoAnimation()
    {
        Animation_ForestStart();

        yield return new WaitForSeconds(5);

        inAnimation = false;
        SetPlayerAsTarget();
    }

    void Update()
    {
        if(!inAnimation)
        {
            FollowTarget();
        }
    }

    void FollowTarget()
    {
        Vector3 targetVelocity = targetAgent.velocity;

        //Debug.Log("Target velocity is " + targetVelocity);

        // Every frame we want the camera to move depending on the difference in the player position
        // and its own position. I add the velocity of the player to this velocity, so the camera is always
        // looking ahead of where the player is moving towards
        float targetXDifference = target.transform.position.x - transform.position.x;
        float targetZDifference = target.transform.position.z - transform.position.z - targetZOffset;

        Vector3 cameraVelocity = new Vector3(targetXDifference + targetVelocity.x * horizontalLookAheadStrength, 0.0f,
            targetZDifference + targetVelocity.z * verticalLookAheadStrength);

        //Debug.Log("Camera velocity is: " + cameraVelocity);

        // Use smoothing so the camera is not just fixed on the player and has some delay to it.
        // This also prevents jerky movmement since the target's velocity can change rapidly
        transform.position = Vector3.SmoothDamp(transform.position, transform.position + cameraVelocity,
            ref velocity, smoothTime);
    }    

    public void ForcePositionChange(Vector3 pos)
    {        
        transform.position = pos + offset;
    }
}
