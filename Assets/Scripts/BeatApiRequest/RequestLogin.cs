using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RequestLogin : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    private const string ApiUrlLogin = "https://api.med2.ipvc.bioeconomy-at-textiles.com/v1/auth/login";

    private const string testEmail = "soqueroochat@gmail.com";
    private const string testPassword = "Daniel1102!";


    public static async Task LoginAsync()
    {
        LoginRequest requestData = new LoginRequest
        {
            email = testEmail,
            password = testPassword
        };

        string jsonData = JsonUtility.ToJson(requestData);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await client.PostAsync(ApiUrlLogin, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseJson);

            if (loginResponse != null)
            {
                PlayerPrefs.SetString("AccessToken", loginResponse.accessToken);
                PlayerPrefs.SetString("RefreshToken", loginResponse.refreshToken);
                PlayerPrefs.Save();
                Debug.Log("Login successful. Tokens saved.");
                
            }
            else
            {
                Debug.LogError("Login response is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Login failed: " + ex.Message);
            throw;
        }
    }


}
