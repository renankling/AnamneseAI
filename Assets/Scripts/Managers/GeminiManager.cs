using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private PatientDataManager patientDataManager;
    [SerializeField] private ConversationManager conversationManager;
    [SerializeField] private TTSManager ttsManager;

    [Header("UI")]
    [SerializeField] private UIManager uiManager;



    private string apiUrl =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=";

    public void AskPatient(string doctorQuestion)
    {
        StartCoroutine(SendQuestionToGemini(doctorQuestion));
    }

    private IEnumerator SendQuestionToGemini(string doctorQuestion)
    {
        conversationManager.AddDoctorMessage(doctorQuestion);

        PatientData patient = patientDataManager.CurrentPatient;

        string patientInfo =
            $"Nome: {patient.nome}\n" +
            $"Idade: {patient.idade}\n" +
            $"Sexo: {patient.sexo}\n\n" +

            $"Queixa principal: {patient.queixa_principal}\n" +
            $"História da doença atual: {patient.historia_doenca_atual}\n\n" +

            $"Sintomas:\n- {string.Join("\n- ", patient.sintomas)}\n\n" +

            $"Doenças prévias:\n- {string.Join("\n- ", patient.doencas_previas)}\n\n" +

            $"Medicamentos:\n- {string.Join("\n- ", patient.medicamentos)}";

        string history = conversationManager.GetConversationHistory();

        string prompt =
                "Você é um paciente em uma simulação médica de anamnese.\n\n" +

                "REGRAS IMPORTANTES:\n" +
                "- Responda SOMENTE usando informações da ficha.\n" +
                "- Nunca invente sintomas.\n" +
                "- Nunca invente doenças.\n" +
                "- Nunca dê diagnóstico.\n" +
                "- Nunca fale como IA.\n" +
                "- Responda como um paciente real.\n" +
                "- Use frases naturais e curtas.\n" +
                "- Se não souber algo, diga que não sabe.\n\n" +

                "FICHA DO PACIENTE:\n\n" +
                patientInfo +

                "\n\nHISTÓRICO DA CONVERSA:\n\n" +
                history +

                "\n\nRESPONDA COMO O PACIENTE.";

        string jsonBody =
            "{ \"contents\": [" +
            "{ \"parts\": [" +
            "{ \"text\": \"" + EscapeJson(prompt) + "\" }" +
            "] } ] }";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request =
            new UnityWebRequest(apiUrl + APIKeyManager.Instance.Keys.gemini_api_key, "POST");

        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;

            GeminiResponse geminiResponse =
                JsonUtility.FromJson<GeminiResponse>(jsonResponse);

            string patientAnswer =
                geminiResponse.candidates[0]
                .content.parts[0].text;


            conversationManager.AddPatientMessage(patientAnswer);

            Debug.Log("PACIENTE:");
            Debug.Log(patientAnswer);
            uiManager.UpdateResponseText(patientAnswer);
            ttsManager.Speak(patientAnswer);
        }
    }

    private string EscapeJson(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }
}