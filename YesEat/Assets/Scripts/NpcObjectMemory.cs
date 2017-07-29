using UnityEngine;

/// <summary>
/// Memory of a single object in the game world.
/// </summary>
public class NpcObjectMemory
{
    #region Private members
    private int subjectID;
    private Vector3 position;
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
    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }
}

