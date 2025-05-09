using UnityEngine;

public class EnemiyProjectile : EnemyDamage
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    [SerializeField] private AudioClip hitSound;

    private float lifeTime;


    public void ActivateProjectile(){
        lifeTime = 0;
        gameObject.SetActive(true);

    }

    private void Update()
    {
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifeTime += Time.deltaTime;

        if(lifeTime > resetTime){
            gameObject.SetActive(false);
        }
    }

    private new void OnTriggerEnter2D(Collider2D collision){
        base.OnTriggerEnter2D(collision);
        SoundManager.Instance.PlaySound(hitSound);
        gameObject.SetActive(false);
    }
}
