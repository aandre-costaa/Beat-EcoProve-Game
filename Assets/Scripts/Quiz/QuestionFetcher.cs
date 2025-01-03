using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class QuestionFetcher : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    private const string ApiUrl = "https://localhost:44345/api/Question/GetAllQuestion"; 

    public async Task<List<QuestionData>> FetchQuestionsAsync()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Debug.Log(responseBody);
            Debug.Log("responseBody");

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

            Debug.Log(apiResponse);

            if (apiResponse?.Questions?.Result == null)
            {
                Debug.LogError("No questions found in the API response.");
                return new List<QuestionData>();
            }

            // Convert API questions to QuestionData
            List<QuestionData> questions = new List<QuestionData>();
            foreach (var question in apiResponse.Questions.Result)
            {
                QuestionData questionData = new QuestionData
                {
                    question = question.TextoPergunta,
                    category = question.Categoria,
                    answers = GenerateAnswers(question),
                    correctAnswer = GetCorrectAnswer(question)
                };
                questions.Add(questionData);
            }

            return questions;
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request error: {e.Message}");
            return new List<QuestionData>();
        }
    }

    private string[] GenerateAnswers(QuestionDto question)
    {
        List<string> answers = new List<string>();

        // Add answers based on the question type
        if (question.TipoPergunta == "Verdadeiro/Falso")
        {
            foreach (var vf in question.VerdadeiroFalsos)
            {
                answers.Add(vf.Correta ? "True" : "False");
            }
        }
        else if (question.TipoPergunta == "Escolha Múltipla")
        {
            foreach (var em in question.EscolhaMultiplas)
            {
                answers.Add(em.TextoOpcao);
            }
        }

        return answers.ToArray();
    }

    private string GetCorrectAnswer(QuestionDto question)
    {
        // Determine the correct answer based on the question type
        if (question.TipoPergunta == "Verdadeiro/Falso")
        {
            foreach (var vf in question.VerdadeiroFalsos)
            {
                if (vf.Correta)
                {
                    return vf.Correta ? "True" : "False";
                }
            }
        }
        else if (question.TipoPergunta == "Escolha Múltipla")
        {
            foreach (var em in question.EscolhaMultiplas)
            {
                if (em.Correta)
                {
                    return em.TextoOpcao;
                }
            }
        }

        return string.Empty; // Return an empty string if no correct answer is found
    }
}
