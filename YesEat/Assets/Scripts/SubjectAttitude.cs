
/// <summary>
/// A Character's attitude about a specific subject.
/// </summary>
public class SubjectAttitude
{
    #region Private members
    private int subjectID = 0;
    private int importance = 0;
    private int goodness = 0;
    #endregion

    public int SubjectID
    {
        get { return subjectID; }
        set { subjectID = value; }
    }

    /// <summary>
    /// The character's perception of the importance or value of the subject.
    /// </summary>
    public int Importance
    {
        get { return importance; }
        set { importance = value; }
    }

    /// <summary>
    /// The character's perception of whether the subject is good or bad.
    /// </summary>
    public int Goodness
    {
        get { return goodness; }
        set { goodness = value; }
    }

}
