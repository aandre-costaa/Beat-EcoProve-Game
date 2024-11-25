using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    private Vector3[] initialPosition;

    private void Awake()
    {
        initialPosition = new Vector3[enemies.Length]; //Inicializa o array com o tamanho do array de inimigos
        for (int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null)
                initialPosition[i] = enemies[i].transform.position;
        }
    }
    public void ActivateRoom(bool status)
    {
        print("Room activated: " + status);
        for (int i = 0; i < enemies.Length; i++) //Ativa ou desativa os inimigos da sala
        {
            if (enemies[i] != null)
            {
                enemies[i].SetActive(status);
                enemies[i].transform.position = initialPosition[i];
            }
        }
    }
}