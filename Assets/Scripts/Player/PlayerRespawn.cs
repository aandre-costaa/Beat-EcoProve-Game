using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    //[SerializeField] private AudioClip checkpointSound;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private PlayerMovement PlayerMovement;
    private UIManager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        PlayerMovement = GetComponent<PlayerMovement>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public void CheckRespawn()
    {
        if(currentCheckpoint == null)
        {
            uiManager.GameOver();
            return;
        }

        transform.position = currentCheckpoint.position;
        playerHealth.Respawn();
        //PlayerMovement.AllowMovement(true);
        //PlayerMovement.HandleMovement();
        //Camera.main.GetComponent<PlayerMovement>().AllowMovement(true);
        GetComponent<PlayerMovement>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Checkpoint")
        {
            currentCheckpoint = collision.transform;
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("appear");


        }
    }
}
