using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    private bool isCorrect;
    private Color correctColor = new Color(57f / 255f, 161f / 255f, 83f / 255f); // Green color
    private Color incorrectColor = new Color(164f / 255f, 23f / 255f, 13f / 255f); // Red color
    private Image colors;
    [SerializeField] private TextMeshProUGUI answerText;

    // Sucessão de questões, a fazer
    // [SerializeField] private QuestionSetup questionSetup;

    void Start()
    {
        colors = GetComponent<Image>();
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
        if(isCorrect)
        {
            Debug.Log("CORRETA!!!");
            colors.color = correctColor;
        }
        else
        {
            Debug.Log("ERRADA!!!");
            colors.color = incorrectColor;
        }

        // Sucessão de questões, a fazer
        // if (questionSetup.questions.Count > 0)
        // {
        //     // Generate a new question
        //     questionSetup.Start();
        // }
    }
}