using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private AudioClip buttonClickSound;

    private bool isPaused = false;
    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseScreen.activeInHierarchy)
                PauseGame(false);
            else
                PauseGame(true);
        }
    }

    #region Game Over
    public void GameOver()
    {   
        gameOverScreen.SetActive(true);
        SoundManager.Instance.PlaySound(gameOverSound);
    }

    public void Restart()
    {
        SoundManager.Instance.PlaySound(buttonClickSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SoundManager.Instance.PlaySound(buttonClickSound);
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        isPaused = status;
        SoundManager.Instance.PlaySound(buttonClickSound);
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1; // Controla o tempo do jogo
    }

    public void PauseGameFinished(bool status)
    {
        isPaused = status;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void TogglePause()
    {
        SoundManager.Instance.PlaySound(buttonClickSound);
        // Alterna entre pausa e jogo
        PauseGame(!isPaused);
    }
    #endregion
}
