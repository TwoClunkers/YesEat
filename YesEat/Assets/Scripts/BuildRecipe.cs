using System;
using UnityEngine;

/// <summary>
/// An ingredient for a BuildRecipe.
/// </summary>
public class Ingredient
{
    private int subjectId;
    private int quantity;

    public int SubjectId { get { return subjectId; } set { subjectId = value; } }
    public int Quantity { get { return quantity; } set { quantity = value; } }

    public Ingredient(int subjectId, int quantity)
    {
        this.subjectId = subjectId;
        this.quantity = quantity;
    }

}

/// <summary>
/// The Build Recipe lists the required items needed for Item or Structure as well as required tools or knowledge
/// </summary>
public class BuildRecipe
{
    #region Private members
    private Ingredient[] _ingredients;
    #endregion

    public BuildRecipe()
    {
        _ingredients = new Ingredient[0];
    }

    public BuildRecipe(Ingredient[] ingredients)
    {
        _ingredients = ingredients;
    }

    /// <summary>
    /// Copy an existing BuildRecipe.
    /// </summary>
    /// <param name="copyBuildRecipe">The recipe to copy.</param>
    public BuildRecipe(BuildRecipe copyBuildRecipe)
    {
        // assign private fields
        _ingredients = copyBuildRecipe._ingredients;
    }

}
