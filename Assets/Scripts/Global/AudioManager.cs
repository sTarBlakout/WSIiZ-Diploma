using System;
using UnityEngine;

namespace Global
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource otherSounds;
        [SerializeField] private AudioSource otherContinuousSounds;
        [SerializeField] private AudioSource backgroundMusic;
        
        public static AudioManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            PlayBackgroundMusic(GlobalManager.Instance.GlobalData.IsMusicEnabled);
        }

        public void PlayBackgroundMusic(bool play)
        {
            if (play) backgroundMusic.Play();
            else backgroundMusic.Stop();
        }

        public void PlaySound(AudioClip clip)
        {
            otherSounds.clip = clip;
            otherSounds.Play();
        }

        public void PlayContinuousSound(bool play, AudioClip clip = null)
        {
            if (clip != null) otherContinuousSounds.clip = clip;
            if (play) otherContinuousSounds.Play();
            else otherContinuousSounds.Stop();
        }

        public void Vibrate()
        {
            if (!GlobalManager.Instance.GlobalData.IsVibrationEnabled) return;
            Handheld.Vibrate();
        }
    }
}
