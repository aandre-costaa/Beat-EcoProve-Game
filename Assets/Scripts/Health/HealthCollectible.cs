using UnityEngine;

public class HealthCollectible : MonoBehaviour
{

    [SerializeField] private float healthValue;
    [SerializeField] private AudioClip healSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().Heal(healthValue);
    	    SoundManager.Instance.PlaySound(healSound);
            gameObject.SetActive(false);
        }
    }
}
