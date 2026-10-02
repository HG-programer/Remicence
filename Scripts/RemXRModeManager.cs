using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Remniscence
{
    public enum CompanionXRMode
    {
        Desktop,
        VR_Immersive,
        AR_Passthrough
    }

    /// <summary>
    /// Manages switching between VR (immersive 3D environment) and AR (real-world passthrough).
    /// Dynamically toggles background environments, lighting, camera clear flags, and AR plane placement.
    /// </summary>
    public class RemXRModeManager : MonoBehaviour
    {
        [Header("Active Mode")]
        [SerializeField] private CompanionXRMode currentMode = CompanionXRMode.VR_Immersive;

        [Header("Environment References")]
        [Tooltip("The parent GameObject containing the 3D room, furniture, and lighting for VR mode")]
        [SerializeField] private GameObject vrEnvironmentContainer;

        [Tooltip("Ambient particle system (e.g. floating mana or sakura petals)")]
        [SerializeField] private ParticleSystem ambientParticles;

        [Header("AR References (Optional - for mobile AR)")]
        [SerializeField] private ARPlaneManager arPlaneManager;
        [SerializeField] private ARRaycastManager arRaycastManager;

        [Header("Camera & Character")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform remCharacterTransform;

        public CompanionXRMode CurrentMode => currentMode;
        public event Action<CompanionXRMode> OnModeChanged;

        void Awake()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (remCharacterTransform == null)
            {
                var controller = FindObjectOfType<RemController>();
                if (controller != null) remCharacterTransform = controller.transform;
            }
        }

        void Start()
        {
            ApplyMode(currentMode);
        }

        /// <summary>
        /// Switch to a specific mode
        /// </summary>
        public void SetMode(CompanionXRMode newMode)
        {
            currentMode = newMode;
            ApplyMode(currentMode);
            OnModeChanged?.Invoke(currentMode);
        }

        /// <summary>
        /// Toggle between VR immersive room and AR passthrough
        /// </summary>
        public void ToggleVRAR()
        {
            if (currentMode == CompanionXRMode.VR_Immersive)
            {
                SetMode(CompanionXRMode.AR_Passthrough);
            }
            else
            {
                SetMode(CompanionXRMode.VR_Immersive);
            }
        }

        private void ApplyMode(CompanionXRMode mode)
        {
            if (mainCamera == null) mainCamera = Camera.main;

            switch (mode)
            {
                case CompanionXRMode.VR_Immersive:
                    ConfigureVRMode();
                    break;

                case CompanionXRMode.AR_Passthrough:
                    ConfigureARMode();
                    break;

                case CompanionXRMode.Desktop:
                    ConfigureDesktopMode();
                    break;
            }

            Debug.Log($"[RemXRModeManager] Mode set to: {mode}");
        }

        private void ConfigureVRMode()
        {
            // Enable rich 3D environment
            if (vrEnvironmentContainer != null)
            {
                vrEnvironmentContainer.SetActive(true);
            }

            if (ambientParticles != null)
            {
                ambientParticles.gameObject.SetActive(true);
                ambientParticles.Play();
            }

            // Disable AR plane visualization
            if (arPlaneManager != null) arPlaneManager.enabled = false;
            if (arRaycastManager != null) arRaycastManager.enabled = false;

            // Camera: Skybox rendering
            if (mainCamera != null)
            {
                mainCamera.clearFlags = CameraClearFlags.Skybox;
            }

            // Rem informs the user
            var voice = FindObjectOfType<RemVoiceTrigger>();
            voice?.ProcessPlayerInput("I have switched to immersive VR room mode. Welcome to our study space, Rem.");
        }

        private void ConfigureARMode()
        {
            // Hide static room in AR mode so real world is visible
            if (vrEnvironmentContainer != null)
            {
                vrEnvironmentContainer.SetActive(false);
            }

            // Camera: Transparent / Solid color for AR passthrough
            if (mainCamera != null)
            {
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = new Color(0, 0, 0, 0);
            }

            // Enable AR plane detection
            if (arPlaneManager != null) arPlaneManager.enabled = true;
            if (arRaycastManager != null) arRaycastManager.enabled = true;

            var voice = FindObjectOfType<RemVoiceTrigger>();
            voice?.ProcessPlayerInput("I have switched to AR passthrough mode. I am standing right here in your room, Rem.");
        }

        private void ConfigureDesktopMode()
        {
            if (vrEnvironmentContainer != null) vrEnvironmentContainer.SetActive(true);
            if (mainCamera != null) mainCamera.clearFlags = CameraClearFlags.Skybox;
            if (arPlaneManager != null) arPlaneManager.enabled = false;
        }

        /// <summary>
        /// Reposition Rem on an AR detected surface (e.g. desk or floor)
        /// </summary>
        public void AnchorRemToPosition(Vector3 worldPosition, Quaternion rotation)
        {
            if (remCharacterTransform != null)
            {
                remCharacterTransform.position = worldPosition;
                remCharacterTransform.rotation = rotation;

                var controller = remCharacterTransform.GetComponent<RemController>();
                controller?.TeleportTo(worldPosition);
            }
        }
    }
}
