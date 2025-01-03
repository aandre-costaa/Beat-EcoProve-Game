using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private GameObject quizCanvas;

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

            var questionSetup = quizCanvas.GetComponent<QuestionSetup>();
            if (questionSetup != null)
            {
                questionSetup.StartQuizForDoor(GetDoorIndex());
            }
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
}