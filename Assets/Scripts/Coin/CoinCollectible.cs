using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    public int coinValue = 1; // Value of the coin
    [SerializeField] private AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CoinManager.Instance.AddCoins(coinValue);
            SoundManager.Instance.PlaySound(collectSound);
            gameObject.SetActive(false); // Remove the coin from the scene
        }
    }
}
