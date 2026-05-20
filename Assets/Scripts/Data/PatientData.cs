using System;

[Serializable]
public class PatientData
{
    public string nome;
    public int idade;
    public string sexo;

    public string queixa_principal;
    public string historia_doenca_atual;

    public string[] sintomas;
    public string[] doencas_previas;
    public string[] medicamentos;
}