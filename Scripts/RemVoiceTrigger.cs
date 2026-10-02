using UnityEngine;
using System.Collections;

namespace Remniscence
{
    /// <summary>
    /// Pure Live Voice System for Rem:
    /// Powered 100% dynamically by Google Gemini API (Conversational AI Brain)
    /// and Fish Audio API (Live Anime Voice Synthesis).
    /// All static pre-recorded clip mechanisms have been retired.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class RemVoiceTrigger : MonoBehaviour
    {
        [Header("Audio Output Settings")]
        [SerializeField] private AudioSource audioSource;
        [Range(0f, 1f)]
        [SerializeField] private float voiceVolume = 0.9f;

        [Header("Live AI & Speech Services")]
        [SerializeField] private GoogleGeminiClient geminiClient;
        [SerializeField] private FishAudioClient fishAudioClient;
        [SerializeField] private RemDialogueBubbleUI dialogueUI;
        [SerializeField] private RemChatVoiceUI chatVoiceUI;

        private bool isSpeaking = false;
        private Coroutine activeInteractionRoutine;

        void Awake()
        {
            InitializeAudioSource();
            FindRequiredServices();
        }

        private void InitializeAudioSource()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.volume = voiceVolume;
            audioSource.spatialBlend = 0.85f; // Immersive 3D audio in VR/AR
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 1.0f;
            audioSource.maxDistance = 15.0f;
        }

        private void FindRequiredServices()
        {
            if (geminiClient == null) geminiClient = GetComponent<GoogleGeminiClient>() ?? gameObject.AddComponent<GoogleGeminiClient>();
            if (fishAudioClient == null) fishAudioClient = GetComponent<FishAudioClient>() ?? gameObject.AddComponent<FishAudioClient>();
            if (dialogueUI == null) dialogueUI = GetComponent<RemDialogueBubbleUI>() ?? gameObject.AddComponent<RemDialogueBubbleUI>();
            if (chatVoiceUI == null) chatVoiceUI = FindObjectOfType<RemChatVoiceUI>();
        }

        /// <summary>
        /// Live voice interaction: process user's microphone WAV audio through Gemini, then speak with Fish Audio
        /// </summary>
        public void ProcessPlayerAudio(byte[] wavBytes)
        {
            if (wavBytes == null || wavBytes.Length == 0) return;

            StopVoiceLine();
            activeInteractionRoutine = StartCoroutine(ProcessLiveAudioRoutine(wavBytes));
        }

        private IEnumerator ProcessLiveAudioRoutine(byte[] wavBytes)
        {
            dialogueUI?.SetThinkingState(true);

            string geminiReply = null;
            string geminiError = null;
            bool isGeminiDone = false;

            yield return geminiClient.GenerateResponseFromAudio(
                wavBytes,
                onSuccess: (res) => { geminiReply = res; isGeminiDone = true; },
                onError: (err) => { geminiError = err; isGeminiDone = true; }
            );

            while (!isGeminiDone) yield return null;
            dialogueUI?.SetThinkingState(false);

            if (!string.IsNullOrEmpty(geminiReply))
            {
                yield return SpeakLiveTextRoutine(geminiReply);
            }
            else
            {
                Debug.LogWarning($"[RemVoiceTrigger] Audio comprehension error: {geminiError}");
                yield return SpeakLiveTextRoutine("I'm sorry, Harshit, I couldn't quite hear you. Could you speak to me once more?");
            }
        }

        /// <summary>
        /// Live text interaction: send player text to Gemini, then vocalize response with Fish Audio
        /// </summary>
        public void ProcessPlayerInput(string playerInput)
        {
            if (string.IsNullOrWhiteSpace(playerInput)) return;

            StopVoiceLine();
            activeInteractionRoutine = StartCoroutine(ProcessLiveTextRoutine(playerInput));
        }

        private IEnumerator ProcessLiveTextRoutine(string playerInput)
        {
            dialogueUI?.SetThinkingState(true);

            string geminiReply = null;
            string geminiError = null;
            bool isGeminiDone = false;

            yield return geminiClient.GenerateResponse(
                playerInput,
                onSuccess: (res) => { geminiReply = res; isGeminiDone = true; },
                onError: (err) => { geminiError = err; isGeminiDone = true; }
            );

            while (!isGeminiDone) yield return null;
            dialogueUI?.SetThinkingState(false);

            if (!string.IsNullOrEmpty(geminiReply))
            {
                yield return SpeakLiveTextRoutine(geminiReply);
            }
            else
            {
                Debug.LogWarning($"[RemVoiceTrigger] Gemini text error: {geminiError}");
                yield return SpeakLiveTextRoutine("Rem is here with you, Harshit. Please keep going, I believe in you.");
            }
        }

