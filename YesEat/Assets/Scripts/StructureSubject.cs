using System;
using UnityEngine;

/// <summary>
/// Structure Subject extends the Subject Class to describe structures for AI
/// </summary>

public class StructureSubject : Subject
{
    #region Private members
    private BuildRecipe buildDirections;
    #endregion

    public StructureSubject() : base()
    {
        name = "Structure";
        description = "A Structure that is Built";
        icon = new Sprite();

        buildDirections = new BuildRecipe();
    }

    /// <summary>
    /// The BuildRecipe used to make this Structure
    /// </summary>
    public BuildRecipe BuildDirections
    {
        get { return buildDirections; }
        set { buildDirections = value; }
    }


}

