using UnityEngine;

public class PatientDataManager : MonoBehaviour
{
    public PatientData CurrentPatient;

    private void Start()
    {
        LoadPatient("patient_001");
    }

    public void LoadPatient(string patientFileName)
    {
        TextAsset jsonFile =
            Resources.Load<TextAsset>("Patients/" + patientFileName);

        if (jsonFile == null)
        {
            Debug.LogError("Arquivo JSON não encontrado!");
            return;
        }

        CurrentPatient =
            JsonUtility.FromJson<PatientData>(jsonFile.text);

        Debug.Log("Paciente carregado: " + CurrentPatient.nome);
    }
}