        /// <summary>
        /// Request a spontaneous contextual voice line from Gemini (no pre-recorded clips)
        /// </summary>
        private void TriggerLiveContextualLine(string contextPrompt)
        {
            StopVoiceLine();
            activeInteractionRoutine = StartCoroutine(LiveContextualRoutine(contextPrompt));
        }

        private IEnumerator LiveContextualRoutine(string contextPrompt)
        {
            dialogueUI?.SetThinkingState(true);

            string geminiReply = null;
            string geminiError = null;
            bool isGeminiDone = false;

            yield return geminiClient.GenerateLiveContext(
                contextPrompt,
                onSuccess: (res) => { geminiReply = res; isGeminiDone = true; },
                onError: (err) => { geminiError = err; isGeminiDone = true; }
            );

            while (!isGeminiDone) yield return null;
            dialogueUI?.SetThinkingState(false);

            if (!string.IsNullOrEmpty(geminiReply))
            {
                yield return SpeakLiveTextRoutine(geminiReply);
            }
            else
            {
                Debug.LogWarning($"[RemVoiceTrigger] Gemini live context error: {geminiError}");
                yield return SpeakLiveTextRoutine("I will always be here by your side, Harshit.");
            }
        }

        /// <summary>
        /// Play a fresh, dynamically generated greeting
        /// </summary>
        public void PlayGreeting()
        {
            TriggerLiveContextualLine("Give Harshit a warm, sweet, and caring greeting as he joins the session.");
        }

        /// <summary>
        /// Play fresh, dynamically generated coding encouragement
        /// </summary>
        public void PlayEncouragement()
        {
            TriggerLiveContextualLine("Give Harshit a brief, uplifting piece of encouragement for his coding work right now.");
        }

        /// <summary>
        /// Play a spontaneous comforting voice line
        /// </summary>
        public void PlayRandomVoiceLine()
        {
            TriggerLiveContextualLine("Say something gentle, comforting, and heartfelt to Harshit as he works.");
        }

        /// <summary>
        /// Triggered when the wake word 'Remniscence' is detected or activated
        /// </summary>
        public void OnTriggerWordDetected()
        {
            var controller = GetComponent<RemController>();
            controller?.TriggerWaveAnimation();

            TriggerLiveContextualLine("Harshit just called out 'Remniscence' to summon you. Greet him lovingly and ask what he needs help with.");
        }

        /// <summary>
        /// Synthesizes text with Fish Audio TTS and plays through AudioSource with synchronized subtitles
        /// </summary>
        public IEnumerator SpeakLiveTextRoutine(string speechText)
        {
            if (string.IsNullOrWhiteSpace(speechText)) yield break;

            dialogueUI?.ShowDialogue(speechText);
            chatVoiceUI?.AppendMessage("Rem", speechText, false);

            if (fishAudioClient != null && fishAudioClient.IsConfigured)
            {
                dialogueUI?.SetStatus("🌸 Generating voice...");

                AudioClip synthesizedClip = null;
                string ttsError = null;
                bool isTTSDone = false;

                yield return fishAudioClient.SynthesizeSpeech(
                    speechText,
                    onSuccess: (clip) => { synthesizedClip = clip; isTTSDone = true; },
                    onError: (err) => { ttsError = err; isTTSDone = true; }
                );

                while (!isTTSDone) yield return null;

                if (synthesizedClip != null)
                {
                    yield return PlayAudioClipRoutine(synthesizedClip);
                    yield break;
                }
                else
                {
                    Debug.LogWarning($"[RemVoiceTrigger] Fish Audio synthesis failed: {ttsError}");
                }
            }

            // Fallback reading pause if Fish Audio is not configured
            dialogueUI?.SetSpeakingState(false);
            yield return new WaitForSeconds(Mathf.Clamp(speechText.Length * 0.05f, 2.5f, 5.0f));
        }

        private IEnumerator PlayAudioClipRoutine(AudioClip clip)
        {
            if (clip == null) yield break;

            isSpeaking = true;
            dialogueUI?.SetSpeakingState(true);

            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitForSeconds(clip.length);

            isSpeaking = false;
            dialogueUI?.SetSpeakingState(false);
        }

        /// <summary>
        /// Immediately stops any ongoing speech and clears states
        /// </summary>
        public void StopVoiceLine()
        {
            if (activeInteractionRoutine != null)
            {
                StopCoroutine(activeInteractionRoutine);
                activeInteractionRoutine = null;
            }

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            isSpeaking = false;
            dialogueUI?.SetSpeakingState(false);
            dialogueUI?.SetThinkingState(false);
        }

        public bool IsPlayingVoiceLine()
        {
            return isSpeaking || (audioSource != null && audioSource.isPlaying);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, audioSource != null ? audioSource.maxDistance : 10.0f);
        }
    }
}
