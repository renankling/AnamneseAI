using System.IO;
using UnityEngine;

public class APIKeyManager : MonoBehaviour
{
    public static APIKeyManager Instance;

    public APIKeys Keys;

    private void Awake()
    {
        Instance = this;

        LoadKeys();
    }

    private void LoadKeys()
    {
        string path =
            Path.Combine(
                Application.streamingAssetsPath,
                "api_keys.json"
            );

        if (!File.Exists(path))
        {
            Debug.LogError(
                "Arquivo api_keys.json não encontrado!"
            );

            return;
        }

        string json =
            File.ReadAllText(path);

        Keys =
            JsonUtility.FromJson<APIKeys>(json);

        Debug.Log("API Keys carregadas.");
    }
}