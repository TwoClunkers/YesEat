using System;
using UnityEngine;

/// <summary>
/// Inventory item to describe the 
/// </summary>
public class InventoryItem
{
    #region Private members
    private int itemID;
    private int stackSize;
    #endregion

    public InventoryItem ()
	{
	}

    /// <summary>
    /// The item ID in this slot. Used to get info from MasterSubjectList
    /// </summary>
    public int ItemID
    {
        get { return itemID; }
        set { itemID = value; }
    }

    /// <summary>
    /// This describes how large the stack is 
    /// </summary>
    public int StackSize
    {
        get { return stackSize;        }

        set { stackSize = value;        }
    }

    public int CombineItems(int id, int number, MasterSubjectList subList)
    {
        if (id == itemID) stackSize += number;
        else return number; //not the same Item

        ItemSubject tempSubject = subList.GetSubject(itemID) as ItemSubject;
        int max = tempSubject.MaxStack;

        if (stackSize > max) return stackSize - max;
        else return 0;

    }
   

}


