using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public abstract class CombatEntityAudio : MonoBehaviour
    {
        // The main audio source of the object
        public AudioSource MainAudioSource { get; protected set; }

        // The audio source for the weapon
        protected AudioSource weaponAudioSource;

        public virtual void Start()
        {
            MainAudioSource = transform.Find("AudioSource_Main").GetComponent<AudioSource>();

            weaponAudioSource = transform.Find("AudioSource_Weapon").gameObject.GetComponent<AudioSource>();
            Debug.Assert(weaponAudioSource, "Could not find AudioSource component on " +
                "AudioSource_Weapon on GameObject hierarchy: " + gameObject.name);
        }

        public abstract void PlayAttackSound();
    }
}