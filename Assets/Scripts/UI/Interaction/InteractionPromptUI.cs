using Arctic.Gameplay.Interaction;
using UnityEngine;
using TMPro;

namespace Arctic.UI.Interaction
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private InteractionInvoker interactionInvoker;
        private IInteractable focusedInteractable;

        private void Awake()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            interactionInvoker.InteractableTracker.OnNewTargetFound += OnFound;
            interactionInvoker.InteractableTracker.OnTargetLost += OnLost;
        }

        private void Update()
        {
            if (focusedInteractable != null)
                promptText.text = focusedInteractable.Prompt;
        }

        private void OnFound(GameObject newTarget)
        {
            newTarget.TryGetComponent(out IInteractable target);
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