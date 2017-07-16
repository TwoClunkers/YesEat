
public partial class NpcCharacter {
    /// <summary>
    /// All possible transient character states.
    /// </summary>
    [System.Flags]
    public enum TransientStates
    {
        Idle,
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
            return (_status & TransientStates.Fear & TransientStates.Stun & TransientStates.Unconscious) == 0;
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
    }
}
