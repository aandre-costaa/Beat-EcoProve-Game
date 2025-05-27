using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EcoCoinsDisplayProfile : MonoBehaviour
{
    public TextMeshProUGUI ecoCoinsText;

    public void UpdateEcoCoins(ProfileResponse profileResponse)
    {
        if (profileResponse != null && profileResponse.mainProfile != null)
        {
            ecoCoinsText.text = "" + profileResponse.mainProfile.ecoCoins.ToString();
        }
        else
        {
            Debug.LogError("Profile response or mainProfile is null.");
        }
    }
}
