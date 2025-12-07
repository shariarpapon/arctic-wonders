using System;
using UnityEngine;

namespace Arctic.Gameplay.Interaction
{
    /// <summary>
    /// Any interactable object should inherit from this.
    /// </summary>
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        public virtual string Prompt => "Interact";
        public abstract void Interact(GameObject source);
    }
}