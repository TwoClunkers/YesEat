﻿
public partial class NpcCore
{
    /// <summary>
    /// All possible transient character states.
    /// </summary>
    [System.Flags]
    public enum NpcStates
    {
        Idle = (1 << 1),
        Moving = (1 << 2),
        Eating = (1 << 3),
        Starving = (1 << 4),
        Fighting = (1 << 5),
        Dead = (1 << 6)
    }

    public class NpcStatus
    {
        private NpcStates _status;

        /// <summary>
        /// Check if the character is currently able to eat. Cannot eat while experiencing disabling statuses like Fear, Stun, Unconscious.
        /// </summary>
        /// <returns>Yes|No</returns>
        public bool CanEat()
        {
            return (_status & NpcStates.Dead) == 0;
        }
        /// <summary>
        /// Turns on the changeState flag.
        /// </summary>
        /// <param name="changeState">The flag to turn on.</param>
        public void SetState(NpcStates changeState)
        {
            _status |= changeState;
        }
        /// <summary>
        /// Turns off the changeState flag.
        /// </summary>
        /// <param name="changeState">The flag to be turned off.</param>
        public void UnsetState(NpcStates changeState)
        {
            _status &= ~changeState;
        }

        /// <summary>
        /// Check if the checkState state is set.
        /// </summary>
        /// <param name="checkState">The state to check for.</param>
        /// <returns>True:set, False: not set.</returns>
        public bool IsStateSet(NpcStates checkState)
        {
            return (_status & checkState) != 0;
        }
    }
}
