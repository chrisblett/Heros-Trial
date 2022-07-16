using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport_Watchtower : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            GameController.Instance.TeleportPlayer(transform.position);
        }
    }
}
