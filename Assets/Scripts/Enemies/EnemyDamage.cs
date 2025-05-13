using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected float damage;

    [SerializeField] public AudioClip damageSound;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SoundManager.Instance.PlaySound(damageSound);
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
