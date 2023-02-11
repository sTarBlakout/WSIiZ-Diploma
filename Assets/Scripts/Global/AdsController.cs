using UnityEngine;

namespace Global
{
    public class AdsController : MonoBehaviour
    {
        const string SdkKey = "TEST_SDK_KEY";
        const string InterstitialId = "TEST_INTERSTITIAL_KEY";

        public bool InterstitialAdLoaded => MaxSdk.IsInterstitialReady(InterstitialId);
        
        public static AdsController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            if (!MaxSdk.IsInitialized())
            {
                MaxSdkCallbacks.OnSdkInitializedEvent += InitializeSdk;

                MaxSdk.SetSdkKey(SdkKey);
                MaxSdk.InitializeSdk();
            }
        }

        void InitializeSdk(MaxSdkBase.SdkConfiguration config)
        {
            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialHidden;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedToLoad;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += OnInterstitialFailedToDisplay;
            LoadInterstitialAd();
        }
        
        private void LoadInterstitialAd()
        {
            MaxSdk.LoadInterstitial(InterstitialId);
        }
        
        public void ShowInterstitialAd()
        {
            MaxSdk.ShowInterstitial(InterstitialId);
        }

        void OnInterstitialHidden(string unitId)
        {
            LoadInterstitialAd();
        }

        void OnInterstitialFailedToLoad(string unitId, int errorCode)
        {
            Debug.LogWarningFormat("Interstitial Ad failed to load, error code: {0}", errorCode);
            LoadInterstitialAd();
        }

        void OnInterstitialFailedToDisplay(string unitId, int errorCode)
        {
            Debug.LogWarningFormat("Interstitial Ad failed to display, error code: {0}", errorCode);
            LoadInterstitialAd();
        }
    }
}
