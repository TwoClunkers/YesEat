using System.Collections.Generic;

/// <summary>
/// All possible character drivers.
/// </summary>
public enum NpcDrivers
{
    Safety,
    Hunger,
    Nest,
    Explore
}

public class NpcDriversList : List<NpcDrivers>
{
    public void SetTopDriver(NpcDrivers npcDriver)
    {
        if (this.Contains(npcDriver))
            this.Remove(npcDriver);

        this.Insert(0, npcDriver);
    }
}

