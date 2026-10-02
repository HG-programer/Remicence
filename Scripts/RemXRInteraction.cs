using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.XR.Interaction.Toolkit;

namespace Remniscence
{
    /// <summary>
    /// Handles XR and Desktop interactions with Rem.
    /// Compatible with both the New Unity Input System and Legacy Input without throwing errors.
    /// Supports VR Ray interactors, AR Touch, and Desktop Hotkeys.
    /// </summary>
    public class RemXRInteraction : MonoBehaviour
    {
        [Header("XR Interaction")]
        [SerializeField] private XRRayInteractor leftRayInteractor;
        [SerializeField] private XRRayInteractor rightRayInteractor;
        [SerializeField] private LayerMask interactableLayer = ~0;

        [Header("Touch & Pointer Input (AR / Desktop)")]
        [SerializeField] private bool enablePointerInput = true;
        [SerializeField] private Camera interactionCamera;

        [Header("Desktop Test Hotkeys")]
        [SerializeField] private KeyCode voiceTestKey = KeyCode.Space;
        [SerializeField] private KeyCode greetingKey = KeyCode.G;
        [SerializeField] private KeyCode encouragementKey = KeyCode.E;
        [SerializeField] private KeyCode wakeWordTestKey = KeyCode.R;

        private RemController remController;
        private RemVoiceTrigger remVoiceTrigger;

        void Awake()
        {
            InitializeComponents();
            SetupXRInteraction();
        }

        void Update()
        {
            HandlePointerAndTouchInput();
            HandleKeyboardInput();
        }

        private void InitializeComponents()
        {
            remController = GetComponent<RemController>();
            remVoiceTrigger = GetComponent<RemVoiceTrigger>();

            if (interactionCamera == null)
            {
                interactionCamera = Camera.main;
            }
        }

        private void SetupXRInteraction()
        {
            var xrInteractable = GetComponent<XRBaseInteractable>();
            if (xrInteractable == null)
            {
                xrInteractable = gameObject.AddComponent<XRSimpleInteractable>();
            }

            xrInteractable.selectEntered.AddListener(OnXRSelect);
            xrInteractable.hoverEntered.AddListener(OnXRHoverEnter);
            xrInteractable.hoverExited.AddListener(OnXRHoverExit);

            // Ensure trigger collider exists for raycast/touch
            var col = GetComponent<Collider>();
            if (col == null)
            {
                var capsule = gameObject.AddComponent<CapsuleCollider>();
                capsule.height = 1.6f;
                capsule.radius = 0.4f;
                capsule.center = new Vector3(0, 0.8f, 0);
                capsule.isTrigger = true;
            }
        }

        /// <summary>
        /// Unified input handling for legacy & new input systems to avoid runtime exceptions
        /// </summary>
        private void HandlePointerAndTouchInput()
        {
            if (!enablePointerInput) return;
            if (interactionCamera == null) interactionCamera = Camera.main;
            if (interactionCamera == null) return;

            bool pointerPressed = false;
            Vector2 screenPos = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                pointerPressed = true;
                screenPos = Mouse.current.position.ReadValue();
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                pointerPressed = true;
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }
#else
            if (Input.GetMouseButtonDown(0))
            {
                pointerPressed = true;
                screenPos = Input.mousePosition;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                pointerPressed = true;
                screenPos = Input.GetTouch(0).position;
            }
#endif

            if (pointerPressed)
            {
                Ray ray = interactionCamera.ScreenPointToRay(screenPos);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
                {
                    var prop = hit.collider.GetComponentInParent<RemInteractiveProp>();
                    if (prop != null)
                    {
                        prop.Interact();
                    }
                    else if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
                    {
                        OnPlayerInteract();
                    }
                }
            }
        }

        private void HandleKeyboardInput()
        {
            bool testPressed = false;
            bool greetingPressed = false;
            bool encouragePressed = false;
            bool wakeWordPressed = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                testPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
                greetingPressed = Keyboard.current.gKey.wasPressedThisFrame;
                encouragePressed = Keyboard.current.eKey.wasPressedThisFrame;
                wakeWordPressed = Keyboard.current.rKey.wasPressedThisFrame;
            }
#else
            testPressed = Input.GetKeyDown(voiceTestKey);
            greetingPressed = Input.GetKeyDown(greetingKey);
            encouragePressed = Input.GetKeyDown(encouragementKey);
            wakeWordPressed = Input.GetKeyDown(wakeWordTestKey);
#endif

            if (testPressed)
            {
                remVoiceTrigger?.PlayRandomVoiceLine();
            }
            else if (greetingPressed)
            {
                remVoiceTrigger?.PlayGreeting();
            }
            else if (encouragePressed)
            {
                remVoiceTrigger?.PlayEncouragement();
            }
            else if (wakeWordPressed)
            {
                remVoiceTrigger?.OnTriggerWordDetected();
            }
        }

        private void OnXRSelect(SelectEnterEventArgs args)
        {
            OnPlayerInteract();
        }

        private void OnXRHoverEnter(HoverEnterEventArgs args)
        {
            // Hover feedback can be added here
        }

        private void OnXRHoverExit(HoverExitEventArgs args)
        {
        }

        public void OnPlayerInteract()
        {
            remController?.OnPlayerInteract();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.8f, 0.4f);
        }
    }
}
