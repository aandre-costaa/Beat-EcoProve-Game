using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class TokenRefresher : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    private const string ApiUrlRefresh = "https://api.med2.ipvc.bioeconomy-at-textiles.com/v1/auth/refresh_tokens";

    public static async Task<bool> RefreshTokenAsync()
    {
        string refreshToken = PlayerPrefs.GetString("RefreshToken", string.Empty);
        string profileId = PlayerPrefs.GetString("ProfileId", string.Empty);

        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(profileId))
        {
            Debug.LogError("Refresh token or Profile ID not found. Please login again.");
            return false;
        }

        string requestUrl = $"{ApiUrlRefresh}?token={refreshToken}&profileId={profileId}";

        try
        {
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError("Token refresh failed: " + response.StatusCode);
                return false;
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(responseJson);
            if (tokenResponse != null)
            {
                // Save the new tokens.
                PlayerPrefs.SetString("AccessToken", tokenResponse.accessToken);
                PlayerPrefs.SetString("RefreshToken", tokenResponse.refreshToken);
                PlayerPrefs.Save();

                Debug.Log("Token refreshed successfully.");
                return true;
            }
            else
            {
                Debug.LogError("Token refresh response is null.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Token refresh failed: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Example method demonstrating how to check a request's response for a 403 status 
    /// and then attempt to refresh tokens.
    /// </summary>
    public static async Task MakeAuthenticatedRequestAsync()
    {
        // Retrieve the current access token.
        string accessToken = PlayerPrefs.GetString("AccessToken", string.Empty);
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access token not found. Please login first.");
            return;
        }

        // Set the Authorization header.
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        // Example API endpoint that requires authentication.
        string apiUrl = "https://api.med2.ipvc.bioeconomy-at-textiles.com/v1/protected_endpoint";

        try
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Debug.Log("Access token expired or invalid. Attempting to refresh tokens...");
                bool refreshed = await RefreshTokenAsync();
                if (refreshed)
                {
                    // If tokens are refreshed, update the header and reattempt the request.
                    accessToken = PlayerPrefs.GetString("AccessToken", string.Empty);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    response = await client.GetAsync(apiUrl);
                }
            }

            // Process the response if successful.
            response.EnsureSuccessStatusCode();
            string responseJson = await response.Content.ReadAsStringAsync();
            Debug.Log("Authenticated request successful: " + responseJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("Authenticated request failed: " + ex.Message);
        }
    }
}
