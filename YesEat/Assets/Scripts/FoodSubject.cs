
using System;
using UnityEngine;

/// <summary>
/// Food Subject extends the Item Class of the Subject Class to describe Food for AI
/// </summary>

public class FoodSubject : ItemSubject
{
    #region Private members
    private int foodType;
    private int foodValue;
    #endregion

    public FoodSubject() : base()
    {
        name = "Food";
        description = "Something Eaten";
        icon = new Sprite();

        foodType = 0;
        foodValue = 1;
    }

    /// <summary>
    /// What kind of food is this?
    /// </summary>
    public int FoodType
    {
        get { return foodType; }
        set { foodType = value; }
    }

    /// <summary>
    /// The how filling is this item
    /// </summary>
    public int FoodValue
    {
        get { return foodValue; }
        set { foodValue = value; }
    }

}


