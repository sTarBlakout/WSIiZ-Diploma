using UnityEngine;

namespace Global
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameData gameData;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = gameData.targetFrameRate;
        }
    }
}
