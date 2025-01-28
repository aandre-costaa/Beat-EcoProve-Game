using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    private int totalCoins = 0;
    public Text coinText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the manager persists across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void Update()
    {
        coinText.text = totalCoins.ToString();
    }


    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Debug.Log("Coins Collected: " + totalCoins);
        // You can update a UI element here
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }
}
