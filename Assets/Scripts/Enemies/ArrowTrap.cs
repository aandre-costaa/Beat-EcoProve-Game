using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float cooldown;
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject[] arrow;

    private float cooldownTimer;

    private void Attack(){
        cooldownTimer = 0;

        arrow[FindProjectile()].transform.position = firepoint.position;
        arrow[FindProjectile()].GetComponent<EnemiyProjectile>().ActivateProjectile();
    }

    private int FindProjectile(){
        for(int i = 0; i < arrow.Length; i++){
            if(!arrow[i].activeInHierarchy){
                return i;
            }
        }
        return 0;
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if(cooldownTimer >= cooldown)
        {
            Attack();
        }
    }
}
