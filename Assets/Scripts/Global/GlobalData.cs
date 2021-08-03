using UnityEngine;

namespace Global
{
    [CreateAssetMenu(fileName = "GlobalData", menuName = "Data/Global Data")]
    public class GlobalData : ScriptableObject
    {
        public int targetFrameRate;
    }
}
