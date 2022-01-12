using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    [CreateAssetMenu(fileName = "LevelList", menuName = "Data/Level List")]
    public class LevelList : ScriptableObject
    {
        [SerializeField] private List<GameObject> levels;

        public GameObject GetLevel(int lvlIdx)
        {
            return lvlIdx >= levels.Count ? null : levels[lvlIdx];
        }
    }
}
