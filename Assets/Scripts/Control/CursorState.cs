namespace Control
{
    /// <summary>
    /// Defines the possible states for the game cursor, determining its appearance and behavior.
    /// </summary>
    public enum CursorState
    {
        /// <summary>
        /// Combat state indicates the cursor is over a valid combat target.
        /// </summary>
        Combat,

        /// <summary>
        /// Movement state indicates the cursor is over a valid movement destination.
        /// </summary>
        Movement,

        /// <summary>
        /// None state represents the default cursor state with no special interaction available.
        /// </summary>
        None,

        /// <summary>
        /// UI state indicates the cursor is interacting with user interface elements.
        /// </summary>
        UI,

        /// <summary>
        /// Pickup state indicates the cursor is over an item that can be collected.
        /// </summary>
        Pickup
    }
}