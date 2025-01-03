using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    public int coinValue = 1; // Value of the coin

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CoinManager.Instance.AddCoins(coinValue);
            gameObject.SetActive(false); // Remove the coin from the scene
        }
    }
}
