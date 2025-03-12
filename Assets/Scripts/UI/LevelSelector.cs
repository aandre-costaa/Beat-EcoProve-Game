using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;

    private Dictionary<int, string> levelCategories = new Dictionary<int, string>()
    {
        { 1, "Sustainability in the textile sector" },
        { 2, "Environmental impacts of the textile sector" },
        { 3, "Environmental impacts of the textile sector" },
        { 4, "Environmental impacts of the textile sector" }
    };

    private void Start()
    {
        int lastCompletedLevel = PlayerPrefs.GetInt("LastCompletedLevel", 0);
        Debug.Log("LastCompleted: " + lastCompletedLevel);
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; 

            
            if (levelIndex == 1 || lastCompletedLevel >= (levelIndex - 1))
            {
                levelButtons[i].interactable = true;
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
            }
            else
            {
                levelButtons[i].interactable = false; 
            }
        }

    }

    //Apenas foi usado para teste para fazer a limpeza dos dados do local storage
    private void ClearPlayerData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Todos os dados do jogador foram apagados!");
    }

    private void LoadLevel(int levelIndex)
    {
        string sceneName = "LevelScene" + levelIndex;

        if (levelCategories.ContainsKey(levelIndex))
        {
            PlayerPrefs.SetString("SelectedCategory", levelCategories[levelIndex]);
        }
        else
        {
            PlayerPrefs.SetString("SelectedCategory", "Unknown");
        }

        SceneManager.LoadScene(sceneName);
    }


    public void GoBack()
    {
        SceneManager.LoadScene("_MainMenu");
    }
}
