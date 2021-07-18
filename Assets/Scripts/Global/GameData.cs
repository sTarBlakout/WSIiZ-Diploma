using UnityEngine;

namespace Global
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/Game Data")]
    public class GameData : ScriptableObject
    {
        public int targetFrameRate;
    }
}
