using System;
using UnityEngine;

namespace Arctic.Gameplay.Interaction
{
    /// <summary>
    /// Any interactable object should inherit from this.
    /// </summary>
    public abstract class InteractableBehavior : MonoBehaviour, IInteractable
    {
        public virtual string Prompt => "Interact";
        public virtual void Interact(InteractionInvoker source) { }
    }
}