using System.Collections.Generic;
using Gameplay.Interfaces;
using Gameplay.Items;
using UnityEngine;

namespace Global
{
    [CreateAssetMenu(fileName = "GlobalData", menuName = "Data/Global Data")]
    public class GlobalData : ScriptableObject
    {
        [SerializeField] private int targetFrameRate;
        [SerializeField] private float repeatLevelEnemyBloodModifier;
        [SerializeField] private LevelList levelList;
        [SerializeField] private List<GameObject> items;

        public int TargetFrameRate => targetFrameRate;
        public LevelList LevelList => levelList;

        public float RepeatLevelEnemyBloodModifier
        {
            get
            {
                if (_repeatLevelCount == 0) return 1f;
                return repeatLevelEnemyBloodModifier* _repeatLevelCount;
            }
        }

        public int RepeatLevelCount => _repeatLevelCount;

        private int _repeatLevelCount = 0;

        public int CurrLevel
        {
            get => _currLevel;
            set
            {
                _currLevel = value;
                if (_currLevel >= levelList.LevelsAmount)
                {
                    _currLevel = 0;
                    _repeatLevelCount++;
                    SaveRepeatLevelCount();
                }
            }
        }
        private int _currLevel;
        
        public int CurrPlayerLevel
        {
            get => _currPlayerLevel;
            set => _currPlayerLevel = value;
        }
        private int _currPlayerLevel;

        public void Init()
        {
            _repeatLevelCount = LoadRepeatLevelCount();
        }
        
        public int LoadRepeatLevelCount()
        {
            return PlayerPrefs.GetInt("RepeatLevelCount", 0);
        }
        
        public void SaveRepeatLevelCount()
        {
            PlayerPrefs.SetInt("RepeatLevelCount", _repeatLevelCount);
            PlayerPrefs.Save();
        }
        
        public void SavePlayerCharacterItem(int itemId)
        {
            PlayerPrefs.SetInt("Item_" + itemId, itemId);
            PlayerPrefs.Save();
        }
        
        public void SavePlayerCharacterBloodLevel(int bloodLevel)
        {
            PlayerPrefs.SetInt("BloodLevel", bloodLevel);
            PlayerPrefs.Save();
        }

        public PlayerCharacterPrefs LoadPlayerCharacterPrefs()
        {
            var charPrefs = new PlayerCharacterPrefs();
            charPrefs.BloodLevel = PlayerPrefs.GetInt("BloodLevel", -1);

            foreach (var itemGO in items)
            {
                var item = itemGO.GetComponent<IItem>();
                var itemId = PlayerPrefs.GetInt("Item_" + item.ItemData.ItemId, -1);
                if (itemId != -1) charPrefs.Items.Add(itemGO);
            }

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
        public List<GameObject> Items = new List<GameObject>();
    }
}
