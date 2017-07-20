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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newSubjectID"></param>
    /// <param name="newStackSize"></param>
    /// <returns></returns>
    public bool SetStack (int newSubjectID, int newStackSize)
    {
        ItemSubject newSubject = masterSubjectList.GetSubject(newSubjectID, ItemSubject) as ItemSubject;

        if (newSubject != null)
        {
            subjectID = newSubjectID;
            stackSize = Math.Min(newStackSize, newSubject.MaxStack);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// AddStack will return the number remaining that would not fit based on ItemSubject.maxStack
    /// </summary>
    /// <param name="newSubjectID"></param>
    /// <param name="newStackSize"></param>
    /// <returns></returns>
    public int AddStack(int newSubjectID, int newStackSize)
    {
        if (newSubjectID != subjectID) return newStackSize; //reject entire amount

        ItemSubject newSubject = masterSubjectList.GetSubject(newSubjectID, ItemSubject) as ItemSubject;

        if (newSubject != null)
        {
            int tempStack = stackSize + newStackSize;
            stackSize = Math.Min(tempStack, newSubject.MaxStack);
            newStackSize = Math.Max(0, tempStack - newSubject.MaxStack);
        }
        return newStackSize;

    }

    /// <summary>
    /// Override for AddStack takes an Inventory Item to add to this Inventory Item and returns amount that would not fit
    /// </summary>
    /// <param name="addedInvItem"></param>
    /// <returns></returns>
    public int AddStack(InventoryItem addedInvItem)
    {
        if (addedInvItem.subjectID != subjectID) return addedInvItem.stackSize; //reject entire amount

        ItemSubject newSubject = masterSubjectList.GetSubject(addedInvItem.subjectID, ItemSubject) as ItemSubject;

        if (newSubject != null)
        {
            int tempStack = stackSize + addedInvItem.stackSize;
            stackSize = Math.Min(tempStack, newSubject.MaxStack);
            addedInvItem.stackSize = Math.Max(0, tempStack - newSubject.MaxStack);
        }
        return addedInvItem.stackSize;

    }

}


