using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Remniscence
{
    /// <summary>
    /// Client for Fish Audio TTS API (https://api.fish.audio)
    /// Converts text into natural, character-matched speech audio clips in real-time.
    /// </summary>
    public class FishAudioClient : MonoBehaviour
    {
        [Header("API Configuration")]
        [Tooltip("Fish Audio API Key from fish.audio")]
        [SerializeField] private string apiKey = "";
        
        [Tooltip("Fish Audio Voice Reference ID / Model ID (e.g., custom Rem voice model ID)")]
        [SerializeField] private string referenceId = "";
        
        [Tooltip("Audio format returned by the API (wav recommended for Unity)")]
        [SerializeField] private string audioFormat = "wav";

        [Header("Playback Settings")]
        [Range(0.5f, 2.0f)]
        [SerializeField] private float speed = 1.0f;
        
        [Range(0f, 1.0f)]
        [SerializeField] private float volume = 0.9f;

        private const string FishAudioEndpoint = "https://api.fish.audio/v1/tts";

        public void SetApiKey(string key)
        {
            apiKey = key;
        }

        public void SetReferenceId(string id)
        {
            referenceId = id;
        }

        public bool IsConfigured => !string.IsNullOrEmpty(apiKey);

        /// <summary>
        /// Request live TTS audio synthesis for given text and return AudioClip via callback
        /// </summary>
        public IEnumerator SynthesizeSpeech(string text, Action<AudioClip> onSuccess, Action<string> onError)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                onError?.Invoke("Text is empty");
                yield break;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                onError?.Invoke("Fish Audio API key is not configured");
                yield break;
            }

            string jsonPayload = BuildJsonPayload(text);

            using (UnityWebRequest request = new UnityWebRequest(FishAudioEndpoint, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerAudioClip(FishAudioEndpoint, AudioType.WAV);
                
                request.SetRequestHeader("Authorization", "Bearer " + apiKey.Trim());
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "audio/wav");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMsg = $"Fish Audio API Error: {request.responseCode} - {request.error}";
                    if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        errorMsg += $"\nDetails: {request.downloadHandler.text}";
                    }
                    onError?.Invoke(errorMsg);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                    if (clip != null)
                    {
                        clip.name = $"RemLiveVoice_{DateTime.Now.Ticks}";
                        onSuccess?.Invoke(clip);
                    }
                    else
                    {
                        onError?.Invoke("Failed to decode AudioClip from Fish Audio response");
                    }
                }
            }
        }

        private string BuildJsonPayload(string text)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"text\":\"").Append(EscapeJson(text)).Append("\",");
            if (!string.IsNullOrWhiteSpace(referenceId))
            {
                sb.Append("\"reference_id\":\"").Append(EscapeJson(referenceId.Trim())).Append("\",");
            }
            sb.Append("\"format\":\"").Append(audioFormat).Append("\",");
            sb.Append("\"speed\":").Append(speed.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
            sb.Append("}");
            return sb.ToString();
        }

        private string EscapeJson(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Replace("\\", "\\\\")
                      .Replace("\"", "\\\"")
                      .Replace("\n", "\\n")
                      .Replace("\r", "\\r")
                      .Replace("\t", "\\t");
        }
    }
}
