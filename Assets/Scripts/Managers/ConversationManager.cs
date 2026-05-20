using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    private List<string> conversationHistory =
        new List<string>();

    public void AddDoctorMessage(string message)
    {
        conversationHistory.Add(
            "Médico: " + message
        );
    }

    public void AddPatientMessage(string message)
    {
        conversationHistory.Add(
            "Paciente: " + message
        );
    }

    public string GetConversationHistory()
    {
        return string.Join(
            "\n",
            conversationHistory
        );
    }

    public void ClearConversation()
    {
        conversationHistory.Clear();
    }
}