using UnityEngine;

namespace Global
{
    public class GlobalManager : MonoBehaviour
    {
        [SerializeField] private GlobalData globalData;
        
        public static GlobalManager Instance { get; private set; }
        public GlobalData GlobalData => globalData;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            globalData.LoadGlobalPrefs();
        }

        private void Start()
        {
            Application.targetFrameRate = globalData.TargetFrameRate;
            globalData.Init();
        }
    }
}
