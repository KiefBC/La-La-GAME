namespace Core
{
    /// <summary>
    /// Defines a contract for game actions that can be cancelled.
    /// Used by the ActionScheduler to manage and coordinate different character actions
    /// such as movement, combat, or other interruptible behaviors.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Cancels the current action, cleaning up any ongoing processes
        /// or animations associated with this action.
        /// </summary>
        void Cancel();
    }
}