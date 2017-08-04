using System;

public partial class NpcCharacter
{
    private void AiCoreSubprocessExplore()
    {
        // TODO: explore for unknown locations
        // TODO: explore known locations for new stuff
        if (unexploredLocations.Count > 0)
            objectScript.MoveToNewLocation(unexploredLocations[0]);
        else
            UnityEngine.Debug.Log("No unexplored location to go to.");
    }
}