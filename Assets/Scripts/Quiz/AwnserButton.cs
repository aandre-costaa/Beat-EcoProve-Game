using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    private bool isCorrect;
    private Color defaultColor = Color.white; // Default button color
    private Color correctColor = new Color(57f / 255f, 161f / 255f, 83f / 255f); // Green color
    private Color incorrectColor = new Color(164f / 255f, 23f / 255f, 13f / 255f); // Red color
    private Image colors;

    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] public QuestionSetup questionSetup; // Reference to QuestionSetup

    [Header("Sounds")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    private Button button;

    void Start()
    {
        colors = GetComponent<Image>();
        button = GetComponent<Button>();
        ResetColor();
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
            questionSetup.OnCorrectAnswer(this); // Pass this button to lock others
            SoundManager.Instance.PlaySound(correctSound); // Play correct sound
        }
        else
        {
            Debug.Log("Incorrect!");
            colors.color = incorrectColor;
            questionSetup.OnIncorrectAnswer();
            SoundManager.Instance.PlaySound(wrongSound);
        }

        questionSetup.DisableAllAnswerButtons();
    }

    public void ResetColor()
    {
        colors.color = defaultColor; // Reset to default
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