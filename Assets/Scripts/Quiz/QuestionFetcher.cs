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
    private const string ApiBaseCuriosidadeUrl = "https://localhost:44345/api/Question/GetAllQuestionCuriosidade";


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

    public async Task<List<QuestionData>> FetchQuestionsCuriousidadeAsync()
    {
        try
        {
            string requestUrl = $"{ApiBaseCuriosidadeUrl}";

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
                    correctAnswer = GetCorrectAnswer(question),
                    curiosidade = GetCuriosidade(question) // Adicionar curiosidade
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
                //answers.Add(vf.Curiosidade);
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

    private string GetCuriosidade(QuestionDto question)
    {
        // Obter a curiosidade baseada no tipo de pergunta
        if (question.TipoPergunta == "True/False")
        {
            if (question.VerdadeiroFalsos != null && question.VerdadeiroFalsos.Count > 0)
            {
                var vf = question.VerdadeiroFalsos[0]; // Pega o primeiro item

                // Debug para verificar o que está vindo da API
                Debug.Log($"Curiosidade da API: '{vf.Curiosidade}'");

                // Verificar se a curiosidade não é null ou vazia
                if (!string.IsNullOrEmpty(vf.Curiosidade) && !string.IsNullOrWhiteSpace(vf.Curiosidade))
                {
                    return vf.Curiosidade;
                }
                else
                {
                    Debug.LogWarning($"Curiosidade vazia ou null para pergunta ID: {question.Id}");
                    return "Curiosidade não disponível para esta pergunta.";
                }
            }
            else
            {
                Debug.LogWarning($"VerdadeiroFalsos vazio para pergunta ID: {question.Id}");
                return "Dados de curiosidade não encontrados.";
            }
        }
        else if (question.TipoPergunta == "Multiple Choice")
        {
            // Para múltipla escolha, você pode implementar lógica similar se tiver curiosidades
            return "Curiosidade não disponível para este tipo de pergunta.";
        }

        return "Curiosidade não disponível.";
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
