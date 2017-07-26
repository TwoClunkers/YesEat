
/// <summary>
/// A Character's attitude about a specific subject.
/// </summary>
public class SubjectAttitude
{
    #region Private members
    private int subjectID = 0;
    private sbyte importance = 0;
    private sbyte goodness = 0;
    #endregion

    /// <summary>
    /// The SubjectID used to reference the Subject in the MasterSubjectList.
    /// </summary>
    public int SubjectID
    {
        get { return subjectID; }
        set { subjectID = value; }
    }

    /// <summary>
    /// The character's perception of the importance or value of the subject. Range: +/-127
    /// </summary>
    public sbyte Importance
    {
        get { return importance; }
        set { importance = value; }
    }

    /// <summary>
    /// The character's perception of whether the subject is good or bad. Range: +/-127 <para />
    /// Negative values are Bad, Positive values are Good.
    /// </summary>
    public sbyte Goodness
    {
        get { return goodness; }
        set { goodness = value; }
    }

    public SubjectAttitude(int SubjectID, sbyte Importance, sbyte Goodness)
    {
        this.subjectID = SubjectID;
        this.importance = Importance;
        this.goodness = Goodness;
    }

    /// <summary>
    /// Add (or subtract) Goodness, clamped at min/max of sbyte type.
    /// </summary>
    /// <param name="value">The amount of Goodness to add (can be negative)</param>
    public void AddGoodness(sbyte value)
    {
        if (goodness + value < sbyte.MinValue)
        {
            goodness = sbyte.MinValue;
        }
        else if (goodness + value > sbyte.MaxValue)
        {
            goodness = sbyte.MaxValue;
        }
        else
        {
            goodness += value;
        }
    }

    /// <summary>
    /// Add (or subtract) Importance, clamped at min/max of sbyte type.
    /// </summary>
    /// <param name="value">The amount of Importance to add (can be negative)</param>
    public void AddImportance(sbyte value)
    {
        if (importance + value < sbyte.MinValue)
        {
            importance = sbyte.MinValue;
        }
        else if (importance + value > sbyte.MaxValue)
        {
            importance = sbyte.MaxValue;
        }
        else
        {
            importance += value;
        }
    }
}
