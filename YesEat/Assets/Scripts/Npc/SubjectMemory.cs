using System;
using System.Linq;

/// <summary>
/// A Character's attitude about a specific subject.
/// </summary>
public class SubjectMemory
{
    #region Private members
    private int subjectID = 0;
    private sbyte safety = 0;
    private sbyte food = 0;
    private int[] sources = null;
    #endregion

    /// <summary>
    /// The SubjectID used to reference the Subject in the KnowledgeBase.
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

    public int[] Sources { get { return sources; } set { sources = value; } }

    public SubjectMemory(SubjectMemory copySubjectMemory)
    {
        subjectID = copySubjectMemory.subjectID;
        safety = copySubjectMemory.safety;
        food = copySubjectMemory.food;
        if (copySubjectMemory.sources != null)
        {
            Sources = new int[copySubjectMemory.sources.Length];
            for (int i = 0; i < copySubjectMemory.Sources.Length; i++)
            {
                Sources[i] = copySubjectMemory.Sources[i];
            }
        }
        else
        {
            sources = new int[0];
        }
    }

    public SubjectMemory(int subjectID, sbyte safety, sbyte food, int[] sources = null)
    {
        this.subjectID = subjectID;
        this.safety = safety;
        this.food = food;
        sources = sources ?? (new int[0]);
    }

    /// <summary>
    /// Add a source for this subject.
    /// </summary>
    /// <param name="SourceSubjectId">The SubjectID to add.</param>
    public void AddSource(int SourceSubjectId)
    {
        if (sources == null)
        {
            sources = new int[1];
            sources[0] = SourceSubjectId;
        }
        else
        {
            if (!sources.Contains(SourceSubjectId))
            {
                int[] newSources = new int[sources.Length + 1];
                for (int i = 0; i < sources.Length; i++)
                {
                    newSources[i] = sources[i];
                }
                newSources[sources.Length] = SourceSubjectId;
                sources = newSources;
            }
        }
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
