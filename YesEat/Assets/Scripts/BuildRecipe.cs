using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The Build Recipe lists the required items needed for Item or Structure as well as required tools or knowledge
/// </summary>
public class BuildRecipe
{
    #region Private members
    private List<InventoryItem> _ingredients;
    #endregion

    public List<InventoryItem> Ingredients { get { return _ingredients; } set { _ingredients = value; } }

    public BuildRecipe()
    {
        _ingredients = new List<InventoryItem>();
    }

    public BuildRecipe(List<InventoryItem> ingredients)
    {
        _ingredients = ingredients;
    }

    /// <summary>
    /// Copy an existing BuildRecipe.
    /// </summary>
    /// <param name="copyBuildRecipe">The recipe to copy.</param>
    public BuildRecipe(BuildRecipe copyBuildRecipe)
    {
        if (copyBuildRecipe != null)
        {
            _ingredients = new List<InventoryItem>(copyBuildRecipe._ingredients);
            for (int i = 0; i < copyBuildRecipe._ingredients.Count; i++)
            {
                _ingredients[i] = copyBuildRecipe._ingredients[i];
            }
        }
    }

}
