using System;
using System.Collections.Generic;

/// <summary>
/// A Character's attitude about a specific subject.
/// </summary>
public class MasterSubjectList
{
    #region Private members
    private List<Subject> masterSubjectList;
    private int maxID;
    #endregion

    public MasterSubjectList()
    {
        //TODO: load all subjects into the list.
        masterSubjectList = new List<Subject>();
    }

    /// <summary>
    /// Get Subject using subjectID.
    /// </summary>
    /// <param name="subjectID">subjectID of Subject to find.</param>
    /// <param name="subjectType">Required type of subject.</param>
    /// <returns>Returns null if subject is not found or the type of the returned subject does not match subjectType.</returns>
    public Subject GetSubject(int subjectID, Type subjectType = null)
    {
        if (masterSubjectList.Exists(o => o.SubjectID == subjectID))
        {
            Subject tempSubject = masterSubjectList.Find(o => o.SubjectID == subjectID);
            if (subjectType != null)
            {
                if (tempSubject.GetType() == subjectType)
                    return tempSubject;
                else
                    return null;
            }
            else
            {
                return tempSubject;
            }
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// GetNextID bumps up the ID count to assign to new subjects
    /// </summary>
    /// <returns></returns>
    public int GetNextID()
    {
        maxID += 1;

        return maxID;
    }

    public bool AddSubject(Subject newSubject)
    {
        //if we don't have a valid subject or if the ID is in use return false
        if (newSubject == null) return false;
        if (masterSubjectList.Exists(o => o.SubjectID == newSubject.SubjectID)) return false;

        //otherwise we should be able to add
        masterSubjectList.Add(newSubject);
        return true;
    }
}
