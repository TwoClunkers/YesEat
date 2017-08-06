using System;
using System.Collections.Generic;

public partial class NpcCore
{
    private void AiCoreSubprocessExplore()
    {
        // explore unknown locations
        if (unexploredLocations.Count > 0)
        {
            objectScript.MoveToNewLocation(unexploredLocations[0]);
        }
        else
        {
            // explore known locations for new stuff
            // populate locations to re-explore
            if (reExploreLocations.Count == 0)
            {
                reExploreLocations = GetAllKnownLocations();
            }
        }
    }
}