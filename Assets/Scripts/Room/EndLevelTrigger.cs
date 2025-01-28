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
            levelEnded = true;
            
            SaveLevelDataLocally();
            if (uiManager != null)
            {
                uiManager.PauseGameFinished(true);
            }

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
                popupText.text = $"Level {levelNumber} Completed!\nCoins Collected: {totalCoins}";
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

        // Guarda a informação
        PlayerPrefs.Save();

        // Debug para ver se funciona
        int coinsInLevel = PlayerPrefs.GetInt($"Level_{levelNumber}_Coins", 0);
        Debug.Log($"Coins in Level {levelNumber}: {coinsInLevel}");
        Debug.Log($"Level {levelNumber} data saved locally: Coins = {totalCoins}");
    }

}
