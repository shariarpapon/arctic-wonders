using Arctic.Gameplay.Interaction;
using Arctic.Player.Interaction;
using TMPro;
using UnityEngine;

namespace Arctic.UI.Interaction
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private PlayerInteractionInvoker interactionHandler;

        private IInteractable focusedInteractable;

        private void Awake()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            interactionHandler.scanner.OnInteractableTargetFound += OnFound;
            interactionHandler.scanner.OnTargetLost += OnLost;
        }

        private void Update()
        {
            if (focusedInteractable != null)
                promptText.text = focusedInteractable.Prompt;
        }

        private void OnFound(IInteractable target)
        {
            if (target == null || string.IsNullOrEmpty(target.Prompt))
                return;

            focusedInteractable = target;
            canvasGroup.alpha = 1;
        }

        private void OnLost(GameObject prevTarget) 
        {
            focusedInteractable = null;
            canvasGroup.alpha = 0;
        }
    }
}   