using System;

[Serializable]
public class STTResponse
{
    public STTResult[] results;
}

[Serializable]
public class STTResult
{
    public STTAlternative[] alternatives;
}

[Serializable]
public class STTAlternative
{
    public string transcript;
}