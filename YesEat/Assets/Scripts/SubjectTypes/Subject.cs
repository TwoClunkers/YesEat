
using System;
using UnityEngine;

/// <summary>
/// Base Class for all subject types. 
/// </summary>

public abstract class Subject
{
    #region Private members
    private SubjectAttributes attributes;
    protected string description;
    protected Sprite icon;
    protected string name;
    protected GameObject prefab;
    protected int[] relatedSubjects;
    protected int subjectID;
    #endregion

    public Subject()
    {
        Attributes = new SubjectAttributes();
        description = "This is the subject description.";
        icon = null;
        name = "Subject Name";
        prefab = null;
        relatedSubjects = new int[0];
        subjectID = 0;
    }

    public Subject(Subject copySubject)
    {
        Attributes = copySubject.Attributes;
        description = copySubject.description;
        icon = copySubject.icon;
        name = copySubject.name;
        prefab = copySubject.prefab;
        relatedSubjects = copySubject.relatedSubjects;
        subjectID = copySubject.subjectID;
    }

    //for use by derived classes
    public Subject(String newName, String newDescription, Sprite newSprite, int[] newRelated)
    {
        Attributes = new SubjectAttributes();
        description = newDescription;
        icon = newSprite;
        name = newName;
        relatedSubjects = newRelated;
    }

    /// <summary>
    /// Copy this subject.
    /// </summary>
    public abstract Subject Copy();

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

    public abstract void TeachNpc(NpcCore npcCharacter);

    /// <summary>
    /// Prefab used by this subject
    /// </summary>
    public GameObject Prefab
    {
        get { return prefab; }
        set { prefab = value; }
    }

    public SubjectAttributes Attributes { get { return attributes; } set { attributes = value; } }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Subject testObj = obj as Subject;
            return (testObj.subjectID == subjectID);
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}


