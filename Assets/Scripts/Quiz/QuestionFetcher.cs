using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class QuestionFetcher : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    private const string ApiUrl = "https://localhost:44345/api/Question/GetAllQuestion";
    private const string ApiBaseUrl = "https://localhost:44345/api/Question/GetAllQuestionCategory";


    public async Task<List<QuestionData>> FetchQuestionsAsync()
    {
        try
        {
            string category = PlayerPrefs.GetString("SelectedCategory", "Unknown");

            string requestUrl = $"{ApiBaseUrl}/{Uri.EscapeDataString(category)}";

            HttpResponseMessage response = await client.GetAsync(requestUrl);
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
                    tempoLimite = question.TempoLimite,
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
        if (question.TipoPergunta == "True/False")
        {
            foreach (var vf in question.VerdadeiroFalsos)
            {
                answers.Add(vf.Correta ? "True" : "False");
            }
        }
        else if (question.TipoPergunta == "Multiple Choice")
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
        if (question.TipoPergunta == "True/False")
        {
            foreach (var vf in question.VerdadeiroFalsos)
            {
                if (vf.Correta)
                {
                    return vf.Correta ? "True" : "False";
                }
            }
        }
        else if (question.TipoPergunta == "Multiple Choice")
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
