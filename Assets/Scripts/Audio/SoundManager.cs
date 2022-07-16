using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class SoundManager : MonoBehaviour
    {
        // A reference to the AudioClip that plays when an enemy loses interest in the player
        public AudioClip enemyLostPlayer;

        [Header("Potion Sounds")]
        public AudioClip potionPickupSound;
        public AudioClip potionCycleSound;
        public AudioClip potionConsumedSound;

        public AudioSource GameAudioSource { get; private set; }
        public GameObject PotionAudioSource { get; private set; }

        void Start()
        {
            GameAudioSource = GameObject.Find("AudioSource_Game").GetComponent<AudioSource>();
            PotionAudioSource = GameObject.Find("AudioSource_Potion");
        }

        public static int GetRandomIndex(int arraySize)
        {
            return Random.Range(0, arraySize);
        }

        // Finds the longest AudioClip in the array of clips and returns the length
        public static float FindLongestClipLength(AudioClip[] clips)
        {
            float longestLength = 0;
            foreach (AudioClip clip in clips)
            {
                if (clip.length > longestLength)
                {
                    longestLength = clip.length;
                }
            }
            return longestLength;
        }

        public void SetPotionAudioSourcePosition(Vector3 position)
        {
            PotionAudioSource.transform.position = position;
        }

        public void DisablePotionAudioSource()
        {
            PotionAudioSource.SetActive(false);
        }
    }
}

