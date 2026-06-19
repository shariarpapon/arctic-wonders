using System;
using UnityEngine;

namespace Arctic.Gameplay.Interaction
{
    /// <summary>
    /// Any interactable object should inherit from this.
    /// </summary>
    public abstract class InteractableBehavior : MonoBehaviour, IInteractable
    {
        public virtual string HoverPrompt => "Interact";
        public virtual bool Interact(InteractionInvoker source) 
        {
            Debug.Log("Player interacting with " + name);
            return true;
        }
    }
}