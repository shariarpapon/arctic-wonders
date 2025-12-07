using UnityEngine;

namespace Arctic.Gameplay.Interaction
{
    /// <summary>
    /// Defines interaction functions.
    /// </summary>
    public interface IInteractable
    {
        public virtual string Prompt => "Interact";
        public abstract void Interact(GameObject source);
    }
}