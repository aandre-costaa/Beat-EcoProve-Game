using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float maxHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool isDead;

    [Header ("iFrames")]
    [SerializeField] private float invincibilityTime;
    [SerializeField] private float numFlashes;
    private SpriteRenderer spriteRend;

    private void Awake()
    {   
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        if (currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
            StartCoroutine(Invincibility());
        }else {
            if (!isDead)
            {   
                anim.SetTrigger("Die");
                GetComponent<PlayerMovement>().enabled = false;
                isDead = true;
            }
        }
    }

    public void Heal(float healValue)
    {
        currentHealth = Mathf.Clamp(currentHealth + healValue, 0, maxHealth);
    }

    private IEnumerator Invincibility(){
        Physics2D.IgnoreLayerCollision(8, 9, true);
        for (int i = 0; i < numFlashes; i++)
        {
            spriteRend.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(invincibilityTime / (numFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(invincibilityTime / (numFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }
}
