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
    #endregion

    /// <summary>
    /// Empty Inventory Item
    /// </summary>
    public InventoryItem()
    {
        subjectID = 0;
        stackSize = 0;
    }


    /// <summary>
    /// InventoryItem constructor setting ID and size
    /// </summary>
    /// <param name="_subjectID"></param>
    /// <param name="_stackSize"></param>
    public InventoryItem(int _subjectID, int _stackSize)
    {
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
    /// Sets the stack, but limits based on subject MaxStack
    /// </summary>
    /// <param name="newSubjectID"></param>
    /// <param name="newStackSize"></param>
    /// <param name="masterSubjectList"></param>
    /// <returns></returns>
    public bool SetStack (int newSubjectID, int newStackSize, ref MasterSubjectList masterSubjectList)
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
    /// Adds to the stack, limits based on the subject MaxStack
    /// </summary>
    /// <param name="newSubjectID"></param>
    /// <param name="newStackSize"></param>
    /// <param name="masterSubjectList"></param>
    /// <returns></returns>
    public int Add(int newSubjectID, int newStackSize, ref MasterSubjectList masterSubjectList)
    {
        if (newSubjectID != subjectID) return newStackSize; //reject entire amount

        ItemSubject newSubject = masterSubjectList.GetSubject(newSubjectID) as ItemSubject;

        if (newSubject != null)
        {
            int tempStack = stackSize + newStackSize;
            stackSize = Math.Min(tempStack, newSubject.MaxStack);
            newStackSize = Math.Max(0, tempStack - newSubject.MaxStack);
        }
        return newStackSize;

    }

    /// <summary>
    /// Adds to the stack, limits based on the subject MaxStack
    /// </summary>
    /// <param name="addedInvItem"></param>
    /// <param name="masterSubjectList"></param>
    /// <returns></returns>
    public int Add(InventoryItem addedInvItem, ref MasterSubjectList masterSubjectList)
    {
        if (addedInvItem.subjectID != subjectID) return addedInvItem.stackSize; //reject entire amount

        ItemSubject newSubject = masterSubjectList.GetSubject(addedInvItem.subjectID) as ItemSubject;

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


