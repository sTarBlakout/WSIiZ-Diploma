using System;
using UnityEngine;

namespace Global
{
    public class AudioManager : MonoBehaviour
    {
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
    }
}
