using UnityEngine;

public class Spikehead : EnemyDamage
{
    [Header("Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;

    private Vector3[] directions = new Vector3[4];
    private Vector3 destination;

    private float checkTimer;
    private bool attack;

    private void OnEnable()
    {
        Stop();
    }

    private void Update()
    {
        if (attack)
            transform.Translate(destination * Time.deltaTime * speed);
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
                CheckForPlayer();
        }
    }
    private void CheckForPlayer()
    {
        CalculateDirections();

        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red); //Visualização dos raios que detetam player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

            if (hit.collider != null && !attack)
            {
                attack = true;
                destination = directions[i];
                checkTimer = 0;
            }
        }
    }
    private void CalculateDirections()
    {
        directions[0] = transform.right * range; //Direita
        directions[1] = -transform.right * range; //Esquerda
        directions[2] = transform.up * range; //Cima
        directions[3] = -transform.up * range; //Baixo
    }
    private void Stop()
    {
        destination = transform.position;
        attack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        Stop();
    }
}
