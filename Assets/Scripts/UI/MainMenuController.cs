using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClickSound;
    public void PlayGame()
    {
        SoundManager.Instance.PlaySound(buttonClickSound);
        SceneManager.LoadScene("_LevelsPage");
    }

    public void QuitGame()
    {
        SoundManager.Instance.PlaySound(buttonClickSound);
        Debug.Log("Game Quit");
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
