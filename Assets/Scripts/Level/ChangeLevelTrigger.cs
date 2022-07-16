using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevelTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        // If the player entered the trigger
        if (coll.CompareTag("Player"))
        {
            GameController.Instance.OnChangeLevel();
        }
    }
}
