﻿

public partial class NpcCore
{
    private void AiCoreSubprocessSafety()
    {
        // If safety is the top priority, combat is an unsafe action.
        // Stop combat, run for your life.
        status.UnsetState(NpcStates.Fighting);
        // if we have a nest assume it is safe and go there first
        if (HaveNest())
        {
            // if not at nest move there first
            if (objectScript.Location.SubjectID != definition.Nest.LocationSubjectID)
            {
                objectScript.MoveToNewLocation(db.GetSubject(definition.Nest.SubjectID) as LocationSubject);
                return;
            }
        }

        // don't have nest or we're at nest and it isn't safe. move to safe location
        objectScript.MoveToNewLocation(FindSafeLocation(objectScript));

    }
}