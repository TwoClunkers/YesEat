using System;
using UnityEngine;

/// <summary>
/// Item contained in an Inventory container.
/// </summary>
public class InventoryItem
{
    #region Private members
    private int subjectID;
    private int stackSize;
    private MasterSubjectList masterSubjectList;
    #endregion

    /// <summary>
    /// New InventoryItem.
    /// </summary>
    /// <param name="masterSubjectListRef">reference to the MasterSubjectList</param>
    public InventoryItem(ref MasterSubjectList masterSubjectListRef)
    {
        subjectID = -1;
        stackSize = 1;
        masterSubjectList = masterSubjectListRef;
    }

    /// <summary>
    /// The Subject ID in this slot. Used to get info from MasterSubjectList.
    /// </summary>
    public int SubjectID
    {
        get { return subjectID; }
        set { subjectID = value; }
    }

    /// <summary>
    /// This describes how large the stack is.
    /// </summary>
    public int StackSize
    {
        get { return stackSize; }
        set { stackSize = value; }
    }

    public int CombineItems(int id, int number, MasterSubjectList subList)
    {
        if (id == subjectID) stackSize += number;
        else return number; //not the same Item

        ItemSubject tempSubject = subList.GetSubject(subjectID) as ItemSubject;
        int max = tempSubject.MaxStack;

        if (stackSize > max) return stackSize - max;
        else return 0;

    }

    public Subject GetSubject()
    {
        return masterSubjectList.GetSubject(subjectID);
    }

}


