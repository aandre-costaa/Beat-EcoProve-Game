using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class QuestionSetup : MonoBehaviour
{
    [SerializeField] public List<QuestionData> allQuestions; // All questions fetched from DB
    private List<QuestionData> remainingQuestionsPool; // Temporary pool of remaining questions
    private Dictionary<int, List<QuestionData>> doorQuestionsMap; // Maps door index to its questions
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private AnswerButton[] answerButtons;
    [SerializeField] private GameObject quizCanvas;
    private CoinManager coinManager;


    private QuestionData currentQuestion;
    private int currentDoorIndex = 0;
    private int currentQuestionIndex = 0;

    private const int TotalQuestionsPerLevel = 8; // Total unique questions per level
    private const int QuestionsPerDoor = 4;

    private bool isInitialized = false;

    private async void OnEnable()
    {
        if (!isInitialized)
        {
            await InitializeQuiz();
            isInitialized = true;
        }

        StartQuizForDoor(currentDoorIndex);
    }

    private async System.Threading.Tasks.Task InitializeQuiz()
    {
        QuestionFetcher fetcher = new QuestionFetcher();
        allQuestions = await fetcher.FetchQuestionsAsync();

        if (allQuestions.Count >= QuestionsPerDoor)
        {
            allQuestions = ShuffleQuestions(allQuestions);
            AllocateQuestionsToDoors();
        }
        else
        {
            Debug.LogWarning($"Not enough questions fetched. Found: {allQuestions.Count}");
            AllocateQuestionsToDoors(); // Allocate what is available
        }

        quizCanvas.SetActive(false);
    }

    private void AllocateQuestionsToDoors()
    {
        doorQuestionsMap = new Dictionary<int, List<QuestionData>>();
        int questionCounter = 0;

        for (int doorIndex = 0; doorIndex < allQuestions.Count / QuestionsPerDoor; doorIndex++)
        {
            List<QuestionData> questionsForDoor = allQuestions.Skip(questionCounter).Take(QuestionsPerDoor).ToList();
            doorQuestionsMap[doorIndex] = questionsForDoor;
            questionCounter += QuestionsPerDoor;
        }
    }

    public void StartQuizForDoor(int doorIndex)
    {
        if (!doorQuestionsMap.ContainsKey(doorIndex) || doorQuestionsMap[doorIndex].Count == 0)
        {
            Debug.LogError($"Door {doorIndex} has no allocated questions.");
            quizCanvas.SetActive(false);
            return;
        }

        currentDoorIndex = doorIndex;
        currentQuestionIndex = 0;
        LoadCurrentQuestion(doorIndex);
    }

    private void AllocateQuestionsToDoor(int doorIndex)
    {
        if (remainingQuestionsPool.Count >= QuestionsPerDoor)
        {
            var questionsForDoor = remainingQuestionsPool.Take(QuestionsPerDoor).ToList();
            remainingQuestionsPool.RemoveRange(0, QuestionsPerDoor);
            doorQuestionsMap[doorIndex] = questionsForDoor;
        }
        else
        {
            Debug.LogWarning($"Not enough questions left to allocate for door {doorIndex}");
            doorQuestionsMap[doorIndex] = new List<QuestionData>(remainingQuestionsPool);
            remainingQuestionsPool.Clear();
        }
    }

    private void LoadCurrentQuestion(int doorIndex)
    {
        if (currentQuestionIndex < doorQuestionsMap[doorIndex].Count)
        {
            currentQuestion = doorQuestionsMap[doorIndex][currentQuestionIndex];
            SetQuestionValues();
            SetAnswerValues();
        }
        else
        {
            Debug.Log("All questions for this door have been answered.");
            quizCanvas.SetActive(false);
        }
    }

    private void SetQuestionValues()
    {
        questionText.text = currentQuestion.question;
        categoryText.text = currentQuestion.category;
    }

    private void SetAnswerValues()
    {
        ResetAnswerButtons();

        List<string> answers = RandomizeAnswers(new List<string>(currentQuestion.answers));

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool isCorrect = answers[i] == currentQuestion.correctAnswer;
            answerButtons[i].SetIsCorrect(isCorrect);
            answerButtons[i].SetAnswerText(answers[i]);
            answerButtons[i].questionSetup = this;
        }
    }

    private void ResetAnswerButtons()
    {
        foreach (var button in answerButtons)
        {
            button.ResetColor();
            button.SetIsCorrect(false);
        }
    }

    private List<string> RandomizeAnswers(List<string> originalList)
    {
        return originalList.OrderBy(_ => Random.value).ToList();
    }

    private List<QuestionData> ShuffleQuestions(List<QuestionData> questions)
    {
        return questions.OrderBy(_ => Random.value).ToList();
    }

    public void OnCorrectAnswer(AnswerButton selectedButton)
    {
        CoinManager.Instance.AddCoins(1);
        DisableOtherButtons(selectedButton);
        StartCoroutine(DelayNextQuestion(2f));
    }

    private IEnumerator DelayNextQuestion(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        currentQuestionIndex++;
        if (currentQuestionIndex < doorQuestionsMap[currentDoorIndex].Count)
        {
           
            LoadCurrentQuestion(currentDoorIndex);
            EnableAllButtons(); // Enable all buttons for the next question
        }
        else
        {
            //PrepareNextDoor();
            EndQuiz();
            EnableAllButtons();
        }
    }

    public void PrepareNextDoor()
    {
        Debug.Log("TESE!");
        currentDoorIndex++;
        StartQuizForDoor(currentDoorIndex);
    }

    public void OnIncorrectAnswer()
    {
        Debug.Log("Incorrect answer!");
        DisableAllAnswerButtons();
        StartCoroutine(DelayNextQuestion(2f));
    }

    public void DisableOtherButtons(AnswerButton selectedButton)
    {
        foreach (var button in answerButtons)
        {
            if (button != selectedButton) // Disable all except the selected button
            {
                button.DisableButton();
            }
        }
    }

    public void EnableAllButtons()
    {
        foreach (var button in answerButtons)
        {
            button.ResetColor(); // Reset to default appearance
            button.EnableButton();
        }
    }
    public void DisableAllAnswerButtons()
    {
        foreach (var button in answerButtons)
        {
            button.DisableButton();
        }
    }

    private void EndQuiz()
    {
        Debug.Log("Quiz finished!");

        if (quizCanvas != null)
        {
            quizCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("QuizCanvas is not assigned in the inspector.");
        }
    }

}