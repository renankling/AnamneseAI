using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TTSManager : MonoBehaviour
{

    [Header("Audio")]
    [SerializeField] private AudioSource patientAudioSource;

    [Header("Data")]
    [SerializeField] private PatientDataManager patientDataManager;

    private string apiUrl =
        "https://texttospeech.googleapis.com/v1/text:synthesize?key=";

    private string GetVoiceName()
    {
        string sexo =
            patientDataManager
            .CurrentPatient
            .sexo
            .ToLower();

        if (sexo == "masculino")
        {
            return "pt-BR-Wavenet-B";
        }
        else
        {
            return "pt-BR-Wavenet-A";
        }
    }

    public void Speak(string text)
    {
        StartCoroutine(
            ConvertTextToSpeech(text)
        );
    }

    private IEnumerator ConvertTextToSpeech(string text)
    {
        string requestJson =
        "{ " +
        "\"input\": {" +
        "\"text\": \"" + EscapeJson(text) + "\"" +
        "}," +

        "\"voice\": {" +
        "\"languageCode\": \"pt-BR\"," +
        "\"name\": \"" + GetVoiceName() + "\"" +
        "}," +

        "\"audioConfig\": {" +
        "\"audioEncoding\": \"MP3\"" +
        "}" +
        "}";

        byte[] bodyRaw =
            Encoding.UTF8.GetBytes(requestJson);

        UnityWebRequest request =
            new UnityWebRequest(
                apiUrl + APIKeyManager.Instance.Keys.google_cloud_api_key,
                "POST"
            );

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
            string response =
                request.downloadHandler.text;

            TTSAudioResponse audioResponse =
                JsonUtility.FromJson<TTSAudioResponse>(
                    response
                );

            string filePath =
                Path.Combine(
                    Application.persistentDataPath,
                    "ttsAudio.mp3"
                );

            byte[] audioBytes =
                Convert.FromBase64String(
                    audioResponse.audioContent
                );

            File.WriteAllBytes(
                filePath,
                audioBytes
            );

            yield return StartCoroutine(
                LoadAndPlayAudio(filePath)
            );
        }
    }

    private IEnumerator LoadAndPlayAudio(string path)
    {
        UnityWebRequest request =
            UnityWebRequestMultimedia.GetAudioClip(
                "file://" + path,
                AudioType.MPEG
            );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            AudioClip clip =
                DownloadHandlerAudioClip
                .GetContent(request);

            patientAudioSource.clip = clip;

            patientAudioSource.Play();
        }
    }

    private string EscapeJson(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }
}