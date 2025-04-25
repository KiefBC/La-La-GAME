namespace Control
{
    /// <summary>
    /// Interface for objects that can be interacted with through raycasting.
    /// Implements cursor state management and raycast handling for interactive game objects.
    /// </summary>
    public interface IRayCastable
    {
        /// <summary>
        /// Gets the cursor state that should be displayed when hovering over this object.
        /// </summary>
        /// <returns>The appropriate cursor state for this interaction</returns>
        CursorState GetCursorState();

        /// <summary>
        /// Handles the raycast interaction with this object.
        /// </summary>
        /// <param name="callingController">The PlayerController that initiated the raycast</param>
        /// <returns>True if the raycast was handled, false otherwise</returns>
        bool HandleRaycast(PlayerController callingController);
    }
}
