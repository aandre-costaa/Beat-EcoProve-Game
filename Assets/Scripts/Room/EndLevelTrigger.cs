using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelTrigger : MonoBehaviour
{
    public GameObject popupUI;   
    public Text popupText;       
    public int levelNumber = 2;  
    public int totalCoins = 10;  
    private CoinManager coinManager;
    private bool levelEnded = false;
    private UIManager uiManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip winSound;
    
    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !levelEnded)
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null && playerHealth.isDead)
            {
                Debug.Log("Player is dead, level completion ignored");
                return; 
            }
            levelEnded = true;
            
            SaveLevelDataLocally();
            if (uiManager != null)
            {
                uiManager.PauseGameFinished(true);
            }
            
            var musicManager = GameObject.Find("MusicManager");
            if (musicManager != null)
            {
                var musicAudio = musicManager.GetComponent<AudioSource>();
                if (musicAudio != null && musicAudio.isPlaying)
                {
                    StartCoroutine(FadeOutMusic(musicAudio, 1f)); // 2 seconds fade-out
                }
            }
            SoundManager.Instance.PlaySound(winSound);

            Debug.Log($"Finished Level {levelNumber}");
        }
    }

    private void ShowPopup()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(true);

            // Display collected coins and level completion
            if (popupText != null)
            {
                popupText.text = $"Level {levelNumber} completed!\nEcoCoins collected: {totalCoins}";
            }
        }
    }

    private void SaveLevelDataLocally()
    {
        coinManager = FindObjectOfType<CoinManager>();

        if (coinManager != null)
        {
            totalCoins = coinManager.GetTotalCoins();
        }
        else
        {
            Debug.LogError("CoinManager not found in the scene!");
        }
        LevelManager levelManager = FindObjectOfType<LevelManager>();

        if (levelManager != null)
        {
            levelNumber = levelManager.GetCurrentLevel();
        }
        else
        {
            Debug.LogError("LevelManager not found in the scene!");
            levelNumber = -1;
        }
        ShowPopup();


        // Guarda as coins para o nivel 
        PlayerPrefs.SetInt($"Level_{levelNumber}_Coins", totalCoins);

        // Guarda o ultimo nivel completado
        if (PlayerPrefs.GetInt("LastCompletedLevel", 0) < levelNumber)
        {
            PlayerPrefs.SetInt("LastCompletedLevel", levelNumber);
        }


        // Parte para guardar o nivel 
        PlayerPrefs.SetInt("Level_" + levelNumber + "_Completed", 1);

        // Guarda a informa��o
        PlayerPrefs.Save();

        // Debug para ver se funciona
        int coinsInLevel = PlayerPrefs.GetInt($"Level_{levelNumber}_Coins", 0);
        Debug.Log($"Coins in Level {levelNumber}: {coinsInLevel}");
        Debug.Log($"Level {levelNumber} data saved locally: Coins = {totalCoins}");

        //Parte para ativar a curiosidade
        PlayerPrefs.SetString("ShowCuriosidadeQuiz", "true");
        PlayerPrefs.Save();
        Debug.Log("Quiz de curiosidade ativado - ShowCuriosidadeQuiz definido como true");
    }

    private IEnumerator FadeOutMusic(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        audioSource.volume = startVolume; // Restore for reuse
    }


}
