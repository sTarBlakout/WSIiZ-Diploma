using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global
{
    [CreateAssetMenu(fileName = "GlobalData", menuName = "Data/Global Data")]
    public class GlobalData : ScriptableObject
    {
        [SerializeField] private int targetFrameRate;
        [SerializeField] private LevelList levelList;

        public int TargetFrameRate => targetFrameRate;
        public LevelList LevelList => levelList;

        public int CurrLevel
        {
            get => _currLevel;
            set
            {
                _currLevel = value;
                if (_currLevel >= levelList.LevelsAmount) _currLevel = 0;
            }
        }
        private int _currLevel;

        public void SavePrefs()
        {
            PlayerPrefs.SetInt("CurrentLevel", _currLevel);
            PlayerPrefs.Save();
        }
 
        public void LoadPrefs()
        {
            _currLevel = PlayerPrefs.GetInt("CurrentLevel", 0); 
        }
    }
}
