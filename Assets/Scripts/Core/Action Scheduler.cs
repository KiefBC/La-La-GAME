using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages and coordinates different character actions to ensure only one action
    /// is active at a time. Handles the cancellation of current actions when new ones begin.
    /// Used by components like Fighter and Mover to coordinate their behaviors.
    /// </summary>
    public class ActionScheduler : MonoBehaviour
    {
        private IAction _currentAction;

        /// <summary>
        /// Starts a new action while properly canceling any current action.
        /// If the new action is the same as the current one, no change occurs.
        /// </summary>
        /// <param name="action">The new action to start. Can be null to just cancel the current action.</param>
        public void StartAction(IAction action)
        {
            if (action == _currentAction) return;
            if (_currentAction != null)
            {
                _currentAction.Cancel();
            }
            _currentAction = action;
        }
        
        /// <summary>
        /// Cancels the currently running action by calling StartAction with null.
        /// Useful for stopping all current actions without starting a new one.
        /// </summary>
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}