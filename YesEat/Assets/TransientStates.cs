using UnityEngine;

public partial class NpcCharacter : MonoBehaviour {
    /// <summary>
    /// All possible transient character states.
    /// </summary>
    [System.Flags]
    public enum TransientStates
    {
        Speaking,
        Listening,
        Eating,
        Drinking,
        Entertaining,
        Meditating,
        Fighting,
        Stunned,
        Blind,
        Prone,
        Fear,
        Root,
        Unconscious
    }
}
