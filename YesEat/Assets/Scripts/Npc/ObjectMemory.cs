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
    /// Quantity of this object.
    /// </summary>
    public int Quantity
    {
        get { return count; }
        set { count = value; }
    }
}

