using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class STTManager : MonoBehaviour
{

    [Header("Recording")]
    [SerializeField] private int recordingLength = 5;

    [Header("References")]
    [SerializeField] private UIManager uiManager;

    private AudioClip recordedClip;

    private string microphoneName;

    private string apiUrl =
        "https://speech.googleapis.com/v1/speech:recognize?key=";

    private void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneName =
                Microphone.devices[0];

            Debug.Log(
                "Microfone encontrado: " +
                microphoneName
            );
        }
        else
        {
            Debug.LogError(
                "Nenhum microfone encontrado!"
            );
        }
    }

    public void StartRecording()
    {
        recordedClip =
            Microphone.Start(
                microphoneName,
                false,
                recordingLength,
                44100
            );

        Debug.Log("Gravando...");
    }

    public void StopRecording()
    {
        Microphone.End(microphoneName);

        Debug.Log("Gravação finalizada.");

        StartCoroutine(
            SendAudioToGoogle()
        );
    }

    private IEnumerator SendAudioToGoogle()
    {
        byte[] wavData =
            WavUtility.FromAudioClip(recordedClip);

        string base64Audio =
            Convert.ToBase64String(wavData);

        string jsonRequest =
        "{ " +
        "\"config\": {" +
        "\"encoding\": \"LINEAR16\"," +
        "\"sampleRateHertz\": 44100," +
        "\"languageCode\": \"pt-BR\"" +
        "}," +

        "\"audio\": {" +
        "\"content\": \"" + base64Audio + "\"" +
        "}" +
        "}";

        byte[] bodyRaw =
            Encoding.UTF8.GetBytes(jsonRequest);

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

            Debug.Log(response);

            STTResponse sttResponse =
                JsonUtility.FromJson<STTResponse>(
                    response
                );

            string recognizedText =
                sttResponse.results[0]
                .alternatives[0]
                .transcript;

            Debug.Log(
                "Texto reconhecido: " +
                recognizedText
            );

            uiManager.SetQuestionText(
                recognizedText
            );

            uiManager.SendQuestion();
        }
    }
}