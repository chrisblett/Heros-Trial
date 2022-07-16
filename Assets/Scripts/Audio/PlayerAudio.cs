using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Item;

namespace Audio
{
    public class PlayerAudio : CombatEntityAudio
    {
        // Used to play audio from the player position
        //public AudioSource MainAudioSource { get; private set; }

        public AudioClip[] weaponSounds;
        public AudioClip[] footstepSounds;

        // Weapon sounds that play when the player is under the effect of a weapon damage potion
        public AudioClip[] critWeaponSounds;

        public static float footStepSoundVolume = 0.33f;

        public override void Start()
        {
            base.Start();
        }

        public override void PlayAttackSound()
        {
            AudioClip[] attackSounds;

            // Determine what weapon sound to play depending on whether the player is
            // under the effects of a weapon damage potion
            if (GameController.Instance.IsPotionTypeActive(PotionType.WeaponDamage))
            {                
                attackSounds = critWeaponSounds;
            }
            else
            {                
                attackSounds = weaponSounds;
            }

            AudioPlayer.PlaySound(weaponAudioSource, attackSounds[SoundManager.GetRandomIndex(attackSounds.Length)]);
        }

        public void PlayFootstepSound()
        {
            MainAudioSource.PlayOneShot(footstepSounds[SoundManager.GetRandomIndex(footstepSounds.Length)]
                , footStepSoundVolume);
        }
    }
}


