using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionSetup : MonoBehaviour
{
    [SerializeField] public List<QuestionData> questions;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private AnswerButton[] answerButtons;
    [SerializeField] private int correctAnswerChoice;

    private QuestionData currentQuestion;
    
    private void Awake()
    {
        // Chamar todas as perguntas
        GetQuestionAssets();
    }

    // Start is called before the first frame update
    public void Start()
    {
        SelectNewQuestion();
        // Chamar função para definir campos da pergunta
        SetQuestionValues();
        // Definir campos das respostas
        SetAnswerValues();
    }

    private void GetQuestionAssets()
    {
        // Para já, vamos buscar todas as perguntas que existem numa pasta no projeto
        questions = new List<QuestionData>(Resources.LoadAll<QuestionData>("Questions"));
    }

    private void SelectNewQuestion()
    {
        // **Randomização da ordem das perguntas**
        int randomQuestionIndex = Random.Range(0, questions.Count);

        // Atribuir o index à perguta atual
        currentQuestion = questions[randomQuestionIndex];

        // Remover a pergunta da lista para não ser repetida
        questions.RemoveAt(randomQuestionIndex);
    }

    private void SetQuestionValues()
    {
        // Texto da Pergunta apenas
        questionText.text = currentQuestion.question;

        // Categoria da pergunta
        categoryText.text = currentQuestion.category;
    }

    private void SetAnswerValues()
    {
        // Randimização da ordem das respostas
        List<string> answers = RandomizeAnswers(new List<string>(currentQuestion.answers));

        // Set up the answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            // Variável temporária para definir se a resposta é correta
            bool isCorrect = false;

            // quando encontra o index da resposta correta, define a resposta como correta
            if(i == correctAnswerChoice)
            {
                isCorrect = true;
            }

            answerButtons[i].SetIsCorrect(isCorrect);
            //Injeção do texto para a opção de resposta
            answerButtons[i].SetAnswerText(answers[i]);
        }
    }

    // LÓGICA DE RANDOMIZAÇÃO DA ORDEM DAS RESPOSTAS
    private List<string> RandomizeAnswers(List<string> originalList)
    {
        // True quando se encontra a resposta correta
        bool correctAnswerChosen = false;

        // Lista temporária para guardar a ordem de respostas
        List<string> newList = new List<string>();

        Debug.Log("Original List: " + originalList.Count);
        Debug.Log("Lenght: " + answerButtons.Length);
        //answerButtons vem dos botoes de resposta adicionados no QuestionData (/Resources/Questions)
        for(int i = 0; i < answerButtons.Length; i++)
        {
            // Randomiza um valor consoante os existentes na lista
            int random = Random.Range(0, originalList.Count);

            // Uma vez que a resposta correta esta sempre na primeira posição, se a resposta correta já foi definida não faz nada
            if(random == 0 && !correctAnswerChosen)
            {
                correctAnswerChoice = i;
                correctAnswerChosen = true;
            }

            // Adiciona a resposta randomizada à lista temporária
            newList.Add(originalList[random]);
            //Remove a resposta da lista original para não ser repetida
            originalList.RemoveAt(random);  
        }


        return newList;
    }
}