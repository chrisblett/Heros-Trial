using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterKeepTrigger : MonoBehaviour
{
    // Stores the Transform to position the player once they enter the Keep
    public Transform keepEntry;

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            Debug.Log("Player entering keep");

            GameController.Instance.OnEnterKeep(keepEntry);
        }
    }
}
