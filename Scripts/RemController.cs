using UnityEngine;
using System.Collections;

namespace Remniscence
{
    /// <summary>
    /// Main controller for Rem - handles smooth positioning, head look-at, organic animations, and proximity detection.
    /// Glitch-free smooth damping and robust camera discovery for both Desktop and XR modes.
    /// </summary>
    public class RemController : MonoBehaviour
    {
        [Header("Player Tracking")]
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float distanceFromPlayer = 1.8f;
        [SerializeField] private float heightOffset = 0.0f;
        [SerializeField] private float positionSmoothTime = 0.6f;
        [SerializeField] private float rotationSpeed = 3.5f;
        [SerializeField] private float repositionThreshold = 0.8f;

        [Header("Animation")]
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private float idleAnimationInterval = 6.0f;
        [SerializeField] private bool lookAtPlayerHead = true;

        [Header("Interaction Proximity")]
        [SerializeField] private float maxInteractionDistance = 2.8f;

        private Vector3 currentVelocity;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private bool isPlayerNearby = false;

        // Animator parameters
        private int idleHash;
        private int waveHash;
        private bool hasIdleVariationParam = false;

        void Awake()
        {
            if (characterAnimator == null)
            {
                characterAnimator = GetComponentInChildren<Animator>();
            }

            idleHash = Animator.StringToHash("Idle");
            waveHash = Animator.StringToHash("Wave");

            CheckAnimatorParameters();
        }

        void Start()
        {
            EnsurePlayerCamera();
            InitializeRemPosition();
            StartCoroutine(IdleAnimationLoop());
        }

        void Update()
        {
            if (playerCamera == null)
            {
                EnsurePlayerCamera();
                if (playerCamera == null) return;
            }

            UpdatePlayerTracking();
            UpdatePosition();
            UpdateRotation();
            CheckPlayerProximity();
        }

        /// <summary>
        /// Robust search for player camera across standard, XR Origin, and WebXR setups
        /// </summary>
        private void EnsurePlayerCamera()
        {
            if (playerCamera != null) return;

            if (Camera.main != null)
            {
                playerCamera = Camera.main.transform;
                return;
            }

            // Fallback: look for any camera tagged or active in scene
            var cam = FindObjectOfType<Camera>();
            if (cam != null)
            {
                playerCamera = cam.transform;
            }
        }

        private void CheckAnimatorParameters()
        {
            if (characterAnimator == null) return;

            foreach (var param in characterAnimator.parameters)
            {
                if (param.name == "IdleVariation")
                {
                    hasIdleVariationParam = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Initialize Rem's starting position in front of player
        /// </summary>
        private void InitializeRemPosition()
        {
            if (playerCamera == null) return;

            Vector3 forward = playerCamera.forward;
            forward.y = 0;
            if (forward == Vector3.zero) forward = Vector3.forward;
            forward.Normalize();

            targetPosition = playerCamera.position + forward * distanceFromPlayer;
            targetPosition.y = playerCamera.position.y + heightOffset;

            transform.position = targetPosition;

            // Face the player
            Vector3 lookDirection = (playerCamera.position - transform.position);
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation;
            }
        }

        /// <summary>
        /// Update target position smoothly when player moves beyond reposition threshold
        /// </summary>
        private void UpdatePlayerTracking()
        {
            if (playerCamera == null) return;

            Vector3 playerPos = playerCamera.position;
            Vector3 diff = transform.position - playerPos;
            diff.y = 0;

            float currentDistance = diff.magnitude;

            // Reposition smoothly if player moves too far or too close
            if (Mathf.Abs(currentDistance - distanceFromPlayer) > repositionThreshold)
            {
                Vector3 forward = playerCamera.forward;
                forward.y = 0;
                if (forward == Vector3.zero) forward = Vector3.forward;
                forward.Normalize();

                targetPosition = playerPos + forward * distanceFromPlayer;
                targetPosition.y = playerPos.y + heightOffset;
            }
        }

        /// <summary>
        /// Smooth organic dampening to eliminate jittering
        /// </summary>
        private void UpdatePosition()
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, positionSmoothTime);
        }

        /// <summary>
        /// Smoothly rotate towards player
        /// </summary>
        private void UpdateRotation()
        {
            if (playerCamera == null) return;

            Vector3 lookDirection = playerCamera.position - transform.position;
            lookDirection.y = 0;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                targetRotation = Quaternion.LookRotation(lookDirection.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        /// <summary>
        /// Check if player is within interaction range
        /// </summary>
        private void CheckPlayerProximity()
        {
            if (playerCamera == null) return;

            float distance = Vector3.Distance(transform.position, playerCamera.position);
            bool wasNearby = isPlayerNearby;
            isPlayerNearby = distance <= maxInteractionDistance;

            if (isPlayerNearby && !wasNearby)
            {
                OnPlayerEnterProximity();
            }
            else if (!isPlayerNearby && wasNearby)
            {
                OnPlayerExitProximity();
            }
        }

        private void OnPlayerEnterProximity()
        {
            TriggerWaveAnimation();
        }

        private void OnPlayerExitProximity()
        {
            if (characterAnimator != null && characterAnimator.HasState(0, idleHash))
            {
                characterAnimator.SetTrigger(idleHash);
            }
        }

        public void TriggerWaveAnimation()
        {
            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger(waveHash);
            }
        }

        private IEnumerator IdleAnimationLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(idleAnimationInterval + Random.Range(-1.5f, 2.5f));

                if (!isPlayerNearby && characterAnimator != null && hasIdleVariationParam)
                {
                    int randomIdle = Random.Range(0, 3);
                    characterAnimator.SetInteger("IdleVariation", randomIdle);
                }
            }
        }

        /// <summary>
        /// Called when player interacts with Rem (via XR or mouse click)
        /// </summary>
        public void OnPlayerInteract()
        {
            TriggerWaveAnimation();

            var voiceTrigger = GetComponent<RemVoiceTrigger>();
            if (voiceTrigger != null)
            {
                voiceTrigger.PlayRandomVoiceLine();
            }
        }

        public void TeleportTo(Vector3 position)
        {
            targetPosition = position;
            transform.position = position;
            currentVelocity = Vector3.zero;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, maxInteractionDistance);

            Gizmos.color = Color.green;
            if (playerCamera != null)
            {
                Gizmos.DrawLine(transform.position, playerCamera.position);
            }
        }
    }
}
