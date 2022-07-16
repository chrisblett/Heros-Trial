using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;

namespace Entity.Item
{
    public enum PotionType
    {
        Health,        
        WeaponDamage,

        MaxPotionTypes
    }

    public class Potion : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public PotionType Type { get; private set; }
                        
        void OnTriggerEnter(Collider other)
        {
            // Only react to the player
            if(other.CompareTag("Player"))
            {
                OnInteract();                
                Destroy(gameObject);
            }            
        }

        public void OnInteract()
        {
            GameController.Instance.OnPotionObtained(transform, Type);
        }
    }
}

