namespace Arctic.Gameplay.Interaction
{
    /// <summary>
    /// Defines interaction functions.
    /// </summary>
    public interface IInteractable
    {
        string Prompt => "Interact";
        bool Interact(InteractionInvoker invoker);
    }
}