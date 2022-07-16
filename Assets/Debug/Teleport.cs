using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Teleport : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            NavMeshAgent nav = GameController.Instance.Player.GetComponent<NavMeshAgent>();

            nav.enabled = false;
            GameController.Instance.Player.transform.position = transform.position;
            nav.enabled = true;
        }
    }
}
