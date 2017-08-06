using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Memory of a single object in the game world.
/// </summary>
public class ObjectMemory
{
    #region Private members
    private int subjectID;
    private int count;
    #endregion

    /// <summary>
    /// The object's subject.
    /// </summary>
    public int SubjectID
    {
        get { return subjectID; }
        set { subjectID = value; }
    }

    /// <summary>
    /// Position of the object in the game world.
    /// </summary>
    public int Count
    {
        get { return count; }
        set { count = value; }
    }
}

