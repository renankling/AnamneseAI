using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Text;

public static class WavUtility
{
    const int HEADER_SIZE = 44;

    public static byte[] FromAudioClip(AudioClip clip)
    {
        MemoryStream stream =
            new MemoryStream();

        int samples = clip.samples;
        int channels = clip.channels;
        int sampleRate = clip.frequency;

        float[] data =
            new float[samples * channels];

        clip.GetData(data, 0);

        byte[] bytesData =
            ConvertAudioClipDataToInt16ByteArray(data);

        WriteHeader(
            stream,
            bytesData.Length,
            sampleRate,
            channels
        );

        stream.Write(
            bytesData,
            0,
            bytesData.Length
        );

        return stream.ToArray();
    }

    private static byte[] ConvertAudioClipDataToInt16ByteArray(
        float[] data)
    {
        int rescaleFactor = 32767;

        byte[] byteArray =
            new byte[data.Length * 2];

        for (int i = 0; i < data.Length; i++)
        {
            short value =
                (short)(data[i] * rescaleFactor);

            byte[] byteArr =
                BitConverter.GetBytes(value);

            byteArr.CopyTo(
                byteArray,
                i * 2
            );
        }

        return byteArray;
    }

    private static void WriteHeader(
        Stream stream,
        int dataLength,
        int sampleRate,
        int channels)
    {
        BinaryWriter writer =
            new BinaryWriter(stream);

        writer.Write(
            Encoding.ASCII.GetBytes("RIFF")
        );

        writer.Write(dataLength + HEADER_SIZE - 8);

        writer.Write(
            Encoding.ASCII.GetBytes("WAVE")
        );

        writer.Write(
            Encoding.ASCII.GetBytes("fmt ")
        );

        writer.Write(16);
        writer.Write((ushort)1);
        writer.Write((ushort)channels);
        writer.Write(sampleRate);

        writer.Write(
            sampleRate * channels * 2
        );

        writer.Write((ushort)(channels * 2));
        writer.Write((ushort)16);

        writer.Write(
            Encoding.ASCII.GetBytes("data")
        );

        writer.Write(dataLength);
    }
}