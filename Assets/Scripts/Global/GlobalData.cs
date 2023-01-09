using UnityEngine;

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
        
        public int CurrPlayerLevel
        {
            get => _currPlayerLevel;
            set => _currPlayerLevel = value;
        }
        private int _currPlayerLevel;


        public void SavePlayerCharacterBloodLevel(int bloodLevel)
        {
            PlayerPrefs.SetInt("BloodLevel", bloodLevel);
            PlayerPrefs.Save();
        }

        public PlayerCharacterPrefs LoadPlayerCharacterPrefs()
        {
            var charPrefs = new PlayerCharacterPrefs();
            charPrefs.BloodLevel = PlayerPrefs.GetInt("BloodLevel", -1);

            return charPrefs;
        }

        public void SaveGlobalPrefs()
        {
            PlayerPrefs.SetInt("CurrentLevel", _currLevel);
            PlayerPrefs.Save();
        }
 
        public void LoadGlobalPrefs()
        {
            _currLevel = PlayerPrefs.GetInt("CurrentLevel", 0); 
        }
    }

    public class PlayerCharacterPrefs
    {
        public int BloodLevel;
    }
}
