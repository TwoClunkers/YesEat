using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Character's attitude about a specific subject.
/// </summary>
public class MasterSubjectList : MonoBehaviour
{
    #region Private members
    private List<Subject> masterSubjectList;

    #endregion

    public MasterSubjectList()
    {
        //TODO: load all subjects into the list.
        masterSubjectList = new List<Subject>();
    }

    public Subject GetSubject(int subjectID)
    {
        if (masterSubjectList.Exists(o => o.SubjectID == subjectID))
            return masterSubjectList.Find(o => o.SubjectID == subjectID);
        else
            return null;
    }
}
