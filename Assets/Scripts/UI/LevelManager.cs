using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int GetCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("LevelScene"))
        {
            
            string levelNumberString = sceneName.Substring(10); 
            if (int.TryParse(levelNumberString, out int levelNumber))
            {
                return levelNumber;
            }
        }

        Debug.LogError("Level name does not follow the expected convention (e.g., 'Level1').");
        return -1; 
    }
}
