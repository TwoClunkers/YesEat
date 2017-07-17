
public partial class NpcCharacter {
    /// <summary>
    /// All possible transient character states.
    /// </summary>
    [System.Flags]
    public enum TransientStates
    {
        Idle,
        Moving,
        Eating,
        Fighting,
        Dead
    }

    public class CharacterStatus
    {
        private TransientStates _status;

        /// <summary>
        /// Check if the character is currently able to eat. Cannot eat while experiencing disabling statuses like Fear, Stun, Unconscious.
        /// </summary>
        /// <returns>Yes|No</returns>
        public bool CanEat()
        {
            return (_status & TransientStates.Dead) == 0;
        }
        /// <summary>
        /// Turns on the changeState flag.
        /// </summary>
        /// <param name="changeState">The flag to turn on.</param>
        public void SetState(TransientStates changeState)
        {
            _status &= changeState;
        }
        /// <summary>
        /// Turns off the changeState flag.
        /// </summary>
        /// <param name="changeState">The flag to be turned off.</param>
        public void UnsetState(TransientStates changeState)
        {
            _status &= ~changeState;
        }

        /// <summary>
        /// Check if the checkState state is set.
        /// </summary>
        /// <param name="checkState">The state to check for.</param>
        /// <returns>True:set, False: not set.</returns>
        public bool IsStateSet(TransientStates checkState)
        {
            return (_status & checkState) != 0;
        }
    }
}
