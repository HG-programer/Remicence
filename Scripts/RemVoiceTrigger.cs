using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Remniscence
{
    /// <summary>
    /// Handles voice interaction, TTS, and comforting voice lines for Rem
    /// </summary>
    public class RemVoiceTrigger : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float voiceVolume = 0.8f;
        [SerializeField] private bool usePreRecordedVoices = true;
        
        [Header("Voice Lines")]
        [SerializeField] private AudioClip[] comfortingVoiceLines;
        [SerializeField] private AudioClip[] greetingVoiceLines;
        [SerializeField] private AudioClip[] encouragementVoiceLines;
        
        [Header("TTS Settings")]
        [SerializeField] private bool enableTTS = false;
        [SerializeField] private string ttsApiUrl = ""; // For future TTS API integration
        
        [Header("GPT Integration")]
        [SerializeField] private bool enableGPTIntegration = false;
        [SerializeField] private string gptApiKey = ""; // Set via inspector or config
        [SerializeField] private float responseDelay = 2.0f;
        
        // Predefined text responses (fallback when no audio available)
        private readonly string[] comfortingTexts = {
            "You're doing great, Harshit.",
            "I believe in you.",
            "Take a deep breath. You've got this.",
            "Remember to take breaks when you need them.",
            "Your hard work will pay off.",
            "I'm here with you.",
            "You're stronger than you know.",
            "Every step forward counts.",
            "I'm proud of your progress.",
            "You don't have to be perfect, just keep going."
        };
        
        private readonly string[] greetingTexts = {
            "Hello again...",
            "Welcome back.",
            "I missed you.",
            "Ready for another adventure?",
            "It's good to see you."
        };
        
        private readonly string[] encouragementTexts = {
            "You can do this!",
            "I have faith in you.",
            "Keep pushing forward.",
            "You're making great progress.",
            "Don't give up now."
        };
        
        private bool isPlayingVoiceLine = false;
        private Queue<string> textResponseQueue = new Queue<string>();
        
        void Start()
        {
            InitializeAudioSource();
            StartCoroutine(ProcessTextResponses());
        }
        
        /// <summary>
        /// Initialize audio source component
        /// </summary>
        private void InitializeAudioSource()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
            
            audioSource.volume = voiceVolume;
            audioSource.spatialBlend = 1.0f; // 3D audio
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = 10.0f;
        }
        
        /// <summary>
        /// Play a random comforting voice line
        /// </summary>
        public void PlayRandomVoiceLine()
        {
            if (isPlayingVoiceLine) return;
            
            if (usePreRecordedVoices && comfortingVoiceLines.Length > 0)
            {
                PlayRandomAudioClip(comfortingVoiceLines);
            }
            else
            {
                // Fallback to text response
                string randomText = comfortingTexts[Random.Range(0, comfortingTexts.Length)];
                DisplayTextResponse(randomText);
                
                if (enableTTS)
                {
                    StartCoroutine(SynthesizeAndPlaySpeech(randomText));
                }
            }
        }
        
        /// <summary>
        /// Play greeting voice line
        /// </summary>
        public void PlayGreeting()
        {
            if (isPlayingVoiceLine) return;
            
            if (usePreRecordedVoices && greetingVoiceLines.Length > 0)
            {
                PlayRandomAudioClip(greetingVoiceLines);
            }
            else
            {
                string randomGreeting = greetingTexts[Random.Range(0, greetingTexts.Length)];
                DisplayTextResponse(randomGreeting);
                
                if (enableTTS)
                {
                    StartCoroutine(SynthesizeAndPlaySpeech(randomGreeting));
                }
            }
        }
        
        /// <summary>
        /// Play encouragement voice line
        /// </summary>
        public void PlayEncouragement()
        {
            if (isPlayingVoiceLine) return;
            
            if (usePreRecordedVoices && encouragementVoiceLines.Length > 0)
            {
                PlayRandomAudioClip(encouragementVoiceLines);
            }
            else
            {
                string randomEncouragement = encouragementTexts[Random.Range(0, encouragementTexts.Length)];
                DisplayTextResponse(randomEncouragement);
                
                if (enableTTS)
                {
                    StartCoroutine(SynthesizeAndPlaySpeech(randomEncouragement));
                }
            }
        }
        
        /// <summary>
        /// Play random audio clip from array
        /// </summary>
        private void PlayRandomAudioClip(AudioClip[] clips)
        {
            if (clips.Length == 0) return;
            
            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            StartCoroutine(PlayAudioClip(randomClip));
        }
        
        /// <summary>
        /// Coroutine to play audio clip
        /// </summary>
        private IEnumerator PlayAudioClip(AudioClip clip)
        {
            isPlayingVoiceLine = true;
            
            audioSource.clip = clip;
            audioSource.Play();
            
            yield return new WaitForSeconds(clip.length);
            
            isPlayingVoiceLine = false;
        }
        
        /// <summary>
        /// Display text response in UI (for when no audio is available)
        /// </summary>
        private void DisplayTextResponse(string text)
        {
            textResponseQueue.Enqueue(text);
            Debug.Log($"Rem says: {text}");
            
            // TODO: Display in UI bubble above character
        }
        
        /// <summary>
        /// Process text response queue for UI display
        /// </summary>
        private IEnumerator ProcessTextResponses()
        {
            while (true)
            {
                if (textResponseQueue.Count > 0)
                {
                    string response = textResponseQueue.Dequeue();
                    // TODO: Show text bubble for 3 seconds
                    yield return new WaitForSeconds(3.0f);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        
        /// <summary>
        /// Synthesize speech using TTS API (placeholder implementation)
        /// </summary>
        private IEnumerator SynthesizeAndPlaySpeech(string text)
        {
            if (string.IsNullOrEmpty(ttsApiUrl))
            {
                Debug.LogWarning("TTS API URL not configured");
                yield break;
            }
            
            // TODO: Implement actual TTS API call
            // This is a placeholder for TTS integration
            yield return new WaitForSeconds(responseDelay);
            
            Debug.Log($"TTS would synthesize: {text}");
        }
        
        /// <summary>
        /// Process player input for GPT integration
        /// </summary>
        public void ProcessPlayerInput(string playerInput)
        {
            if (!enableGPTIntegration)
            {
                PlayRandomVoiceLine();
                return;
            }
            
            StartCoroutine(GetGPTResponse(playerInput));
        }
        
        /// <summary>
        /// Get response from GPT API (placeholder implementation)
        /// </summary>
        private IEnumerator GetGPTResponse(string input)
        {
            if (string.IsNullOrEmpty(gptApiKey))
            {
                Debug.LogWarning("GPT API key not configured");
                PlayRandomVoiceLine();
                yield break;
            }
            
            // TODO: Implement actual GPT API integration
            // This is a placeholder for GPT integration
            yield return new WaitForSeconds(responseDelay);
            
            // For now, respond with a random comforting line
            PlayRandomVoiceLine();
        }
        
        /// <summary>
        /// Triggered when player says "Remniscence"
        /// </summary>
        public void OnTriggerWordDetected()
        {
            PlayGreeting();
            
            // Trigger special appearing effect
            var remController = GetComponent<RemController>();
            if (remController != null)
            {
                remController.TriggerWaveAnimation();
            }
            
            // TODO: Add particle effects for magical appearance
        }
        
        /// <summary>
        /// Stop current voice line
        /// </summary>
        public void StopVoiceLine()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                isPlayingVoiceLine = false;
            }
        }
        
        /// <summary>
        /// Check if currently playing a voice line
        /// </summary>
        public bool IsPlayingVoiceLine()
        {
            return isPlayingVoiceLine;
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw audio range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, audioSource?.maxDistance ?? 10.0f);
        }
    }
}
