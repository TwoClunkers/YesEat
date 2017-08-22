using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Memory of a special object in the game world.
/// </summary>
public class SpecialObjectMemory : ObjectMemory
{
    #region Private members
    private Vector3 position;
    #endregion

    public SpecialObjectMemory() : base()
    {
        Position = default(Vector3);
    }

    public SpecialObjectMemory(SpecialObjectMemory copyObjectMemory) : base(copyObjectMemory)
    {
        Position = copyObjectMemory.Position;
    }

    /// <summary>
    /// The coordinates of this object.
    /// </summary>
    public Vector3 Position { get { return position; } set { position = value; } }
}

