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
        public virtual bool Interact(InteractionInvoker source) 
        {
            Debug.Log("Interacting with " + name);
            return true;
        }
    }
}