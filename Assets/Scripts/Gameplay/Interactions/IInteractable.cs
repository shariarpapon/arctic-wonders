namespace Arctic.Gameplay.Interaction
{
    /// <summary>
    /// Defines interaction functions.
    /// </summary>
    public interface IInteractable
    {
        string Prompt => "Interact";
        void Interact(InteractionInvoker invoker);
        virtual void Interact(InteractionInvoker invoker, System.Action<bool> callback) { }
    }
}