using System;
using UnityEngine;

/// <summary>
/// The Item Subject extends Subject Class and should describe and encapsulate everything that can be placed in inventory
/// </summary>

public class ItemSubject : Subject
{
    #region Private members
    private BuildRecipe _recipe;
    int maxStack;
    #endregion

    public ItemSubject() : base()
    {
        name = "Item";
        description = "An Item is anything that can fit in your inventory";
        icon = new Sprite();

        //qualities = new int[2];
        _recipe = null;
        maxStack = 10;
    }

    /// <summary>
    /// Copy an existing ItemSubject.
    /// </summary>
    public ItemSubject(ItemSubject copyItemSubject) : base(copyItemSubject)
    {
        _recipe = new BuildRecipe(copyItemSubject._recipe);
        maxStack = copyItemSubject.maxStack;
    }

    /// <summary>
    /// Copy an existing ItemSubject.
    /// </summary>
    public override Subject Copy()
    {
        return new ItemSubject(this);
    }

    /// <summary>
    /// The BuildRecipe used to make this Item
    /// </summary>
    public BuildRecipe Recipe
    {
        get { return _recipe; }
        set { _recipe = value; }
    }

    /// <summary>
    /// MaxStack is the number of items in one inventory slot (for this kind of item)
    /// </summary>
    public int MaxStack
    {
        get { return maxStack; }
        set { maxStack = value; }
    }

    /// <summary>
    /// IsStackable tests if an item can stack using maxStack
    /// </summary>
    public bool IsStackable
    {
        get {
            if (maxStack > 1)
                return true;
            else
                return false;
        }
    }

    public override void TeachNpc(NpcCore npcCharacter)
    {
        npcCharacter.Definition.Memories.Add(new SubjectMemory(subjectID, 0, 0));
    }
}
