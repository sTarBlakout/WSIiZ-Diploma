using UnityEngine;

namespace Global
{
    public class GlobalManager : MonoBehaviour
    {
        [SerializeField] private GlobalData globalData;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = globalData.targetFrameRate;
        }
    }
}
