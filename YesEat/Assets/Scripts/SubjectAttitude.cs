
using System;
/// <summary>
/// A Character's attitude about a specific subject.
/// </summary>
public class SubjectAttitude
{
    #region Private members
    private int subjectID = 0;
    private sbyte safety = 0;
    private sbyte food = 0;
    #endregion

    /// <summary>
    /// The SubjectID used to reference the Subject in the MasterSubjectList.
    /// </summary>
    public int SubjectID
    {
        get { return subjectID; }
        set { subjectID = value; }
    }

    public sbyte Safety
    {
        get { return safety; }
        set { safety = value; }
    }

    public sbyte Food
    {
        get { return food; }
        set { food = value; }
    }

    public SubjectAttitude(int SubjectID, sbyte SafetyValue, sbyte FoodValue)
    {
        this.subjectID = SubjectID;
        this.safety = SafetyValue;
        this.food = FoodValue;
    }

    /// <summary>
    /// Add (or subtract) Food, clamped at min/max of sbyte type.
    /// </summary>
    /// <param name="value">The amount of Food to add (can be negative)</param>
    public void AddFood(sbyte value)
    {
        if (food + value < sbyte.MinValue)
        {
            food = sbyte.MinValue;
        }
        else if (food + value > sbyte.MaxValue)
        {
            food = sbyte.MaxValue;
        }
        else
        {
            food += value;
        }
    }

    /// <summary>
    /// Add (or subtract) Food, clamped at min/max of sbyte type.
    /// </summary>
    /// <param name="value">The amount of Food to add (can be negative)</param>
    public void AddSafety(sbyte value)
    {
        if (safety + value < sbyte.MinValue)
        {
            safety = sbyte.MinValue;
        }
        else if (safety + value > sbyte.MaxValue)
        {
            safety = sbyte.MaxValue;
        }
        else
        {
            safety += value;
        }
    }

    internal void AddValues(sbyte FoodValue, sbyte SafetyValue)
    {
        AddFood(FoodValue);
        AddSafety(SafetyValue);
    }

    internal void SetValues(sbyte FoodValue, sbyte SafetyValue)
    {
        Food = FoodValue;
        Safety = SafetyValue;
    }
}
