using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("Timers")]
    [SerializeField] private float activationTime;
    [SerializeField] private float activeTime;

    [Header("Damage")]
    [SerializeField] private float damage;

    private Animator anim;
    private SpriteRenderer spriteRend;

    private bool trigger;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(!trigger)
            {
                StartCoroutine(ActivateTrap());
            }

            if(active)
            {
                collision.GetComponent<Health>().TakeDamage(damage);
            }
        }
    }

    private IEnumerator ActivateTrap()
    {
        trigger = true;
        spriteRend.color = Color.red;
        yield return new WaitForSeconds(activationTime);
        spriteRend.color = Color.white;
        active = true;
        anim.SetBool("Activated", true);

        yield return new WaitForSeconds(activeTime);
        active = false;
        trigger = false;
        anim.SetBool("Activated", false);
    }
}
