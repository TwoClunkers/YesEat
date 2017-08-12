using System;
using System.Collections.Generic;
using UnityEngine;

public partial class NpcCore
{
    private void AiExplore()
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
                reExploreLocations = GetAllKnownLocations(objectScript.transform.position);
            }
            objectScript.MoveToNewLocation(reExploreLocations[0]);
        }
    }

}
