using System;
using UnityEngine;

/// <summary>
/// Item contained in an Inventory container.
/// </summary>
public class InventoryItem
{
    #region Private members
    private int subjectID;
    private int quantity;
    #endregion

    /// <summary>
    /// Empty Inventory Item
    /// </summary>
    public InventoryItem()
    {
        subjectID = 0;
        quantity = 0;
    }

    /// <summary>
    /// Copy an InventoryItem.
    /// </summary>
    /// <param name="copyInventoryItem">The InventoryItem to copy.</param>
    public InventoryItem(InventoryItem copyInventoryItem)
    {
        subjectID = copyInventoryItem.subjectID;
        quantity = copyInventoryItem.quantity;
    }

    /// <summary>
    /// InventoryItem constructor setting ID and size
    /// </summary>
    /// <param name="subjectID"></param>
    /// <param name="quantity"></param>
    public InventoryItem(int subjectID, int quantity)
    {
        this.subjectID = subjectID;
        this.quantity = quantity;
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
    public int Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }

    /// <summary>
    /// Redefines the item, limits quantity based on subject MaxStack.
    /// </summary>
    /// <param name="subjectID"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool Redefine(int subjectID, int quantity)
    {
        ItemSubject newSubject = MasterSubjectList.GetSubject(subjectID, typeof(ItemSubject)) as ItemSubject;

        if (newSubject != null)
        {
            this.subjectID = subjectID;
            this.quantity = Math.Min(quantity, newSubject.MaxStack);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Adds to the stack, limits based on the subject MaxStack
    /// </summary>
    /// <param name="subjectID"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public int Add(int subjectID, int quantity)
    {
        if (subjectID != this.subjectID) return quantity; //reject entire amount

        ItemSubject newSubject = MasterSubjectList.GetSubject(subjectID) as ItemSubject;

        if (newSubject != null)
        {
            int tempStack = this.quantity + quantity;
            this.quantity = Math.Min(tempStack, newSubject.MaxStack);
            quantity = Math.Max(0, tempStack - newSubject.MaxStack);
        }
        return quantity;

    }

    /// <summary>
    /// Adds to the stack, limits based on the subject MaxStack
    /// </summary>
    /// <param name="inventoryItem"></param>
    /// <returns></returns>
    public int Add(InventoryItem inventoryItem)
    {
        if (inventoryItem.subjectID != subjectID) return inventoryItem.quantity; //reject entire amount

        ItemSubject newSubject = MasterSubjectList.GetSubject(inventoryItem.subjectID) as ItemSubject;

        if (newSubject != null)
        {
            int tempStack = quantity + inventoryItem.quantity;
            quantity = Math.Min(tempStack, newSubject.MaxStack);
            inventoryItem.quantity = Math.Max(0, tempStack - newSubject.MaxStack);
        }
        return inventoryItem.quantity;

    }

    /// <summary>
    /// Take subtracts the number of needed Items from this stack and returns the amount successfuly grabbed
    /// </summary>
    /// <param name="inventoryItem"></param>
    /// <returns></returns>
    public int Take(InventoryItem inventoryItem)
    {
        if (inventoryItem.subjectID != subjectID) return 0; //no amount pulled

        int amountTaken = Math.Min(inventoryItem.quantity, quantity);

        quantity -= amountTaken;
        if (quantity < 1) subjectID = -1;

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


