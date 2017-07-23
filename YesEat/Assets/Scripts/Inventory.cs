using System;
using System.Collections.Generic;


class Inventory
{
    #region Private members
    private int size;
    private InventoryItem[] inventorySlots;
    #endregion

    /// <summary>
    /// Constructor for an Inventory object. Must set the initial size in constructor.
    /// </summary>
    /// <param name="newSize"></param>
    public Inventory(int newSize) 
    {
        size = newSize;
        inventorySlots = new InventoryItem[size];
    }

    /// <summary>
    /// Resize will allow Inventory to have size adjusted after creation
    /// If decreased in size, items in inventory will be lost
    /// </summary>
    /// <param name="newSize"></param>
    public void Resize(int newSize)
    {
        //create a new array of Inventory Slots that is the correct size
        size = newSize;
        InventoryItem[] newInventorySlots = new InventoryItem[size];
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            //copy into the new array as much as will fit
            if (i < size)
            {
                newInventorySlots[i] = inventorySlots[i];
            }
            else break;
        }
        //set the new array as the one for this object
        inventorySlots = newInventorySlots;

    }

    /// <summary>
    /// Add tries to put the contents of a single "InventoryItem" stack into this object.
    /// It will attempt to stack on matching piles and then empty slots.
    /// </summary>
    /// <param name="newInventoryItem"></param>
    /// <returns></returns>
    public InventoryItem Add(InventoryItem newInventoryItem)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            //If this slot matches, we can stack on top of it
            if(inventorySlots[i].SubjectID == newInventoryItem.SubjectID)
            {
                int remainder = inventorySlots[i].AddStack(newInventoryItem);
                newInventoryItem.StackSize = remainder;
            }
            //If we still have some, and have an empty slot, we can make a new stack.
            if(inventorySlots[i].SubjectID == 0)
            {
                int remainder = inventorySlots[i].AddStack(newInventoryItem);
                newInventoryItem.StackSize = remainder;
            }
            //If we the passed InventoryItem is empty, we can reset the subject id and break
            if(newInventoryItem.StackSize < 1)
            {
                newInventoryItem.SubjectID = -1;
                break;
            }
        }

        return newInventoryItem;
    }
}