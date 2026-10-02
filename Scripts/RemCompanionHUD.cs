using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Remniscence
{
    /// <summary>
    /// Companion Control HUD for desktop and XR testing.
    /// Supports live microphone push-to-talk, dynamic quick prompts, and text chat.
    /// </summary>
    public class RemCompanionHUD : MonoBehaviour
    {
        [Header("Target References")]
        [SerializeField] private RemVoiceTrigger voiceTrigger;
        [SerializeField] private RemVoiceInput voiceInput;
        [SerializeField] private GoogleGeminiClient geminiClient;
        [SerializeField] private FishAudioClient fishAudioClient;

        [Header("UI Elements")]
        [SerializeField] private TMP_InputField chatInputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button pushToTalkButton;
        [SerializeField] private TextMeshProUGUI pushToTalkButtonText;

        [Header("Live Prompt Buttons")]
        [SerializeField] private Button cheerButton;
        [SerializeField] private Button postureCheckButton;
        [SerializeField] private Button waterReminderButton;
        [SerializeField] private Button breakButton;

        void Awake()
        {
            FindReferences();
        }

        void Start()
        {
            SetupListeners();
            SetupPushToTalkButton();
        }

        private void FindReferences()
        {
            if (voiceTrigger == null) voiceTrigger = FindObjectOfType<RemVoiceTrigger>();
            if (voiceInput == null) voiceInput = FindObjectOfType<RemVoiceInput>();
            if (geminiClient == null) geminiClient = FindObjectOfType<GoogleGeminiClient>();
            if (fishAudioClient == null) fishAudioClient = FindObjectOfType<FishAudioClient>();
        }

        private void SetupListeners()
        {
            if (sendButton != null && chatInputField != null)
            {
                sendButton.onClick.AddListener(OnSendMessageClicked);
            }

            if (cheerButton != null)
            {
                cheerButton.onClick.AddListener(() => voiceTrigger?.PlayEncouragement());
            }

            if (postureCheckButton != null)
            {
                postureCheckButton.onClick.AddListener(() => SendPresetMessage("Rem, please gently remind me to sit up straight and stretch."));
            }

            if (waterReminderButton != null)
            {
                waterReminderButton.onClick.AddListener(() => SendPresetMessage("Rem, can you remind me to drink some water?"));
            }

            if (breakButton != null)
            {
                breakButton.onClick.AddListener(() => SendPresetMessage("Rem, I'm feeling a bit tired."));
            }
        }

        private void SetupPushToTalkButton()
        {
            if (pushToTalkButton == null) return;

            EventTrigger trigger = pushToTalkButton.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = pushToTalkButton.gameObject.AddComponent<EventTrigger>();

            // Pointer Down -> Start recording
            var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDown.callback.AddListener((data) =>
            {
                if (voiceInput != null)
                {
                    voiceInput.StartRecording();
                    if (pushToTalkButtonText != null) pushToTalkButtonText.text = "🔴 Release to Send";
                }
            });
            trigger.triggers.Add(pointerDown);

            // Pointer Up -> Stop and send
            var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUp.callback.AddListener((data) =>
            {
                if (voiceInput != null)
                {
                    voiceInput.StopAndSendRecording();
                    if (pushToTalkButtonText != null) pushToTalkButtonText.text = "🎙️ Hold to Talk";
                }
            });
            trigger.triggers.Add(pointerUp);
        }

        public void OnSendMessageClicked()
        {
            if (chatInputField == null || string.IsNullOrWhiteSpace(chatInputField.text)) return;

            string message = chatInputField.text.Trim();
            chatInputField.text = "";

            SendPresetMessage(message);
        }

        public void SendPresetMessage(string prompt)
        {
            if (voiceTrigger != null)
            {
                voiceTrigger.ProcessPlayerInput(prompt);
            }
            else
            {
                Debug.LogWarning("[RemCompanionHUD] No RemVoiceTrigger found in scene!");
            }
        }
    }
}
