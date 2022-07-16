using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class EnemyAudioData : MonoBehaviour
{
    public static EnemyAudioData Instance { get; private set; }

    public AudioClip[] weaponSounds;
    public AudioClip[] attackedSounds;
    public AudioClip[] deathSounds;
    public AudioClip[] footstepSounds;

    public AudioClip aggroSound;

    // Stores the length of the longest death sound AudioClip
    public float DeathSoundsLongestClipLength { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        DeathSoundsLongestClipLength = SoundManager.FindLongestClipLength(deathSounds);
        Debug.Log("Longest Enemy death sound clip length: " + DeathSoundsLongestClipLength);
    }
}
