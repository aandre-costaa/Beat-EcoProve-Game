using UnityEngine;
using UnityEngine.XR;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = 9999999;
    [SerializeField]private float attackColldown;
    [SerializeField]private Transform firePoint;
    [SerializeField]private GameObject[] projectiles;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    public void HandleAttack(){
        if(playerMovement.canAttack() && cooldownTimer > attackColldown){
            Attack();
        }
    }

    private void Attack(){
        cooldownTimer = 0;
        anim.SetTrigger("Attack");

        //Array de projéteis permite poupar recursos ao reutilizar projéteis que já foram criados
        projectiles[FindProjectile()].transform.position = firePoint.position;
        projectiles[FindProjectile()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindProjectile(){
        for(int i = 0; i < projectiles.Length; i++){
            if(!projectiles[i].activeInHierarchy){
                return i;
            }
        }
        return 0;
    }
}
