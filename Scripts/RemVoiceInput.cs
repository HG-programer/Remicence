using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Remniscence
{
    /// <summary>
    /// Captures live voice from the player's microphone for real-time conversation with Rem.
    /// Supports Push-to-Talk via keyboard hotkey (e.g. V key), XR controller, or UI button.
    /// Encodes speech audio to WAV and passes directly to Google Gemini and Fish Audio.
    /// </summary>
    public class RemVoiceInput : MonoBehaviour
    {
        [Header("Microphone Settings")]
        [Tooltip("Sample rate for microphone recording (16000Hz is optimal for speech recognition/Gemini)")]
        [SerializeField] private int sampleRate = 16000;
        
        [Tooltip("Max recording duration in seconds before auto-sending")]
        [SerializeField] private int maxRecordingDuration = 15;

        [Header("Hotkeys (Push-to-Talk)")]
        [SerializeField] private KeyCode pushToTalkKey = KeyCode.V;

        [Header("Component References")]
        [SerializeField] private RemVoiceTrigger voiceTrigger;
        [SerializeField] private RemDialogueBubbleUI dialogueUI;

        private string selectedDevice = null;
        private AudioClip micClip;
        private bool isRecording = false;
        private float recordStartTime;

        public bool IsRecording => isRecording;

        void Awake()
        {
            if (voiceTrigger == null) voiceTrigger = GetComponent<RemVoiceTrigger>();
            if (dialogueUI == null) dialogueUI = GetComponent<RemDialogueBubbleUI>();
        }

        void Start()
        {
            InitializeMicrophone();
        }

        void Update()
        {
            HandlePushToTalkInput();

            if (isRecording && Time.time - recordStartTime >= maxRecordingDuration)
            {
                StopAndSendRecording();
            }
        }

        private void InitializeMicrophone()
        {
            if (Microphone.devices.Length > 0)
            {
                selectedDevice = Microphone.devices[0];
                Debug.Log($"[RemVoiceInput] Using microphone device: {selectedDevice}");
            }
            else
            {
                Debug.LogWarning("[RemVoiceInput] No microphone device detected on this system.");
            }
        }

        private void HandlePushToTalkInput()
        {
            bool keyDown = false;
            bool keyUp = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                keyDown = Keyboard.current.vKey.wasPressedThisFrame;
                keyUp = Keyboard.current.vKey.wasReleasedThisFrame;
            }
#else
            keyDown = Input.GetKeyDown(pushToTalkKey);
            keyUp = Input.GetKeyUp(pushToTalkKey);
#endif

            if (keyDown && !isRecording)
            {
                StartRecording();
            }
            else if (keyUp && isRecording)
            {
                StopAndSendRecording();
            }
        }

        /// <summary>
        /// Start recording live voice from microphone
        /// </summary>
        public void StartRecording()
        {
            if (isRecording) return;

            if (Microphone.devices.Length == 0)
            {
                Debug.LogWarning("[RemVoiceInput] Cannot record: No microphone found.");
                dialogueUI?.ShowDialogue("No microphone detected. You can type to me in the chat!", 3f);
                return;
            }

            if (string.IsNullOrEmpty(selectedDevice))
            {
                selectedDevice = Microphone.devices[0];
            }

            // Stop any ongoing Rem speech so she listens
            voiceTrigger?.StopVoiceLine();

            micClip = Microphone.Start(selectedDevice, false, maxRecordingDuration, sampleRate);
            isRecording = true;
            recordStartTime = Time.time;

            dialogueUI?.SetStatus("🎙️ Listening to you...");
            dialogueUI?.SetVisibility(true);
        }

        /// <summary>
        /// Stop recording, encode to WAV bytes, and send to Gemini
        /// </summary>
        public void StopAndSendRecording()
        {
            if (!isRecording) return;

            int micPosition = Microphone.GetPosition(selectedDevice);
            Microphone.End(selectedDevice);
            isRecording = false;

            dialogueUI?.SetStatus("Processing your voice...");

            if (micPosition <= 0 || micClip == null)
            {
                dialogueUI?.SetStatus("");
                return;
            }

            // Convert recorded clip into PCM WAV byte array
            byte[] wavBytes = WavUtility.FromAudioClip(micClip, micPosition);

            if (wavBytes != null && wavBytes.Length > 0 && voiceTrigger != null)
            {
                voiceTrigger.ProcessPlayerAudio(wavBytes);
            }
        }
    }
}
