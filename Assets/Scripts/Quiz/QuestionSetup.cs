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
    private Health health;

    private QuestionData currentQuestion;
    private int currentDoorIndex = 0;
    private int currentQuestionIndex = 0;

    private const int TotalQuestionsPerLevel = 8; // Total unique questions per level
    private const int QuestionsPerDoor = 2;

    private bool isInitialized = false;

    [SerializeField] private TextMeshProUGUI timerText; // Display timer
    private float remainingTime; // Time left for the current question
    private Coroutine timerCoroutine;

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
        //doorQuestionsMap = new Dictionary<int, List<QuestionData>>();
        //int questionCounter = 0;

        //for (int doorIndex = 0; doorIndex < allQuestions.Count / QuestionsPerDoor; doorIndex++)
        //{
        //    List<QuestionData> questionsForDoor = allQuestions.Skip(questionCounter).Take(QuestionsPerDoor).ToList();
        //    doorQuestionsMap[doorIndex] = questionsForDoor;
        //    questionCounter += QuestionsPerDoor;
        //}
        doorQuestionsMap = new Dictionary<int, List<QuestionData>>();
        remainingQuestionsPool = new List<QuestionData>(allQuestions); // Copy all questions to a temporary pool

        int doorCount = 4/* Set the total number of doors here, e.g., based on your game setup */;
        int doorIndex = 0;

        // Allocate questions to each door
        while (doorIndex < doorCount)
        {
            if (remainingQuestionsPool.Count >= QuestionsPerDoor)
            {
                // Allocate a full set of questions for this door
                List<QuestionData> questionsForDoor = remainingQuestionsPool.Take(QuestionsPerDoor).ToList();
                remainingQuestionsPool.RemoveRange(0, QuestionsPerDoor); // Remove allocated questions
                doorQuestionsMap[doorIndex] = questionsForDoor;
            }
            else if (remainingQuestionsPool.Count > 0)
            {
                // Allocate remaining questions to the last door
                doorQuestionsMap[doorIndex] = new List<QuestionData>(remainingQuestionsPool);
                remainingQuestionsPool.Clear();
            }
            else
            {
                // No more questions to allocate
                doorQuestionsMap[doorIndex] = new List<QuestionData>(); // Empty list for this door
            }

            Debug.Log($"Door {doorIndex} allocated {doorQuestionsMap[doorIndex].Count} questions.");
            doorIndex++;
        }
    }

    public void StartQuizForDoor(int doorIndex)
    {
        //Debug.Log("DOOR INDEX: " + doorIndex);
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

            if (float.TryParse(currentQuestion.tempoLimite, out float parsedTime))
            {
                StartTimer(parsedTime);
            }
            else
            {
                Debug.LogWarning($"Invalid tempoLimite format: {currentQuestion.tempoLimite}");
                StartTimer(30f); // Default to 30 seconds if parsing fails
            }
        }
        else
        {
            Debug.Log("All questions for this door have been answered.");
            quizCanvas.SetActive(false);
        }
    }


    private void StartTimer(float timeLimit)
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        remainingTime = timeLimit;
        timerCoroutine = StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerDisplay();
            yield return null;
        }

        TimerExpired();
    }

    private void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(remainingTime);
        timerText.text = $"Time: {seconds}s"; // Update UI
    }

    private void TimerExpired()
    {
        Debug.Log("Time's up! Moving to the next question.");
        OnIncorrectAnswer(); // Treat as incorrect
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
            PrepareNextDoor();
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
        Health playerHealth = FindObjectOfType<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1); // Adjust the damage value as needed
            if (!playerHealth.isDead)
            {
                DisableAllAnswerButtons();
                StartCoroutine(DelayNextQuestion(2f));

            }
            else
            {
                quizCanvas.SetActive(false);
            }
        }
        //DisableAllAnswerButtons();
        //StartCoroutine(DelayNextQuestion(2f));
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