using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "ScriptableObjects/Question", order = 1)]
public class QuestionData : ScriptableObject
{
    public string question;
    public string category;
    [Tooltip("A resposta correta TEM de estar na primeira posição do array SEMPRE, randomização é feita depois")]
    public string[] answers;
}