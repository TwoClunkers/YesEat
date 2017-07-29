using UnityEngine;

/// <summary>
/// Structure Subject extends the Subject Class to describe structures for AI
/// </summary>

public class StructureSubject : Subject
{
    #region Private members
    private BuildRecipe _buildRecipe;
    #endregion

    public StructureSubject() : base()
    {
        name = "Structure";
        description = "A Structure that is Built";
        icon = new Sprite();

        _buildRecipe = new BuildRecipe();
    }

    /// <summary>
    /// The BuildRecipe used to make this Structure
    /// </summary>
    public BuildRecipe BuildRecipe
    {
        get { return _buildRecipe; }
        set { _buildRecipe = value; }
    }

}

