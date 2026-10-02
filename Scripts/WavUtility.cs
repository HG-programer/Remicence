using System;
using System.IO;
using UnityEngine;

namespace Remniscence
{
    /// <summary>
    /// Lightweight utility to convert Unity AudioClip recording data into 16-bit PCM WAV byte array
    /// for Google Gemini and audio API requests.
    /// </summary>
    public static class WavUtility
    {
        private const int HeaderSize = 44;

        /// <summary>
        /// Converts an AudioClip to a WAV byte array, optionally trimming trailing silence
        /// </summary>
        public static byte[] FromAudioClip(AudioClip clip, int recordedSamples = -1)
        {
            if (clip == null) return null;

            int channels = clip.channels;
            int frequency = clip.frequency;
            int sampleCount = recordedSamples > 0 ? recordedSamples * channels : clip.samples * channels;

            float[] samples = new float[sampleCount];
            clip.GetData(samples, 0);

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    WriteWavHeader(writer, channels, frequency, sampleCount);

                    // Convert float samples [-1.0f, 1.0f] to 16-bit PCM integers
                    short[] intData = new short[sampleCount];
                    for (int i = 0; i < sampleCount; i++)
                    {
                        float val = Mathf.Clamp(samples[i], -1.0f, 1.0f);
                        intData[i] = (short)(val * 32767);
                        writer.Write(intData[i]);
                    }
                }
                return stream.ToArray();
            }
        }

        private static void WriteWavHeader(BinaryWriter writer, int channels, int sampleRate, int sampleCount)
        {
            int byteRate = sampleRate * channels * 2; // 16-bit = 2 bytes per sample
            int dataChunkSize = sampleCount * 2;
            int fileSize = HeaderSize + dataChunkSize - 8;

            writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
            writer.Write(fileSize);
            writer.Write(new char[4] { 'W', 'A', 'V', 'E' });

            // 'fmt ' subchunk
            writer.Write(new char[4] { 'f', 'm', 't', ' ' });
            writer.Write(16); // Subchunk1Size (16 for PCM)
            writer.Write((short)1); // AudioFormat (1 for PCM)
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)(channels * 2)); // BlockAlign
            writer.Write((short)16); // BitsPerSample

            // 'data' subchunk
            writer.Write(new char[4] { 'd', 'a', 't', 'a' });
            writer.Write(dataChunkSize);
        }
    }
}
