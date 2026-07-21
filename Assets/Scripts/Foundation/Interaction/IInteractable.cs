namespace Arctic.Foundation.Interaction
{
    /// <summary>
    /// Defines interaction functions.
    /// </summary>
    public interface IInteractable
    {
        string HoverPrompt => "Interact";
        bool Interact(InteractionInvoker invoker);
    }
}