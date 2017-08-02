
/// <summary>
/// All traits of this character.
/// </summary>
[System.Flags]
public enum NpcTraits
{
    NestMaker = (1 << 1),
    Herbivore = (1 << 2),
    Carnivore = (1 << 3)
}

public class NpcCharacterTraits
{
    private NpcTraits _traits;

    public NpcCharacterTraits() { }

    public NpcCharacterTraits(NpcTraits Traits) { _traits = Traits; }

    /// <summary>
    /// Is this NPC a nest maker?
    /// </summary>
    public bool IsNestMaker { get { return HasTrait(NpcTraits.NestMaker); } }

    /// <summary>
    /// Turns on the changeTrait flag.
    /// </summary>
    /// <param name="changeTrait">The flag to turn on.</param>
    public void SetTrait(NpcTraits changeTrait) { _traits &= changeTrait; }

    /// <summary>
    /// Turns off the changeTrait flag.
    /// </summary>
    /// <param name="changeTrait">The flag to be turned off.</param>
    public void UnsetTrait(NpcTraits changeTrait) { _traits &= ~changeTrait; }

    /// <summary>
    /// Check if checkTrait is set.
    /// </summary>
    /// <param name="checkTrait">The trait to check for.</param>
    /// <returns>True:set, False: not set.</returns>
    public bool HasTrait(NpcTraits checkTrait) { return (_traits & checkTrait) != 0; }
}