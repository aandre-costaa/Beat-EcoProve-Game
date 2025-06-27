using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnswerButtonCuriosidade : MonoBehaviour
{
    private bool isCorrect;
    private Color defaultColor = Color.white; // Default button color
    private Color correctColor = new Color(57f / 255f, 161f / 255f, 83f / 255f); // Green color
    private Color incorrectColor = new Color(164f / 255f, 23f / 255f, 13f / 255f); // Red color
    private Image colors;

    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] public QuestionSetupCuriusidade questionSetupCuriusidade; // Reference to QuestionSetupCuriusidade

    [Header("Sounds")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    private Button button;

    void Start()
    {
        colors = GetComponent<Image>();
        button = GetComponent<Button>();
        ResetColor();

        // Tentar encontrar automaticamente o QuestionSetupCuriusidade se não foi definido
        if (questionSetupCuriusidade == null)
        {
            questionSetupCuriusidade = FindObjectOfType<QuestionSetupCuriusidade>();
        }
    }

    public void SetAnswerText(string newText)
    {
        answerText.text = newText;
    }

    public void SetIsCorrect(bool newBool)
    {
        isCorrect = newBool;
    }

    public void OnClick()
    {
        if (!button.interactable) return; // Prevent multiple clicks

        if (isCorrect)
        {
            Debug.Log("Correct!");
            colors.color = correctColor;

            if (questionSetupCuriusidade != null)
            {
                questionSetupCuriusidade.OnCorrectAnswer(this); // Pass this button
            }

            // Play correct sound
            if (SoundManager.Instance != null && correctSound != null)
            {
                SoundManager.Instance.PlaySound(correctSound);
            }
        }
        else
        {
            Debug.Log("Incorrect!");
            colors.color = incorrectColor;

            if (questionSetupCuriusidade != null)
            {
                questionSetupCuriusidade.OnIncorrectAnswer();
            }

            // Play wrong sound
            if (SoundManager.Instance != null && wrongSound != null)
            {
                SoundManager.Instance.PlaySound(wrongSound);
            }
        }

        // Disable all buttons after clicking
        if (questionSetupCuriusidade != null)
        {
            questionSetupCuriusidade.DisableAllAnswerButtons();
        }
    }

    public void ResetColor()
    {
        if (colors != null)
        {
            colors.color = defaultColor; // Reset to default
        }
    }

    public void DisableButton()
    {
        if (button != null)
        {
            button.interactable = false; // Disable button interaction
        }
    }

    public void EnableButton()
    {
        if (button != null)
        {
            button.interactable = true;
        }
    }
}
