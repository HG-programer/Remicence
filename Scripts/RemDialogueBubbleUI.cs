using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Remniscence
{
    /// <summary>
    /// World-space dialogue bubble and companion status UI for Rem.
    /// Floats above Rem's head, faces the player camera (billboard), and displays typewriter subtitles.
    /// Can also auto-construct a clean UI if no prefab is assigned.
    /// </summary>
    public class RemDialogueBubbleUI : MonoBehaviour
    {
        [Header("Target & Positioning")]
        [Tooltip("Transform to float above (defaults to this GameObject)")]
        [SerializeField] private Transform characterHead;
        [SerializeField] private Vector3 offset = new Vector3(0, 1.85f, 0);
        [SerializeField] private float smoothFollowSpeed = 6.0f;

        [Header("UI References")]
        [SerializeField] private Canvas worldSpaceCanvas;
        [SerializeField] private RectTransform bubblePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI statusBadge;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Dialogue Animation")]
        [SerializeField] private float typeWriterSpeed = 0.035f;
        [SerializeField] private float displayDurationPerCharacter = 0.06f;
        [SerializeField] private float minimumDisplayDuration = 3.5f;

        private Transform playerCameraTransform;
        private Coroutine typingCoroutine;
        private Coroutine hideCoroutine;
        private bool isSpeaking = false;

        void Awake()
        {
            if (characterHead == null)
            {
                characterHead = transform;
            }

            EnsureUIHierarchy();
        }

        void Start()
        {
            LocateCamera();
            SetStatus("Ready");
            SetVisibility(false, true);
        }

        void LateUpdate()
        {
            UpdateBillboardPositionAndRotation();
        }

        private void LocateCamera()
        {
            if (Camera.main != null)
            {
                playerCameraTransform = Camera.main.transform;
            }
        }

        private void UpdateBillboardPositionAndRotation()
        {
            if (playerCameraTransform == null)
            {
                LocateCamera();
                if (playerCameraTransform == null) return;
            }

            if (bubblePanel == null) return;

            // Position above Rem's head smoothly
            Vector3 desiredPosition = characterHead.position + offset;
            bubblePanel.position = Vector3.Lerp(bubblePanel.position, desiredPosition, Time.deltaTime * smoothFollowSpeed);

            // Billboard: rotate to look directly at the player's camera
            Vector3 lookDir = bubblePanel.position - playerCameraTransform.position;
            if (lookDir != Vector3.zero)
            {
                bubblePanel.rotation = Quaternion.LookRotation(lookDir);
            }
        }

        /// <summary>
        /// Display a message from Rem with typewriter animation
        /// </summary>
        public void ShowDialogue(string message, float? customDuration = null)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            if (hideCoroutine != null) StopCoroutine(hideCoroutine);

            SetVisibility(true);
            typingCoroutine = StartCoroutine(TypeWriterRoutine(message, customDuration));
        }

        private IEnumerator TypeWriterRoutine(string message, float? customDuration)
        {
            if (dialogueText == null) yield break;

            dialogueText.text = "";
            for (int i = 0; i < message.Length; i++)
            {
                dialogueText.text += message[i];
                yield return new WaitForSeconds(typeWriterSpeed);
            }

            // Calculate display hold time based on text length or custom duration
            float holdTime = customDuration ?? Mathf.Max(minimumDisplayDuration, message.Length * displayDurationPerCharacter);
            
            // Wait for duration before fading out, unless currently speaking
            while (isSpeaking)
            {
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(holdTime);
            SetVisibility(false);
        }

        /// <summary>
        /// Update companion status badge (e.g., "Thinking...", "Speaking...", "Listening...")
        /// </summary>
        public void SetStatus(string status)
        {
            if (statusBadge != null)
            {
                statusBadge.text = status;
                statusBadge.gameObject.SetActive(!string.IsNullOrEmpty(status));
            }
        }

        public void SetSpeakingState(bool speaking)
        {
            isSpeaking = speaking;
            if (speaking)
            {
                SetStatus("🌸 Speaking...");
            }
            else
            {
                SetStatus("");
            }
        }

        public void SetThinkingState(bool thinking)
        {
            if (thinking)
            {
                SetVisibility(true);
                SetStatus("💭 Rem is thinking...");
                if (dialogueText != null) dialogueText.text = "...";
            }
            else
            {
                SetStatus("");
            }
        }

        public void SetVisibility(bool visible, bool instant = false)
        {
            if (canvasGroup == null) return;

            if (hideCoroutine != null) StopCoroutine(hideCoroutine);

            if (instant)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
            }
            else
            {
                hideCoroutine = StartCoroutine(FadeCanvasGroup(visible ? 1f : 0f, 0.25f));
            }
        }

        private IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            canvasGroup.interactable = targetAlpha > 0.5f;
            canvasGroup.blocksRaycasts = targetAlpha > 0.5f;
        }

        /// <summary>
        /// Construct procedural UI if not pre-configured in Inspector
        /// </summary>
        private void EnsureUIHierarchy()
        {
            if (worldSpaceCanvas != null && bubblePanel != null && dialogueText != null) return;

            // Check if child canvas already exists
            worldSpaceCanvas = GetComponentInChildren<Canvas>();
            if (worldSpaceCanvas == null)
            {
                GameObject canvasObj = new GameObject("Rem_WorldDialogueCanvas");
                canvasObj.transform.SetParent(transform, false);
                worldSpaceCanvas = canvasObj.AddComponent<Canvas>();
                worldSpaceCanvas.renderMode = RenderMode.WorldSpace;
                
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10;
                
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            RectTransform canvasRect = worldSpaceCanvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(400, 200);
            canvasRect.localScale = Vector3.one * 0.003f; // VR/AR friendly scale

            if (bubblePanel == null)
            {
                GameObject panelObj = new GameObject("BubblePanel");
                panelObj.transform.SetParent(worldSpaceCanvas.transform, false);
                bubblePanel = panelObj.AddComponent<RectTransform>();
                bubblePanel.sizeDelta = new Vector2(380, 150);
                bubblePanel.localPosition = Vector3.zero;

                var bgImage = panelObj.AddComponent<Image>();
                bgImage.color = new Color(0.12f, 0.14f, 0.22f, 0.88f); // Soft dark navy background

                canvasGroup = panelObj.AddComponent<CanvasGroup>();
            }

            if (statusBadge == null)
            {
                GameObject badgeObj = new GameObject("StatusBadge");
                badgeObj.transform.SetParent(bubblePanel, false);
                var badgeRect = badgeObj.AddComponent<RectTransform>();
                badgeRect.anchorMin = new Vector2(0, 1);
                badgeRect.anchorMax = new Vector2(1, 1);
                badgeRect.pivot = new Vector2(0.5f, 1);
                badgeRect.sizeDelta = new Vector2(-20, 25);
                badgeRect.anchoredPosition = new Vector2(0, -10);

                statusBadge = badgeObj.AddComponent<TextMeshProUGUI>();
                statusBadge.fontSize = 14;
                statusBadge.color = new Color(0.6f, 0.85f, 1f); // Soft Rem cyan
                statusBadge.alignment = TextAlignmentOptions.Center;
                statusBadge.text = "";
            }

            if (dialogueText == null)
            {
                GameObject textObj = new GameObject("DialogueText");
                textObj.transform.SetParent(bubblePanel, false);
                var textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.pivot = new Vector2(0.5f, 0.5f);
                textRect.sizeDelta = new Vector2(-30, -50);
                textRect.anchoredPosition = new Vector2(0, -12);

                dialogueText = textObj.AddComponent<TextMeshProUGUI>();
                dialogueText.fontSize = 18;
                dialogueText.color = Color.white;
                dialogueText.alignment = TextAlignmentOptions.MidlineLeft;
                dialogueText.textWrappingMode = TextWrappingModes.Normal;
                dialogueText.text = "";
            }
        }
    }
}
