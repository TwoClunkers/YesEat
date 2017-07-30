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
        masterSubjectList = masterSubjectListRef;
        subjectID = -1;
        stackSize = 0;
    }


    /// <summary>
    /// New InventoryItem constructor
    /// </summary>
    /// <param name="masterSubjectListRef"></param>
    /// <param name="_subjectID"></param>
    /// <param name="_stackSize"></param>
    public InventoryItem(ref MasterSubjectList masterSubjectListRef, int _subjectID, int _stackSize)
    {
        masterSubjectList = masterSubjectListRef;
        subjectID = _subjectID;
        stackSize = _stackSize;
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
    /// SetStack will set the value of the Inventory Item and only return false if we set an invalid subject
    /// </summary>
    /// <param name="newSubjectID"></param>
    /// <param name="newStackSize"></param>
    /// <returns></returns>
    public bool SetStack (int newSubjectID, int newStackSize)
    {
        ItemSubject newSubject = masterSubjectList.GetSubject(newSubjectID, typeof(ItemSubject)) as ItemSubject;

        if (newSubject != null)
        {
            subjectID = newSubjectID;
            stackSize = Math.Min(newStackSize, newSubject.MaxStack);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Add will return the number remaining that would not fit based on ItemSubject.maxStack
    /// </summary>
    /// <param name="newSubjectID"></param>
    /// <param name="newStackSize"></param>
    /// <returns></returns>
    public int Add(int newSubjectID, int newStackSize)
    {
        if (newSubjectID != subjectID) return newStackSize; //reject entire amount

        ItemSubject newSubject = masterSubjectList.GetSubject(newSubjectID, typeof(ItemSubject)) as ItemSubject;

        if (newSubject != null)
        {
            int tempStack = stackSize + newStackSize;
            stackSize = Math.Min(tempStack, newSubject.MaxStack);
            newStackSize = Math.Max(0, tempStack - newSubject.MaxStack);
        }
        return newStackSize;

    }

    /// <summary>
    /// Override for Add takes an Inventory Item to add to this Inventory Item and returns amount that would not fit
    /// </summary>
    /// <param name="addedInvItem"></param>
    /// <returns></returns>
    public int Add(InventoryItem addedInvItem)
    {
        if (addedInvItem.subjectID != subjectID) return addedInvItem.stackSize; //reject entire amount

        ItemSubject newSubject = masterSubjectList.GetSubject(addedInvItem.subjectID, typeof(ItemSubject)) as ItemSubject;

        if (newSubject != null)
        {
            int tempStack = stackSize + addedInvItem.stackSize;
            stackSize = Math.Min(tempStack, newSubject.MaxStack);
            addedInvItem.stackSize = Math.Max(0, tempStack - newSubject.MaxStack);
        }
        return addedInvItem.stackSize;

    }

    /// <summary>
    /// Take subtracts the number of needed Items from this stack and returns the amount successfuly grabbed
    /// </summary>
    /// <param name="neededInvItem"></param>
    /// <returns></returns>
    public int Take(InventoryItem neededInvItem)
    {
        if (neededInvItem.subjectID != subjectID) return 0; //no amount pulled

        int amountTaken = Math.Min(neededInvItem.stackSize, stackSize);

        stackSize -= amountTaken;
        if (stackSize < 1) subjectID = -1;

        return amountTaken;
    }

    /// <summary>
    /// Simple tool to check if we already have this subject on the list
    /// </summary>
    /// <param name="listToCheck"></param>
    /// <returns></returns>
    public bool IsOnList(InventoryItem[] listToCheck)
    {
        for (int i = 0; i < listToCheck.Length; i++)
        {
            if (listToCheck[i].SubjectID == subjectID) return true;
        }

        return false;
    }
}


