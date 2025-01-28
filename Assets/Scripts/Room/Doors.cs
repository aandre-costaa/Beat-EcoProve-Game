using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private GameObject quizCanvas;
    private static List<Door> allDoors = new List<Door>(); // List to track all doors
    private int doorIndex = -1; // Unique index for this door instance

    private void Awake()
    {
        // Add the door to the list for indexing
        allDoors.Add(this);
    }

    private void Start()
    {
        // Assign indices to doors in the correct order
        AssignIndices();
    }

    private static void AssignIndices()
    {
        // Sort doors by their sibling index in the hierarchy
        allDoors.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        for (int i = 0; i < allDoors.Count; i++)
        {
            allDoors[i].doorIndex = i;
            Debug.Log($"Door {allDoors[i].name} assigned index {i}");
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x)
            {
                nextRoom.GetComponent<Room>().ActivateRoom(true);
                previousRoom.GetComponent<Room>().ActivateRoom(false);
            }
            else
            {
                previousRoom.GetComponent<Room>().ActivateRoom(true);
                nextRoom.GetComponent<Room>().ActivateRoom(false);
            }

            ShowQuizCanvas();
            Debug.Log($"Player triggered Door {doorIndex}");
        }
    }

    private int GetDoorIndex()
    {
        return transform.GetSiblingIndex(); // Assuming each door is a sibling in the hierarchy
    }

    public void ShowQuizCanvas()
    {
        if (quizCanvas != null)
        {
            quizCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("QuizCanvas is not assigned in the inspector.");
        }
    }

    //public int GetDoorIndex()
    //{
    //    return doorIndex;
    //}
}