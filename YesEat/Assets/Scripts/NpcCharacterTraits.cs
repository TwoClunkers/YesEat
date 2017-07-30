
/// <summary>
/// All traits of this character.
/// </summary>
[System.Flags]
public enum NpcTraits
{
    NestMaker,
    Herbivore,
    Carnivore
}

public class NpcCharacterTraits
{
    private NpcTraits _traits;

    /// <summary>
    /// Is this NPC a nest maker?
    /// </summary>
    public bool IsNestMaker { get { return HasTrait(NpcTraits.NestMaker); } }

    /// <summary>
    /// Turns on the changeTrait flag.
    /// </summary>
    /// <param name="changeTrait">The flag to turn on.</param>
    public void SetState(NpcTraits changeTrait) { _traits &= changeTrait; }

    /// <summary>
    /// Turns off the changeTrait flag.
    /// </summary>
    /// <param name="changeTrait">The flag to be turned off.</param>
    public void UnsetState(NpcTraits changeTrait) { _traits &= ~changeTrait; }

    /// <summary>
    /// Check if the checkState state is set.
    /// </summary>
    /// <param name="checkTrait">The state to check for.</param>
    /// <returns>True:set, False: not set.</returns>
    public bool HasTrait(NpcTraits checkTrait) { return (_traits & checkTrait) != 0; }
}