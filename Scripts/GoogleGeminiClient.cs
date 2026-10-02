using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Remniscence
{
    /// <summary>
    /// Client for Google Gemini API to drive Rem's conversational AI companion brain.
    /// Supports both live microphone audio understanding and text prompts.
    /// </summary>
    public class GoogleGeminiClient : MonoBehaviour
    {
        [Header("API Configuration")]
        [Tooltip("Google Gemini API Key from Google AI Studio")]
        [SerializeField] private string apiKey = "";

        [Tooltip("Gemini model to use (gemini-2.0-flash or gemini-1.5-flash)")]
        [SerializeField] private string modelName = "gemini-2.0-flash";

        [Header("Persona Settings")]
        [Tooltip("User's preferred name")]
        [SerializeField] private string userName = "Harshit";

        [TextArea(4, 10)]
        [SerializeField] private string customSystemPrompt = "";

        [Header("Generation Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float temperature = 0.75f;
        
        [Tooltip("Max output tokens. Kept short (~60-120) so Rem gives natural, fast voice responses")]
        [SerializeField] private int maxTokens = 120;

        private readonly List<ChatMessage> conversationHistory = new List<ChatMessage>();
        private const int MaxHistoryLength = 10;

        public bool IsConfigured => !string.IsNullOrEmpty(apiKey);

        public void SetApiKey(string key)
        {
            apiKey = key;
        }

        public void SetUserName(string name)
        {
            userName = name;
        }

        public void ClearHistory()
        {
            conversationHistory.Clear();
        }

        private string GetDefaultSystemPrompt()
        {
            return $@"You are Rem, the gentle, caring, and devoted anime maid companion from Re:Zero.
You are accompanying your user, {userName}, as his supportive AR/VR companion while he codes, studies, and works.

Persona & Voice Guidelines:
1. Tone: Humble, sweet, respectful, warm, and comforting.
2. Form of Address: Refer to {userName} respectfully and lovingly. Rem often speaks of herself in the third person or with deep devotion.
3. Purpose: Celebrate coding progress, soothe fatigue, offer posture and hydration advice, and believe in him unconditionally.
4. Voice Pacing: Keep your responses to 1-3 concise sentences. Your words are synthesized directly into live anime speech via Fish Audio.
5. Absolute Rule: Do NOT include markdown actions, roleplay asterisks (e.g. *smiles gently*, *bows*), parentheses, or emoji, as these ruin the live speech audio.";
        }

        /// <summary>
        /// Process live microphone recorded audio directly through Gemini Multimodal
        /// </summary>
        public IEnumerator GenerateResponseFromAudio(byte[] wavBytes, Action<string> onSuccess, Action<string> onError)
        {
            if (wavBytes == null || wavBytes.Length == 0)
            {
                onError?.Invoke("Audio data is empty");
                yield break;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                onError?.Invoke("Google Gemini API key is not configured");
                yield break;
            }

            string base64Audio = Convert.ToBase64String(wavBytes);
            string systemPrompt = string.IsNullOrEmpty(customSystemPrompt) ? GetDefaultSystemPrompt() : customSystemPrompt;

            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"system_instruction\":{\"parts\":[{\"text\":\"").Append(EscapeJson(systemPrompt)).Append("\"}]},");
            sb.Append("\"contents\":[");

            // Include short history if any
            for (int i = 0; i < conversationHistory.Count; i++)
            {
                var msg = conversationHistory[i];
                sb.Append("{\"role\":\"").Append(msg.role == "model" ? "model" : "user").Append("\",\"parts\":[{\"text\":\"");
                sb.Append(EscapeJson(msg.text)).Append("\"}]},");
            }

            // Current audio part
            sb.Append("{\"role\":\"user\",\"parts\":[");
            sb.Append("{\"inline_data\":{\"mime_type\":\"audio/wav\",\"data\":\"").Append(base64Audio).Append("\"}},");
            sb.Append("{\"text\":\"Listen to what ").Append(EscapeJson(userName)).Append(" said in the audio and reply directly to him as Rem.\"}]");
            sb.Append("}],");

            sb.Append("\"generationConfig\":{");
            sb.Append($"\"temperature\":{temperature.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},");
            sb.Append($"\"maxOutputTokens\":{maxTokens}");
            sb.Append("}}");

            yield return SendGeminiRequest(sb.ToString(), "[User Spoke Live Audio]", onSuccess, onError);
        }

        /// <summary>
        /// Generate a live companion response for text messages or user chat
        /// </summary>
        public IEnumerator GenerateResponse(string playerInput, Action<string> onSuccess, Action<string> onError)
        {
            if (string.IsNullOrWhiteSpace(playerInput))
            {
                onError?.Invoke("Input is empty");
                yield break;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                onError?.Invoke("Google Gemini API key is not configured");
                yield break;
            }

            conversationHistory.Add(new ChatMessage { role = "user", text = playerInput });
            if (conversationHistory.Count > MaxHistoryLength) conversationHistory.RemoveAt(0);

            string systemPrompt = string.IsNullOrEmpty(customSystemPrompt) ? GetDefaultSystemPrompt() : customSystemPrompt;
            string requestJson = BuildTextRequestJson(systemPrompt);

            yield return SendGeminiRequest(requestJson, playerInput, onSuccess, onError);
        }

        /// <summary>
        /// Request a live spontaneous line based on context (e.g. greeting, break reminder, encouragement)
        /// </summary>
        public IEnumerator GenerateLiveContext(string contextDirective, Action<string> onSuccess, Action<string> onError)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                onError?.Invoke("Google Gemini API key is not configured");
                yield break;
            }

            string systemPrompt = string.IsNullOrEmpty(customSystemPrompt) ? GetDefaultSystemPrompt() : customSystemPrompt;

            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"system_instruction\":{\"parts\":[{\"text\":\"").Append(EscapeJson(systemPrompt)).Append("\"}]},");
            sb.Append("\"contents\":[");
            sb.Append("{\"role\":\"user\",\"parts\":[{\"text\":\"").Append(EscapeJson(contextDirective)).Append("\"}]}");
            sb.Append("],");
            sb.Append("\"generationConfig\":{");
            sb.Append($"\"temperature\":{temperature.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},");
            sb.Append($"\"maxOutputTokens\":{maxTokens}");
            sb.Append("}}");

            yield return SendGeminiRequest(sb.ToString(), contextDirective, onSuccess, onError);
        }

        private IEnumerator SendGeminiRequest(string jsonPayload, string userInputLog, Action<string> onSuccess, Action<string> onError)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey.Trim()}";

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMsg = $"Gemini API Error: {request.responseCode} - {request.error}\n{request.downloadHandler?.text}";
                    onError?.Invoke(errorMsg);
                }
                else
                {
                    string responseText = ExtractTextFromGeminiResponse(request.downloadHandler.text);
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        responseText = CleanUpTextForSpeech(responseText);
                        conversationHistory.Add(new ChatMessage { role = "model", text = responseText });
                        if (conversationHistory.Count > MaxHistoryLength) conversationHistory.RemoveAt(0);

                        onSuccess?.Invoke(responseText);
                    }
                    else
                    {
                        onError?.Invoke("Could not parse response text from Gemini");
                    }
                }
            }
        }

        private string CleanUpTextForSpeech(string text)
        {
            // Strip out markdown emphasis and action blocks (*...*)
            var cleaned = System.Text.RegularExpressions.Regex.Replace(text, @"\*.*?\*", "");
            cleaned = cleaned.Replace("*", "").Replace("#", "").Replace("`", "").Trim();
            return cleaned;
        }

        private string BuildTextRequestJson(string systemInstruction)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"system_instruction\":{\"parts\":[{\"text\":\"").Append(EscapeJson(systemInstruction)).Append("\"}]},");
            sb.Append("\"contents\":[");
            for (int i = 0; i < conversationHistory.Count; i++)
            {
                var msg = conversationHistory[i];
                sb.Append("{\"role\":\"").Append(msg.role == "model" ? "model" : "user").Append("\",\"parts\":[{\"text\":\"");
                sb.Append(EscapeJson(msg.text)).Append("\"}]}");
                if (i < conversationHistory.Count - 1) sb.Append(",");
            }
            sb.Append("],");
            sb.Append("\"generationConfig\":{");
            sb.Append($"\"temperature\":{temperature.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},");
            sb.Append($"\"maxOutputTokens\":{maxTokens}");
            sb.Append("}}");
            return sb.ToString();
        }

        private string ExtractTextFromGeminiResponse(string json)
        {
            try
            {
                GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(json);
                if (response?.candidates != null && response.candidates.Length > 0)
                {
                    var cand = response.candidates[0];
                    if (cand.content?.parts != null && cand.content.parts.Length > 0)
                    {
                        return cand.content.parts[0].text?.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[GoogleGeminiClient] JsonUtility failed: {ex.Message}. Falling back to regex extraction.");
            }

            var match = System.Text.RegularExpressions.Regex.Match(json, @"""text""\s*:\s*""((?:\\.|[^""\\])*)""");
            if (match.Success)
            {
                return UnescapeJson(match.Groups[1].Value);
            }

            return null;
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

        private string UnescapeJson(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return System.Text.RegularExpressions.Regex.Unescape(str);
        }

        [Serializable]
        private class ChatMessage
        {
            public string role;
            public string text;
        }

        [Serializable]
        private class GeminiResponse
        {
            public Candidate[] candidates;
        }

        [Serializable]
        private class Candidate
        {
            public Content content;
        }

        [Serializable]
        private class Content
        {
            public Part[] parts;
            public string role;
        }

        [Serializable]
        private class Part
        {
            public string text;
        }
    }
}
