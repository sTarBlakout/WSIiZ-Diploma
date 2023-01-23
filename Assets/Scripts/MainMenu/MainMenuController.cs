using System;
using Doozy.Engine.UI;
using Global;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private UIView optionsPanel;
        [SerializeField] private Text musicToggleText;
        [SerializeField] private Text vibrationToggleText;

        private void Start()
        {
            optionsPanel.Hide(true);
            UpdateOptionTexts();
        }

        private void UpdateOptionTexts()
        {
            musicToggleText.text = "MUSIC: " + (GlobalManager.Instance.GlobalData.IsMusicEnabled ? "ON" : "OFF");
            vibrationToggleText.text = "VIBRATION: " + (GlobalManager.Instance.GlobalData.IsVibrationEnabled ? "ON" : "OFF");
        }

        public void StartGame()
        {
            SceneManager.LoadScene("GameArena");
        }

        public void ShowOptionsMenu(bool show)
        {
            if (show) optionsPanel.Show();
            else optionsPanel.Hide();
        }

        public void ToggleMusic()
        {
            var currState = GlobalManager.Instance.GlobalData.IsMusicEnabled;
            GlobalManager.Instance.GlobalData.SaveToggleMusic(!currState);
            GlobalManager.Instance.GlobalData.ReloadOptions();
            UpdateOptionTexts();
        }

        public void ToggleVibration()
        {
            var currState = GlobalManager.Instance.GlobalData.IsVibrationEnabled;
            GlobalManager.Instance.GlobalData.SaveToggleVibration(!currState);
            GlobalManager.Instance.GlobalData.ReloadOptions();
            UpdateOptionTexts();
        }
    }
}
