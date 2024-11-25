using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image maxHealth;
    [SerializeField] private Image currentHealth;

    private void Start()
    {
        maxHealth.fillAmount = playerHealth.currentHealth / 10;
    }

    private void Update()
    {
        currentHealth.fillAmount = playerHealth.currentHealth / 10;
    }
}
