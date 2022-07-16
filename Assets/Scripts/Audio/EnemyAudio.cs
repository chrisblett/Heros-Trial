using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class EnemyAudio : CombatEntityAudio
    {
        // Reference to the Singleton GameObject holding the audio data
        EnemyAudioData enemyAudioData;

        public static float footstepVolume = 0.5f;

        public override void Start()
        {
            base.Start();

            enemyAudioData = EnemyAudioData.Instance;
        }

        public override void PlayAttackSound()
        {
            AudioClip[] weaponSounds = enemyAudioData.weaponSounds;

            AudioPlayer.PlaySound(weaponAudioSource, weaponSounds[SoundManager.GetRandomIndex(weaponSounds.Length)]);
        }

        public void PlayAggroSound()
        {
            AudioPlayer.PlaySound(MainAudioSource, enemyAudioData.aggroSound);
        }

        public void PlayAttackedSound()
        {
            AudioClip[] attackedSounds = enemyAudioData.attackedSounds;

            AudioPlayer.PlaySound(MainAudioSource,
                attackedSounds[SoundManager.GetRandomIndex(attackedSounds.Length)]);
        }

        public void PlayDeathSound()
        {
            AudioClip[] deathSounds = enemyAudioData.deathSounds;

            AudioPlayer.PlaySound(MainAudioSource, deathSounds[SoundManager.GetRandomIndex(deathSounds.Length)]);
        }

        public void PlayFootstepSound()
        {
            AudioClip[] footStepSounds = enemyAudioData.footstepSounds;
            MainAudioSource.PlayOneShot(footStepSounds[SoundManager.GetRandomIndex(footStepSounds.Length)]
                , footstepVolume);
        }

        public float GetDeathSoundClipLength()
        {
            return enemyAudioData.DeathSoundsLongestClipLength;
        }
    }
}


