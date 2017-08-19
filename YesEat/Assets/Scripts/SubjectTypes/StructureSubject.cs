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
    /// Copy an existing StructureSubject.
    /// </summary>
    public StructureSubject(StructureSubject copyStructureSubject) : base(copyStructureSubject)
    {
        buildDirections = new BuildRecipe(copyStructureSubject.buildDirections);
    }

    /// <summary>
    /// Copy this StructureSubject.
    /// </summary>
    /// <returns>A new copy of this StructureSubject</returns>
    public override Subject Copy()
    {
        return new StructureSubject(this);
    }

    /// <summary>
    /// The BuildRecipe used to make this Structure
    /// </summary>
    public BuildRecipe BuildDirections
    {
        get { return buildDirections; }
        set { buildDirections = value; }
    }

    public override void TeachNpc(NpcCore npcCharacter)
    {
        npcCharacter.Definition.Memories.Add(new SubjectMemory(subjectID, 0, 0));
    }
}

