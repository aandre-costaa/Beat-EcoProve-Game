using System.Net.Http;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;

public class RequestProfile : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    private const string ApiUrlProfile = "https://api.med2.ipvc.bioeconomy-at-textiles.com/v1/profiles";

    public static async Task GetProfileAsync()
    {
        string accessToken = PlayerPrefs.GetString("AccessToken", string.Empty);
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access token not found. Please login first.");
            return;
        }

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        HttpResponseMessage response = null;
        try
        {
            response = await client.GetAsync(ApiUrlProfile);

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                Debug.Log("Access token expired. Attempting to refresh tokens...");
                bool refreshed = await TokenRefresher.RefreshTokenAsync();
                if (refreshed)
                {
                    accessToken = PlayerPrefs.GetString("AccessToken", string.Empty);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    response = await client.GetAsync(ApiUrlProfile);
                }
                else
                {
                    Debug.LogError("Token refresh failed. Please login again.");
                    return;
                }
            }

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            ProfileResponse profileResponse = JsonUtility.FromJson<ProfileResponse>(responseJson);

            if (profileResponse != null)
            {
                string profileData = JsonUtility.ToJson(profileResponse);
                PlayerPrefs.SetString("ProfileData", profileData);
                PlayerPrefs.Save();

                Debug.Log("Profile fetched and saved locally.");

                EcoCoinsDisplayProfile ecoDisplay = FindObjectOfType<EcoCoinsDisplayProfile>();
                if (ecoDisplay != null)
                {
                    ecoDisplay.UpdateEcoCoins(profileResponse);
                }
            }
            else
            {
                Debug.LogError("Profile response is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Profile request failed: " + ex.Message);
            throw;
        }
    }
}