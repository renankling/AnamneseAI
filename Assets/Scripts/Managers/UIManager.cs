using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField questionInput;

    [SerializeField] private GeminiManager geminiManager;

    [SerializeField] private TMP_Text responseText;

    public void SendQuestion()
    {
        string question = questionInput.text;

        if (string.IsNullOrEmpty(question))
            return;

        geminiManager.AskPatient(question);
    }

    public void UpdateResponseText(string response)
    {
        responseText.text = response;
    }
}