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
    }
}
