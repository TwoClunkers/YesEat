
using System;
using UnityEngine;

/// <summary>
/// Base Class for all subject types. 
/// </summary>

public class Subject
{
    #region Private members
    protected int subjectID;
	protected string name;
	protected string description;
	protected Sprite icon;
	protected int[] relatedSubjects;
	#endregion

	public Subject ()
	{
        subjectID = 0;
		name = new String("Name");
		description = new String("This is the description");
		icon = new Sprite();
		relatedSubjects = new int[0];
	}

	//for use by derived classes
	public Subject (String newName, String newDescription, Sprite newSprite, int[] newRelated)
	{
		name = newName;
		description = newDescription;
		icon = newSprite;
		relatedSubjects = newRelated;
	}

    /// <summary>
    /// The subject ID is used to find the right subject
    /// </summary>
    public int SubjectID
    {
        get { return subjectID; }
        set { subjectID = value; }
    }


    /// <summary>
    /// This subject's name.
    /// </summary>
	public string Name
	{
		get { return name; }
		set { name = value; }
	}

    /// <summary>
    /// This subject's description.
    /// </summary>
	public string Description
	{
		get { return description; }
		set { description = value ?? String.Empty; }
	}

	/// <summary>
    /// GUI Icon for this subject.
    /// </summary>
	public Sprite Icon
	{
		get { return icon; }
		set { icon = value; }
	}

    /// <summary>
    /// Subjects related to this one.
    /// </summary>
    public int[] RelatedSubjects 
	{
		get { return relatedSubjects; }
		set { relatedSubjects = value; }
	}



}


