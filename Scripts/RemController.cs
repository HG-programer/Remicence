using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace Remniscence
{
    /// <summary>
    /// Main controller for Rem character - handles positioning, animations, and basic interactions
    /// </summary>
    public class RemController : MonoBehaviour
    {
        [Header("Character Settings")]
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float distanceFromPlayer = 2.0f;
        [SerializeField] private float heightOffset = 0.5f;
        [SerializeField] private float rotationSpeed = 2.0f;
        
        [Header("Animation")]
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private float idleAnimationInterval = 5.0f;
        
        [Header("Interaction")]
        [SerializeField] private LayerMask interactionLayer = 1;
        [SerializeField] private float maxInteractionDistance = 3.0f;
        
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private bool isPlayerNearby = false;
        private float lastIdleTime;
        
        // Animation state hashes for performance
        private readonly int idleHash = Animator.StringToHash("Idle");
        private readonly int waveHash = Animator.StringToHash("Wave");
        private readonly int lookAtPlayerHash = Animator.StringToHash("LookAtPlayer");
        
        void Start()
        {
            InitializeRemPosition();
            StartCoroutine(IdleAnimationLoop());
        }
        
        void Update()
        {
            UpdatePlayerTracking();
            UpdatePosition();
            UpdateRotation();
            CheckPlayerProximity();
        }
        
        /// <summary>
        /// Initialize Rem's starting position relative to player
        /// </summary>
        private void InitializeRemPosition()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main?.transform;
                if (playerCamera == null)
                {
                    Debug.LogWarning("RemController: No player camera found!");
                    return;
                }
            }
            
            // Position Rem in front of the player
            Vector3 forward = playerCamera.forward;
            forward.y = 0; // Keep on ground level
            forward.Normalize();
            
            targetPosition = playerCamera.position + forward * distanceFromPlayer;
            targetPosition.y += heightOffset;
            
            transform.position = targetPosition;
            
            // Face the player
            Vector3 lookDirection = (playerCamera.position - transform.position).normalized;
            lookDirection.y = 0;
            targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = targetRotation;
        }
        
        /// <summary>
        /// Update target position based on player movement
        /// </summary>
        private void UpdatePlayerTracking()
        {
            if (playerCamera == null) return;
            
            Vector3 playerPos = playerCamera.position;
            Vector3 currentDistance = transform.position - playerPos;
            
            // Only update position if player has moved significantly
            if (currentDistance.magnitude > distanceFromPlayer + 1.0f)
            {
                Vector3 forward = playerCamera.forward;
                forward.y = 0;
                forward.Normalize();
                
                targetPosition = playerPos + forward * distanceFromPlayer;
                targetPosition.y = playerPos.y + heightOffset;
            }
        }
        
        /// <summary>
        /// Smoothly move to target position
        /// </summary>
        private void UpdatePosition()
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2.0f);
        }
        
        /// <summary>
        /// Smoothly rotate to look at player
        /// </summary>
        private void UpdateRotation()
        {
            if (playerCamera == null) return;
            
            Vector3 lookDirection = (playerCamera.position - transform.position).normalized;
            lookDirection.y = 0;
            
            if (lookDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                    Time.deltaTime * rotationSpeed);
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
            
            // Trigger events when player enters/exits proximity
            if (isPlayerNearby && !wasNearby)
            {
                OnPlayerEnterProximity();
            }
            else if (!isPlayerNearby && wasNearby)
            {
                OnPlayerExitProximity();
            }
        }
        
        /// <summary>
        /// Handle player entering interaction range
        /// </summary>
        private void OnPlayerEnterProximity()
        {
            TriggerWaveAnimation();
            // Could trigger voice line here
        }
        
        /// <summary>
        /// Handle player leaving interaction range
        /// </summary>
        private void OnPlayerExitProximity()
        {
            // Return to idle state
            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger(idleHash);
            }
        }
        
        /// <summary>
        /// Trigger wave animation
        /// </summary>
        public void TriggerWaveAnimation()
        {
            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger(waveHash);
            }
        }
        
        /// <summary>
        /// Coroutine for random idle animations
        /// </summary>
        private IEnumerator IdleAnimationLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(idleAnimationInterval + Random.Range(-1f, 2f));
                
                if (!isPlayerNearby && characterAnimator != null)
                {
                    // Random idle variations
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
            
            // Get RemVoiceTrigger component and play voice line
            var voiceTrigger = GetComponent<RemVoiceTrigger>();
            if (voiceTrigger != null)
            {
                voiceTrigger.PlayRandomVoiceLine();
            }
        }
        
        /// <summary>
        /// Teleport Rem to a specific position
        /// </summary>
        public void TeleportTo(Vector3 position)
        {
            targetPosition = position;
            transform.position = position;
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw interaction range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, maxInteractionDistance);
            
            // Draw distance from player
            Gizmos.color = Color.green;
            if (playerCamera != null)
            {
                Gizmos.DrawLine(transform.position, playerCamera.position);
            }
        }
    }
}
