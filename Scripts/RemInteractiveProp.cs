using UnityEngine;
using UnityEngine.Events;

namespace Remniscence
{
    /// <summary>
    /// Interactive background prop in Rem's VR environment (Tea Set, Bookshelf, Window, Fireplace, Desk).
    /// When clicked or targeted with XR raycast, triggers visual feedback and prompts Rem to comment live.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RemInteractiveProp : MonoBehaviour
    {
        [Header("Prop Details")]
        [SerializeField] private string propName = "Tea Set";
        [TextArea(2, 4)]
        [SerializeField] private string contextualAIPrompt = "Harshit just interacted with the tea set on the table. Offer to pour him some fresh tea and remind him to relax.";

        [Header("Visual Feedback")]
        [SerializeField] private Color highlightColor = new Color(0.4f, 0.8f, 1f, 1f);
        [SerializeField] private ParticleSystem interactionParticles;
        [SerializeField] private AudioSource interactionSound;

        [Header("Events")]
        public UnityEvent onInteracted;

        private Renderer propRenderer;
        private Color[] originalColors;
        private bool isHovered = false;

        void Awake()
        {
            propRenderer = GetComponent<Renderer>();
            if (propRenderer != null && propRenderer.materials.Length > 0)
            {
                originalColors = new Color[propRenderer.materials.Length];
                for (int i = 0; i < propRenderer.materials.Length; i++)
                {
                    if (propRenderer.materials[i].HasProperty("_Color"))
                    {
                        originalColors[i] = propRenderer.materials[i].color;
                    }
                }
            }
        }

        public void OnHoverEnter()
        {
            if (isHovered) return;
            isHovered = true;

            // Highlight glow
            if (propRenderer != null && propRenderer.materials.Length > 0)
            {
                for (int i = 0; i < propRenderer.materials.Length; i++)
                {
                    if (propRenderer.materials[i].HasProperty("_EmissionColor"))
                    {
                        propRenderer.materials[i].EnableKeyword("_EMISSION");
                        propRenderer.materials[i].SetColor("_EmissionColor", highlightColor * 0.4f);
                    }
                }
            }
        }

        public void OnHoverExit()
        {
            if (!isHovered) return;
            isHovered = false;

            if (propRenderer != null && propRenderer.materials.Length > 0)
            {
                for (int i = 0; i < propRenderer.materials.Length; i++)
                {
                    if (propRenderer.materials[i].HasProperty("_EmissionColor"))
                    {
                        propRenderer.materials[i].SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }

        /// <summary>
        /// Trigger interaction (called via mouse click, touch, or XR ray interactor)
        /// </summary>
        public void Interact()
        {
            if (interactionParticles != null) interactionParticles.Play();
            if (interactionSound != null) interactionSound.Play();

            onInteracted?.Invoke();

            // Prompt Rem to comment dynamically on this interaction
            var voiceTrigger = FindObjectOfType<RemVoiceTrigger>();
            if (voiceTrigger != null)
            {
                voiceTrigger.ProcessPlayerInput(contextualAIPrompt);
            }
        }
    }
}
