using System.Collections.Generic;

public partial class NpcCharacter
{

    /// <summary>
    /// All possible character drivers.
    /// </summary>
    public enum NpcDrivers
    {
        Health,
        Safety,
        Hunger,
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
}
