using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioPlayer
    {
        public static void PlaySound(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }

        public static void PlaySoundFromPlayer(AudioClip clip)
        {
            AudioSource source = GameController.Instance.Player.GetComponent<PlayerAudio>().MainAudioSource;
            source.clip = clip;
            source.Play();
        }

        public static void Play2DSound(AudioClip clip)
        {
            AudioSource source = GameController.Instance.SoundManager.GameAudioSource;

            // Use PlayOneShot as to not cancel clips that are already being played
            source.PlayOneShot(clip);
        }
    }
}


