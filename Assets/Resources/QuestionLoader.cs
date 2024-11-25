using UnityEngine;
using UnityEditor;
using System.IO;

public class QuestionLoader
{
    private static string questionsCSVPath = "/Resources/QuizResources/questoes_teste.csv";
    private static string questionsPath = "Assets/Resources/Questions/";
    private static int numberOfAnswers = 4;

    [MenuItem("Utilities/Generate Questions")]
    public static void GenerateQuestions()
    {

        string[] allLines = File.ReadAllLines(Application.dataPath + questionsCSVPath);

        foreach (string s in allLines)
        {
            //Separa cada string até encontrar a primeira ,
            string[] splitData = s.Split(',');

            QuestionData questionData = ScriptableObject.CreateInstance<QuestionData>();
            questionData.question = splitData[1]; // N da coluna onde esta a pergunta
            questionData.category = splitData[3]; // N da coluna onde esta a categoria

            // Array de questoes
            questionData.answers = new string[4];

            // Validacao se existe pasta de questoes
            if (!Directory.Exists(questionsPath))
            {
                // Cria pasta se não existir
                Directory.CreateDirectory(questionsPath);
            }

            for (int i = 0; i < numberOfAnswers; i++)
            {
                questionData.answers[i] = splitData[7 + i]; // Respostas começam na coluna 7 (7+0, 7+1, 7+2, 7+3)
            }

            // Criar o ficheiro de questao
            // Ficheiro questao tem como nome a própria pergunta, logo se a pergunta tiver um ? é removido
            if (questionData.question.Contains("?"))
            {
                // Atribui o nome da pergunta
                questionData.name = questionData.question.Remove(questionData.question.IndexOf("?"));
            }
            else
            {
                questionData.name = questionData.question;
            }
            // Guarda a pergunta na diretoria questionsPath = "Assets/Resources/Questions/";
            AssetDatabase.CreateAsset(questionData, $"{questionsPath}/{questionData.name}.asset");
        }

        AssetDatabase.SaveAssets();

        Debug.Log($"Generated Questions");
    }
}