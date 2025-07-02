using System.Collections;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("Timers")]
    [SerializeField] private float activationTime;
    [SerializeField] private float activeTime;

    [Header("Damage")]
    [SerializeField] private float damage;

    [Header("Sounds")]
    [SerializeField] private AudioClip activationSound;

    private Animator anim;
    private SpriteRenderer spriteRend;

    private bool active;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        StartCoroutine(ActivateTrapLoop());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && active)
        {
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }

    private IEnumerator ActivateTrapLoop()
    {
        while (true)
        {
            spriteRend.color = Color.red;
            yield return new WaitForSeconds(activationTime);

            spriteRend.color = Color.white;
            active = true;
            anim.SetBool("Activated", true);
            PlayTrapSound();

            yield return new WaitForSeconds(activeTime);

            active = false;
            anim.SetBool("Activated", false);
        }
    }

    private void PlayTrapSound()
    {
        if (SoundManager.Instance != null && activationSound != null)
        {
            SoundManager.Instance.PlaySound(activationSound);
        }
    }
}
