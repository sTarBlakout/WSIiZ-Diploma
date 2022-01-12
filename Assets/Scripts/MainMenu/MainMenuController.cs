using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("GameArena");
        }
    }
}
