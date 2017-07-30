using UnityEngine;
using System.Collections;

public class SubjectObjectScript : MonoBehaviour
{
    #region Private members
    protected Subject thisSubject;
    protected LocationSubject location;
    #endregion

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ThisSubject is the subjectcard used to make this object
    /// </summary>
    public Subject ThisSubject
    {
        get { return thisSubject; }
        set { thisSubject = value; }
    }

    public LocationSubject Location
    {
        get { return location; }
        set { location = value; }
    }

    public virtual void InitializeFromSubject(Subject newSubject, Vector3 position)
    {

    }

}
