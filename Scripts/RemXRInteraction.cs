using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Remniscence
{
    /// <summary>
    /// Handles XR input for interacting with Rem character
    /// Supports both VR controllers and AR touch input
    /// </summary>
    public class RemXRInteraction : MonoBehaviour
    {
        [Header("XR Interaction Settings")]
        [SerializeField] private XRRayInteractor leftRayInteractor;
        [SerializeField] private XRRayInteractor rightRayInteractor;
        [SerializeField] private LayerMask interactableLayer = 1;
        
        [Header("Touch Input (AR Mode)")]
        [SerializeField] private bool enableTouchInput = true;
        [SerializeField] private Camera arCamera;
        
        [Header("Voice Commands")]
        [SerializeField] private bool enableVoiceCommands = true;
        [SerializeField] private KeyCode voiceTestKey = KeyCode.Space;
        
        private RemController remController;
        private RemVoiceTrigger remVoiceTrigger;
        
        void Start()
        {
            InitializeComponents();
            SetupXRInteraction();
        }
        
        void Update()
        {
            HandleTouchInput();
            HandleKeyboardInput(); // For testing in editor
        }
        
        /// <summary>
        /// Initialize required components
        /// </summary>
        private void InitializeComponents()
        {
            remController = GetComponent<RemController>();
            remVoiceTrigger = GetComponent<RemVoiceTrigger>();
            
            if (arCamera == null)
            {
                arCamera = Camera.main;
            }
        }
        
        /// <summary>
        /// Setup XR interaction system
        /// </summary>
        private void SetupXRInteraction()
        {
            // Add XR interactable component if not present
            var xrInteractable = GetComponent<XRBaseInteractable>();
            if (xrInteractable == null)
            {
                xrInteractable = gameObject.AddComponent<XRSimpleInteractable>();
            }
            
            // Subscribe to interaction events
            xrInteractable.selectEntered.AddListener(OnXRSelect);
            xrInteractable.hoverEntered.AddListener(OnXRHoverEnter);
            xrInteractable.hoverExited.AddListener(OnXRHoverExit);
            
            // Setup collider for interaction
            var collider = GetComponent<Collider>();
            if (collider == null)
            {
                var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.height = 1.8f;
                capsuleCollider.radius = 0.5f;
                capsuleCollider.center = new Vector3(0, 0.9f, 0);
                capsuleCollider.isTrigger = true;
            }
        }
        
        /// <summary>
        /// Handle touch input for AR interactions
        /// </summary>
        private void HandleTouchInput()
        {
            if (!enableTouchInput || arCamera == null) return;
            
            #if UNITY_EDITOR || UNITY_STANDALONE
            // Mouse input for testing in editor
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
                HandleRaycast(ray);
            }
            #elif UNITY_ANDROID || UNITY_IOS
            // Touch input for mobile AR
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(touch.position);
                    HandleRaycast(ray);
                }
            }
            #endif
        }
        
        /// <summary>
        /// Handle raycast for touch/mouse input
        /// </summary>
        private void HandleRaycast(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    OnPlayerTouch();
                }
            }
        }
        
        /// <summary>
        /// Handle keyboard input for testing
        /// </summary>
        private void HandleKeyboardInput()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(voiceTestKey))
            {
                TestVoiceInteraction();
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                remVoiceTrigger?.PlayGreeting();
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                remVoiceTrigger?.PlayEncouragement();
            }
            #endif
        }
        
        /// <summary>
        /// Called when XR controller selects Rem
        /// </summary>
        private void OnXRSelect(SelectEnterEventArgs args)
        {
            Debug.Log("XR Select: Player interacted with Rem");
            OnPlayerInteract();
        }
        
        /// <summary>
        /// Called when XR controller hovers over Rem
        /// </summary>
        private void OnXRHoverEnter(HoverEnterEventArgs args)
        {
            Debug.Log("XR Hover Enter: Player looking at Rem");
            // Could add visual feedback here (glow effect, etc.)
        }
        
        /// <summary>
        /// Called when XR controller stops hovering over Rem
        /// </summary>
        private void OnXRHoverExit(HoverExitEventArgs args)
        {
            Debug.Log("XR Hover Exit: Player looked away from Rem");
            // Remove visual feedback
        }
        
        /// <summary>
        /// Called when player touches Rem (AR mode)
        /// </summary>
        private void OnPlayerTouch()
        {
            Debug.Log("Touch: Player touched Rem");
            OnPlayerInteract();
        }
        
        /// <summary>
        /// Main interaction handler
        /// </summary>
        private void OnPlayerInteract()
        {
            remController?.OnPlayerInteract();
        }
        
        /// <summary>
        /// Test voice interaction (for development)
        /// </summary>
        private void TestVoiceInteraction()
        {
            remVoiceTrigger?.PlayRandomVoiceLine();
        }
        
        /// <summary>
        /// Enable/disable XR interaction
        /// </summary>
        public void SetXRInteractionEnabled(bool enabled)
        {
            var xrInteractable = GetComponent<XRBaseInteractable>();
            if (xrInteractable != null)
            {
                xrInteractable.enabled = enabled;
            }
        }
        
        /// <summary>
        /// Enable/disable touch interaction
        /// </summary>
        public void SetTouchInteractionEnabled(bool enabled)
        {
            enableTouchInput = enabled;
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw interaction sphere
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.9f, 0.5f);
        }
    }
}
