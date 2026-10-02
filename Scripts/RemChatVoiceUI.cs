using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Remniscence
{
    /// <summary>
    /// Comprehensive in-game Chat & Voice Chat interface for Rem.
    /// Features:
    /// - Scrollable multi-turn conversation history (chat bubbles)
    /// - Live microphone Push-to-Talk voice chat with visual audio feedback
    /// - Text chat input field with send button
    /// - Quick prompt action chips (Tea, Coding Cheer, Water, Posture)
    /// - VR / AR mode toggle button
    /// - Auto-builds a stylish anime dark-glass VR/AR canvas if no prefab is assigned
    /// </summary>
    public class RemChatVoiceUI : MonoBehaviour
    {
        [Header("Target References")]
        [SerializeField] private RemVoiceTrigger voiceTrigger;
        [SerializeField] private RemVoiceInput voiceInput;
        [SerializeField] private RemXRModeManager xrModeManager;

        [Header("UI Canvas & Panels")]
        [SerializeField] private Canvas chatCanvas;
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private ScrollRect chatScrollRect;
        [SerializeField] private Transform messageContainer;

        [Header("Controls")]
        [SerializeField] private TMP_InputField chatInputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button pushToTalkButton;
        [SerializeField] private TextMeshProUGUI pushToTalkLabel;
        [SerializeField] private Button modeToggleButton;
        [SerializeField] private TextMeshProUGUI modeToggleLabel;

        [Header("Quick Chips")]
        [SerializeField] private Button teaChip;
        [SerializeField] private Button cheerChip;
        [SerializeField] private Button postureChip;
        [SerializeField] private Button waterChip;

        private readonly List<GameObject> messageBubbles = new List<GameObject>();
        private const int MaxMessages = 30;

        void Awake()
        {
            FindServices();
            EnsureUIHierarchy();
        }

        void Start()
        {
            SetupEventListeners();
            AppendMessage("Rem", "Hello, Harshit! Rem is here to accompany and support you. Hold to speak with voice, or type to me below.", false);
        }

        private void FindServices()
        {
            if (voiceTrigger == null) voiceTrigger = FindObjectOfType<RemVoiceTrigger>();
            if (voiceInput == null) voiceInput = FindObjectOfType<RemVoiceInput>();
            if (xrModeManager == null) xrModeManager = FindObjectOfType<RemXRModeManager>();
        }

        private void SetupEventListeners()
        {
            if (sendButton != null) sendButton.onClick.AddListener(OnSendClicked);
            if (chatInputField != null) chatInputField.onSubmit.AddListener((s) => OnSendClicked());

            if (modeToggleButton != null)
            {
                modeToggleButton.onClick.AddListener(OnModeToggleClicked);
                UpdateModeLabel();
            }

            // Quick Chips
            if (teaChip != null) teaChip.onClick.AddListener(() => SendUserMessage("Rem, could you make some warm tea for us?"));
            if (cheerChip != null) cheerChip.onClick.AddListener(() => voiceTrigger?.PlayEncouragement());
            if (postureChip != null) postureChip.onClick.AddListener(() => SendUserMessage("Rem, please check how I'm sitting."));
            if (waterChip != null) waterChip.onClick.AddListener(() => SendUserMessage("Rem, remind me to drink some water."));

            SetupPushToTalkEvents();
        }

        private void SetupPushToTalkEvents()
        {
            if (pushToTalkButton == null) return;

            EventTrigger trigger = pushToTalkButton.gameObject.GetComponent<EventTrigger>() ?? pushToTalkButton.gameObject.AddComponent<EventTrigger>();

            // Pointer Down
            var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            down.callback.AddListener((d) =>
            {
                if (voiceInput != null)
                {
                    voiceInput.StartRecording();
                    if (pushToTalkLabel != null) pushToTalkLabel.text = "🔴 Recording... Release";
                }
            });
            trigger.triggers.Add(down);

            // Pointer Up
            var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            up.callback.AddListener((d) =>
            {
                if (voiceInput != null)
                {
                    voiceInput.StopAndSendRecording();
                    if (pushToTalkLabel != null) pushToTalkLabel.text = "🎙️ Hold to Speak";
                }
            });
            trigger.triggers.Add(up);
        }

        public void OnSendClicked()
        {
            if (chatInputField == null || string.IsNullOrWhiteSpace(chatInputField.text)) return;

            string text = chatInputField.text.Trim();
            chatInputField.text = "";

            SendUserMessage(text);
        }

        private void SendUserMessage(string text)
        {
            AppendMessage("You", text, true);

            if (voiceTrigger != null)
            {
                voiceTrigger.ProcessPlayerInput(text);
            }
        }

        /// <summary>
        /// Append a chat bubble to the scroll view
        /// </summary>
        public void AppendMessage(string sender, string message, bool isUser)
        {
            if (messageContainer == null) return;

            GameObject bubble = new GameObject("ChatBubble_" + sender);
            bubble.transform.SetParent(messageContainer, false);

            var rect = bubble.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(340, 60);

            var img = bubble.AddComponent<Image>();
            // User = elegant blue, Rem = soft navy/cyan
            img.color = isUser ? new Color(0.12f, 0.35f, 0.65f, 0.9f) : new Color(0.12f, 0.16f, 0.26f, 0.9f);

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(bubble.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12, 8);
            textRect.offsetMax = new Vector2(-12, -8);

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = $"<b>{(isUser ? "You" : "🌸 Rem")}:</b> {message}";
            tmp.fontSize = 13;
            tmp.color = Color.white;
            tmp.textWrappingMode = TextWrappingModes.Normal;

            messageBubbles.Add(bubble);
            if (messageBubbles.Count > MaxMessages)
            {
                Destroy(messageBubbles[0]);
                messageBubbles.RemoveAt(0);
            }

            // Scroll to bottom
            Canvas.ForceUpdateCanvases();
            if (chatScrollRect != null) chatScrollRect.verticalNormalizedPosition = 0f;
        }

        private void OnModeToggleClicked()
        {
            if (xrModeManager != null)
            {
                xrModeManager.ToggleVRAR();
                UpdateModeLabel();
            }
        }

        private void UpdateModeLabel()
        {
            if (modeToggleLabel == null || xrModeManager == null) return;
            modeToggleLabel.text = xrModeManager.CurrentMode == CompanionXRMode.VR_Immersive ? "📱 Switch to AR" : "🥽 Switch to VR";
        }

        /// <summary>
        /// Procedurally construct the Chat & Voice UI Canvas if missing
        /// </summary>
        private void EnsureUIHierarchy()
        {
            if (chatCanvas != null && mainPanel != null && messageContainer != null) return;

            chatCanvas = GetComponentInChildren<Canvas>();
            if (chatCanvas == null)
            {
                GameObject canvasObj = new GameObject("Rem_ChatVoice_WorldCanvas");
                canvasObj.transform.SetParent(transform, false);
                chatCanvas = canvasObj.AddComponent<Canvas>();
                chatCanvas.renderMode = RenderMode.WorldSpace;
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10;
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            var canvasRect = chatCanvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(400, 520);
            canvasRect.localScale = Vector3.one * 0.0022f; // Comfortable VR distance scale
            canvasRect.localPosition = new Vector3(-0.85f, 1.25f, 1.8f);
            canvasRect.localRotation = Quaternion.Euler(0, 20, 0);

            if (mainPanel == null)
            {
                GameObject panelObj = new GameObject("ChatPanel");
                panelObj.transform.SetParent(chatCanvas.transform, false);
                mainPanel = panelObj.AddComponent<RectTransform>();
                mainPanel.anchorMin = Vector2.zero;
                mainPanel.anchorMax = Vector2.one;
                mainPanel.sizeDelta = Vector2.zero;

                var bg = panelObj.AddComponent<Image>();
                bg.color = new Color(0.08f, 0.10f, 0.16f, 0.94f); // Anime midnight glass
            }

            // Scroll view
            if (messageContainer == null)
            {
                GameObject scrollObj = new GameObject("ScrollView");
                scrollObj.transform.SetParent(mainPanel, false);
                var scrollRect = scrollObj.AddComponent<RectTransform>();
                scrollRect.anchorMin = new Vector2(0, 0.28f);
                scrollRect.anchorMax = new Vector2(1, 0.88f);
                scrollRect.offsetMin = new Vector2(15, 0);
                scrollRect.offsetMax = new Vector2(-15, 0);

                chatScrollRect = scrollObj.AddComponent<ScrollRect>();

                GameObject viewport = new GameObject("Viewport");
                viewport.transform.SetParent(scrollObj.transform, false);
                var viewRect = viewport.AddComponent<RectTransform>();
                viewRect.anchorMin = Vector2.zero;
                viewRect.anchorMax = Vector2.one;
                viewRect.sizeDelta = Vector2.zero;
                viewport.AddComponent<Mask>().showMaskGraphic = false;
                viewport.AddComponent<Image>();

                GameObject content = new GameObject("Content");
                content.transform.SetParent(viewport.transform, false);
                messageContainer = content.AddComponent<RectTransform>();
                var contentRect = (RectTransform)messageContainer;
                contentRect.anchorMin = new Vector2(0, 1);
                contentRect.anchorMax = new Vector2(1, 1);
                contentRect.pivot = new Vector2(0.5f, 1);

                var layout = content.AddComponent<VerticalLayoutGroup>();
                layout.spacing = 8;
                layout.childControlHeight = false;
                layout.childControlWidth = true;

                var fitter = content.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                chatScrollRect.content = contentRect;
                chatScrollRect.viewport = viewRect;
                chatScrollRect.horizontal = false;
            }

            // Input field
            if (chatInputField == null)
            {
                GameObject inputObj = new GameObject("InputField");
                inputObj.transform.SetParent(mainPanel, false);
                var inputRect = inputObj.AddComponent<RectTransform>();
                inputRect.anchorMin = new Vector2(0, 0.08f);
                inputRect.anchorMax = new Vector2(0.72f, 0.18f);
                inputRect.offsetMin = new Vector2(15, 0);
                inputRect.offsetMax = new Vector2(0, 0);

                var inputBg = inputObj.AddComponent<Image>();
                inputBg.color = new Color(0.15f, 0.18f, 0.28f, 1f);

                chatInputField = inputObj.AddComponent<TMP_InputField>();
                
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(inputObj.transform, false);
                var tRect = textObj.AddComponent<RectTransform>();
                tRect.anchorMin = Vector2.zero;
                tRect.anchorMax = Vector2.one;
                tRect.offsetMin = new Vector2(10, 0);
                tRect.offsetMax = new Vector2(-10, 0);
                var tComp = textObj.AddComponent<TextMeshProUGUI>();
                tComp.fontSize = 13;
                tComp.color = Color.white;
                chatInputField.textComponent = tComp;
            }

            // Send button
            if (sendButton == null)
            {
                GameObject sendObj = new GameObject("SendBtn");
                sendObj.transform.SetParent(mainPanel, false);
                var sRect = sendObj.AddComponent<RectTransform>();
                sRect.anchorMin = new Vector2(0.74f, 0.08f);
                sRect.anchorMax = new Vector2(1, 0.18f);
                sRect.offsetMin = new Vector2(5, 0);
                sRect.offsetMax = new Vector2(-15, 0);

                var sBg = sendObj.AddComponent<Image>();
                sBg.color = new Color(0.02f, 0.52f, 0.78f, 1f); // Rem blue
                sendButton = sendObj.AddComponent<Button>();

                var label = new GameObject("Label").AddComponent<TextMeshProUGUI>();
                label.transform.SetParent(sendObj.transform, false);
                label.text = "Send";
                label.alignment = TextAlignmentOptions.Center;
                label.fontSize = 13;
                label.color = Color.white;
            }

            // Push to talk button
            if (pushToTalkButton == null)
            {
                GameObject pttObj = new GameObject("PushToTalkBtn");
                pttObj.transform.SetParent(mainPanel, false);
                var pttRect = pttObj.AddComponent<RectTransform>();
                pttRect.anchorMin = new Vector2(0, 0);
                pttRect.anchorMax = new Vector2(1, 0.07f);
                pttRect.offsetMin = new Vector2(15, 0);
                pttRect.offsetMax = new Vector2(-15, 0);

                var pttBg = pttObj.AddComponent<Image>();
                pttBg.color = new Color(0.18f, 0.22f, 0.35f, 1f);
                pushToTalkButton = pttObj.AddComponent<Button>();

                var label = new GameObject("PTTLabel").AddComponent<TextMeshProUGUI>();
                label.transform.SetParent(pttObj.transform, false);
                label.text = "🎙️ Hold to Speak (or hold 'V')";
                label.alignment = TextAlignmentOptions.Center;
                label.fontSize = 13;
                label.color = new Color(0.6f, 0.85f, 1f);
                pushToTalkLabel = label;
            }
        }
    }
}
