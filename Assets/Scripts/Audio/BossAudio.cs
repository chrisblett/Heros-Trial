using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class BossAudio : MonoBehaviour
{
    public AudioClip[] projectileCastSounds;
    public AudioClip[] meleeAttackSounds;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Returns a random audio clip from the array of projectile sounds
    public AudioClip GetProjectileSound()
    {
        return projectileCastSounds[SoundManager.GetRandomIndex(projectileCastSounds.Length)];
    }

    public void PlayMeleeAttackSound()
    {
        AudioPlayer.PlaySound(audioSource, meleeAttackSounds[SoundManager.GetRandomIndex(meleeAttackSounds.Length)]);
    }
}
