using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRegionManager : MonoBehaviour
{
    private const string lastCompletedKey = "LastCompletedLevel";

    public MapRegion[] mapRegions;

    void Start()
    {
        //PlayerPrefs.DeleteKey("LastCompletedLevel");
        //PlayerPrefs.Save();
        PlayerPrefs.DeleteKey("Level_1_Completed");
        PlayerPrefs.Save();
        UpdateRegions();
    }

    public void UpdateRegions()
    {
        foreach (MapRegion region in mapRegions)
        {
            int levelNumber = region.requiredLevel;
            bool isCompleted = PlayerPrefs.GetInt("Level_" + levelNumber + "_Completed", 0) == 1;
    
            if (isCompleted)
            {
                Debug.Log($"Teste: ");
                if (region.lockedRegion != null)
                    region.lockedRegion.SetActive(true);
                if (region.unlockedRegion != null)
                    region.unlockedRegion.SetActive(false);
            }
            else
            {
                Debug.Log($"Teste2: ");
                if (region.lockedRegion != null)
                    region.lockedRegion.SetActive(false);
                if (region.unlockedRegion != null)
                    region.unlockedRegion.SetActive(true);
            }
        }
    }
}