﻿using UnityEngine;
using System.Collections;

public abstract class SubjectObjectScript : MonoBehaviour
{
    #region Private members
    protected Subject subject;
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
    public Subject Subject
    {
        get { return subject; }
        set { subject = value; }
    }

    public LocationSubject Location
    {
        get { return location; }
        set { location = value; }
    }

    /// <summary>
    /// Will be used by derived classes
    /// </summary>
    /// <param name="newSubject"></param>
    public abstract void InitializeFromSubject(Subject newSubject);

    public abstract InventoryItem Harvest(int itemIdToHarvest);
}
