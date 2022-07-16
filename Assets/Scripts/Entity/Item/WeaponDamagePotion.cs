using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Stats;

namespace Entity.Item
{
    public class WeaponDamagePotion : MonoBehaviour, IConsumable
    {
        public float duration;
        public float valueChange;

        private float originalValue;

        void Start()
        {
            originalValue = PlayerStats.Instance.weaponDamage;
        }
   
        public void Consume()
        {
            if(GameController.Instance.IsTimeLimitedPotionActive())
            {
                // If another potion is currently active, terminate its effect
                StopAllCoroutines();

                // Make sure to revert the effects before consuming the new potion
                PlayerStats.Instance.weaponDamage = originalValue;                
            }

            StartCoroutine(DoEffect());
            GameController.Instance.OnTimeLimitedPotionConsumed();
        }

        IEnumerator DoEffect()
        {                        
            // Apply the effect by modifiying the value
            PlayerStats.Instance.weaponDamage += valueChange;

            Debug.Log("Effect applied!");

            // Keep track of how long the potion has been active for
            float currentTime = 0;
            
            // The actual potion effect takes place during this loop
            while (currentTime < duration)
            {
                GameController.Instance.GetTimeLimitedPotionCurrentTime(currentTime, duration);
                currentTime += Time.deltaTime;                
                
                yield return null;
            }

            Debug.Log("Potion expired!");

            // Restore the original value
            PlayerStats.Instance.weaponDamage = originalValue;
            GameController.Instance.OnTimeLimitedPotionExpired();
        }
    }
}

