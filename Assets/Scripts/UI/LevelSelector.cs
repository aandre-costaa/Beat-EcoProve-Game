using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons; 

    private void Start()
    {
        // Assign click listeners to each button
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; 
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    private void LoadLevel(int levelIndex)
    {
        string sceneName = "SampleScene" + levelIndex;
        SceneManager.LoadScene(sceneName);
    }


    public void GoBack()
    {
        SceneManager.LoadScene("_MainMenu");
    }
}
