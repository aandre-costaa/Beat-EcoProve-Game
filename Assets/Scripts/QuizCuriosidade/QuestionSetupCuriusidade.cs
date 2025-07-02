using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionSetupCuriusidade : MonoBehaviour
{
    [SerializeField] public List<QuestionData> allQuestions; // All questions fetched from DB
    private bool isInitialized = false;
    [SerializeField] private GameObject quizCanvas;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private AnswerButtonCuriosidade[] answerButtons; // Mudança para AnswerButtonCuriosidade
    [SerializeField] private TextMeshProUGUI timerText; // Display timer
    [SerializeField] private RectTransform questionPanel; // Container da pergunta para redimensionamento

    [Header("Quiz Activation Settings")]
    [SerializeField] private string playerPrefsKey = "ShowCuriosidadeQuiz"; // Chave do PlayerPrefs
    [SerializeField] private GameObject quizMainObject; // Referência ao GameObject Quiz principal

    private QuestionData currentQuestion;
    private float timeRemaining;
    private bool isQuizActive = false;
    private bool hasAnswered = false; // Flag para verificar se já respondeu
    private Coroutine timerCoroutine; // Referência para o coroutine do timer
    private bool isShowingCuriosidade = false; // Flag para saber se está mostrando curiosidade
    private ContentSizeFitter contentSizeFitter; // Para redimensionamento automático

    private async void Start()
    {
        // Configurar redimensionamento automático
        SetupDynamicTextSize();

        // Se não foi definido manualmente, encontrar o GameObject Quiz pai
        if (quizMainObject == null)
        {
            quizMainObject = GameObject.Find("Quiz");
            if (quizMainObject == null)
            {
                // Tentar encontrar pelo transform pai
                Transform parent = transform.parent;
                while (parent != null)
                {
                    if (parent.name == "Quiz")
                    {
                        quizMainObject = parent.gameObject;
                        break;
                    }
                    parent = parent.parent;
                }
            }
        }

        // Verificar se deve mostrar o quiz baseado no PlayerPrefs
        if (!ShouldShowQuiz())
        {
            Debug.Log("Quiz não deve ser mostrado - desativando GameObject Quiz");
            if (quizMainObject != null)
            {
                quizMainObject.SetActive(false);
            }
            return;
        }

        if (!isInitialized)
        {
            await InitializeQuiz();
            isInitialized = true;
        }
    }

    private void SetupDynamicTextSize()
    {
        // Se questionPanel não foi definido, tentar encontrar automaticamente
        if (questionPanel == null)
        {
            // Procurar pelo pai do questionText
            if (questionText != null)
            {
                questionPanel = questionText.transform.parent.GetComponent<RectTransform>();
            }
        }

        if (questionPanel != null)
        {
            // Adicionar Content Size Fitter se não existir
            if (questionPanel.GetComponent<ContentSizeFitter>() == null)
            {
                contentSizeFitter = questionPanel.gameObject.AddComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                Debug.Log("Content Size Fitter adicionado ao questionPanel");
            }
            else
            {
                contentSizeFitter = questionPanel.GetComponent<ContentSizeFitter>();
            }
        }

        // Configurar quebra de linha no texto
        if (questionText != null)
        {
            questionText.enableWordWrapping = true;
            questionText.overflowMode = TextOverflowModes.Overflow;
        }
    }

    private async void OnEnable()
    {
        // Verificação adicional quando o componente é ativado
        if (!ShouldShowQuiz())
        {
            Debug.Log("Quiz não deve ser mostrado - desativando GameObject Quiz no OnEnable");
            if (quizMainObject != null)
            {
                quizMainObject.SetActive(false);
            }
            return;
        }

        if (isInitialized)
        {
            StartSingleQuestionQuiz();
        }
    }

    private bool ShouldShowQuiz()
    {
        // Verificar se a chave existe no PlayerPrefs e se está como true
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            string showQuizValue = PlayerPrefs.GetString(playerPrefsKey, "false");
            bool shouldShow = showQuizValue.ToLower() == "true";

            Debug.Log($"Verificando PlayerPrefs - Chave: {playerPrefsKey}, Valor: {showQuizValue}, Deve mostrar: {shouldShow}");
            return shouldShow;
        }
        else
        {
            Debug.Log($"Chave {playerPrefsKey} não encontrada no PlayerPrefs - Quiz não será mostrado");
            return false;
        }
    }

    public void ActivateQuiz()
    {
        PlayerPrefs.SetString(playerPrefsKey, "true");
        PlayerPrefs.Save();
        Debug.Log($"Quiz ativado - {playerPrefsKey} definido como true no PlayerPrefs");

        if (quizMainObject != null && !quizMainObject.activeInHierarchy)
        {
            quizMainObject.SetActive(true);
        }
    }

    public void DeactivateQuiz()
    {
        PlayerPrefs.SetString(playerPrefsKey, "false");
        PlayerPrefs.Save();
        Debug.Log($"Quiz desativado - {playerPrefsKey} definido como false no PlayerPrefs");

        if (quizMainObject != null && quizMainObject.activeInHierarchy)
        {
            quizMainObject.SetActive(false);
        }
    }

    private async System.Threading.Tasks.Task InitializeQuiz()
    {
        CanvasGroup canvasGroup = quizCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = quizCanvas.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Fetch questions and prepare quiz
        QuestionFetcher fetcher = new QuestionFetcher();
        allQuestions = await fetcher.FetchQuestionsCuriousidadeAsync();

        if (allQuestions.Count > 0)
        {
            allQuestions = ShuffleQuestions(allQuestions);
        }
        else
        {
            Debug.LogWarning($"No questions fetched. Found: {allQuestions.Count}");
        }

        quizCanvas.SetActive(false);

        // Iniciar o quiz após a inicialização
        StartSingleQuestionQuiz();
    }

    private List<QuestionData> ShuffleQuestions(List<QuestionData> questions)
    {
        return questions.OrderBy(_ => Random.value).ToList();
    }

    public void StartSingleQuestionQuiz()
    {
        // Verificação adicional antes de iniciar
        if (!ShouldShowQuiz())
        {
            Debug.Log("Tentativa de iniciar quiz bloqueada - desativando GameObject Quiz");
            if (quizMainObject != null)
            {
                quizMainObject.SetActive(false);
            }
            return;
        }

        if (allQuestions == null || allQuestions.Count == 0)
        {
            Debug.LogError("No questions available to start quiz!");
            return;
        }

        // Reset flags
        hasAnswered = false;
        isShowingCuriosidade = false;

        // Show quiz canvas
        quizCanvas.SetActive(true);
        CanvasGroup canvasGroup = quizCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Display single random question
        DisplaySingleQuestion();
    }

    private void DisplaySingleQuestion()
    {
        // Pegar uma pergunta aleatória
        currentQuestion = allQuestions[Random.Range(0, allQuestions.Count)];

        // Set question text and category
        questionText.text = currentQuestion.question;
        categoryText.text = currentQuestion.category;

        // Forçar atualização do layout após mudança do texto
        ForceLayoutUpdate();

        // Set up True/False buttons
        SetupTrueFalseButtons();

        // Start timer
        if (float.TryParse(currentQuestion.tempoLimite, out float timeLimit))
        {
            timeRemaining = timeLimit;
            isQuizActive = true;
            timerCoroutine = StartCoroutine(CountdownTimer());
        }
        else
        {
            timeRemaining = 20f; // Default time
            isQuizActive = true;
            timerCoroutine = StartCoroutine(CountdownTimer());
        }

        // Enable all answer buttons
        EnableAllAnswerButtons();
    }

    private void SetupTrueFalseButtons()
    {
        // For True/False questions, we only need 2 buttons
        if (answerButtons.Length >= 2)
        {
            // Debug logs para verificar os dados
            Debug.Log($"Current question: {currentQuestion.question}");
            Debug.Log($"Correct answer: {currentQuestion.correctAnswer}");
            Debug.Log($"Curiosidade recebida: '{currentQuestion.curiosidade}'");

            // Sempre configurar botões True/False independentemente do array answers
            string[] trueFalseOptions = { "True", "False" };
            bool[] isCorrectArray = new bool[2];

            // Determine which is correct based on correctAnswer
            bool correctIsTrue = currentQuestion.correctAnswer.ToLower() == "true";
            isCorrectArray[0] = correctIsTrue;  // True button
            isCorrectArray[1] = !correctIsTrue; // False button

            Debug.Log($"Correct is True: {correctIsTrue}");
            Debug.Log($"Button 0 (True) is correct: {isCorrectArray[0]}");
            Debug.Log($"Button 1 (False) is correct: {isCorrectArray[1]}");

            // Randomize button positions
            if (Random.Range(0, 2) == 1)
            {
                // Swap positions
                string tempAnswer = trueFalseOptions[0];
                trueFalseOptions[0] = trueFalseOptions[1];
                trueFalseOptions[1] = tempAnswer;

                bool tempCorrect = isCorrectArray[0];
                isCorrectArray[0] = isCorrectArray[1];
                isCorrectArray[1] = tempCorrect;
            }

            // Set up buttons
            answerButtons[0].SetAnswerText(trueFalseOptions[0]);
            answerButtons[0].SetIsCorrect(isCorrectArray[0]);
            answerButtons[0].gameObject.SetActive(true);

            answerButtons[1].SetAnswerText(trueFalseOptions[1]);
            answerButtons[1].SetIsCorrect(isCorrectArray[1]);
            answerButtons[1].gameObject.SetActive(true);

            Debug.Log($"Button 0 text: {trueFalseOptions[0]}, is correct: {isCorrectArray[0]}");
            Debug.Log($"Button 1 text: {trueFalseOptions[1]}, is correct: {isCorrectArray[1]}");

            // Hide other buttons if they exist
            for (int i = 2; i < answerButtons.Length; i++)
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Not enough answer buttons for True/False questions!");
        }
    }

    private IEnumerator CountdownTimer()
    {
        while (timeRemaining > 0 && isQuizActive && !hasAnswered)
        {
            timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        if (isQuizActive && !hasAnswered)
        {
            // Time's up
            timerText.text = "Time: 0";
            OnTimeUp();
        }
    }

    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            Debug.Log("Timer stopped!");
        }
    }

    private void OnTimeUp()
    {
        isQuizActive = false;
        hasAnswered = true;
        DisableAllAnswerButtons();
        Debug.Log("Time's up!");

        StartCoroutine(ShowTimeUpAndCuriosidade());
    }

    private IEnumerator ShowTimeUpAndCuriosidade()
    {
        timerText.text = "Time's Up!";
        yield return new WaitForSeconds(2f);

        // Mostrar curiosidade por 4 segundos
        ShowCuriosidade();
        yield return new WaitForSeconds(4f);

        EndQuiz();
    }

    public void OnCorrectAnswer(AnswerButtonCuriosidade correctButton)
    {
        if (hasAnswered) return; // Evitar múltiplas respostas

        isQuizActive = false;
        hasAnswered = true;
        Debug.Log("Correct answer selected!");

        // Parar o timer imediatamente
        StopTimer();

        // Mostrar resultado e depois curiosidade
        StartCoroutine(ShowResultAndCuriosidade(true));
    }

    public void OnIncorrectAnswer()
    {
        if (hasAnswered) return; // Evitar múltiplas respostas

        isQuizActive = false;
        hasAnswered = true;
        Debug.Log("Incorrect answer selected!");

        // Parar o timer imediatamente
        StopTimer();

        // Mostrar resultado e depois curiosidade
        StartCoroutine(ShowResultAndCuriosidade(false));
    }

    private IEnumerator ShowResultAndCuriosidade(bool wasCorrect)
    {
        // Mostrar resultado por 2 segundos
        timerText.text = wasCorrect ? "Correct!" : "Incorrect!";
        yield return new WaitForSeconds(2f);

        // Mostrar curiosidade por 4 segundos
        ShowCuriosidade();
        yield return new WaitForSeconds(4f);

        EndQuiz();
    }

    private void ShowCuriosidade()
    {
        isShowingCuriosidade = true;

        // Substituir o texto da pergunta pela curiosidade
        if (!string.IsNullOrEmpty(currentQuestion.curiosidade) && !string.IsNullOrWhiteSpace(currentQuestion.curiosidade))
        {
            questionText.text = currentQuestion.curiosidade;
            Debug.Log("Mostrando curiosidade: " + currentQuestion.curiosidade);
        }
        else
        {
            questionText.text = "Curiosidade não disponível para esta pergunta.";
            Debug.LogWarning("Curiosidade não disponível ou vazia");
        }

        // Forçar atualização do layout após mudança do texto
        ForceLayoutUpdate();

        // Esconder os botões de resposta
        foreach (AnswerButtonCuriosidade button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Mostrar texto indicativo no timer
        timerText.text = "Did you know?";
    }

    private void ForceLayoutUpdate()
    {
        // Forçar atualização do layout para redimensionamento automático
        if (questionPanel != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(questionPanel);
        }
    }

    private void EndQuiz()
    {
        Debug.Log("Quiz completed!");
        isQuizActive = false;
        hasAnswered = false;
        isShowingCuriosidade = false;

        // Garantir que o timer está parado
        StopTimer();

        // Desativar o quiz após mostrar a curiosidade
        DeactivateQuiz();

        // Hide quiz canvas
        CanvasGroup canvasGroup = quizCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        quizCanvas.SetActive(false);
    }

    public void DisableAllAnswerButtons()
    {
        foreach (AnswerButtonCuriosidade button in answerButtons)
        {
            button.DisableButton();
        }
    }

    public void EnableAllAnswerButtons()
    {
        foreach (AnswerButtonCuriosidade button in answerButtons)
        {
            if (button.gameObject.activeInHierarchy)
            {
                button.EnableButton();
                button.ResetColor();
            }
        }
    }
}
