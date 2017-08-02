using System;
using System.Collections.Generic;

/// <summary>
/// A memory of single location and the objects that were observed there.
/// </summary>
public class NpcLocationMemory
{
    #region Private members
    private List<NpcObjectMemory> objectMemories;
    private int locationSubjectID;
    private DateTime lastTimeSeen;
    private MasterSubjectList masterSubjectList;
    #endregion

    public NpcLocationMemory(ref MasterSubjectList MasterSubjectListRef)
    {
        masterSubjectList = MasterSubjectListRef;
    }

    public LocationSubject LocationSubject { get { return masterSubjectList.GetSubject(locationSubjectID) as LocationSubject; } }

    /// <summary>
    /// MasterSubjectList.SubjectID for this location.
    /// </summary>
    public int LocationSubjectID
    {
        get { return locationSubjectID; }
        set { locationSubjectID = value; }
    }

    /// <summary>
    /// The time when this location was last seen.
    /// </summary>
    public DateTime LastTimeSeen
    {
        get { return lastTimeSeen; }
        set { lastTimeSeen = value; }
    }

    /// <summary>
    /// All remembered objects in this location.
    /// </summary>
    public List<NpcObjectMemory> ObjectMemories
    {
        get { return objectMemories; }
        set { objectMemories = value; }
    }
}
