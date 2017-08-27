using System;
using System.Collections.Generic;

/// <summary>
/// A memory of single location and the objects that were observed there.
/// </summary>
public class LocationMemory : SubjectMemory
{
    #region Private members
    private List<ObjectMemory> objectMemories;
    private int locationSubjectID;
    private DateTime lastTimeSeen;

    public LocationMemory(LocationMemory copyLocationMemory) : base(copyLocationMemory)
    {
        objectMemories = new List<ObjectMemory>(copyLocationMemory.objectMemories);
        locationSubjectID = copyLocationMemory.locationSubjectID;
        lastTimeSeen = copyLocationMemory.LastTimeSeen;
    }
    #endregion

    public LocationMemory(int SubjectID, sbyte SafetyValue, sbyte FoodValue) : base(SubjectID, SafetyValue, FoodValue)
    {
        objectMemories = new List<ObjectMemory>();
        locationSubjectID = 0;
        lastTimeSeen = default(DateTime);
    }

    /// <summary>
    /// KnowledgeBase.SubjectID for this location.
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
    public List<ObjectMemory> ObjectMemories
    {
        get { return objectMemories; }
        set { objectMemories = value; }
    }
}
