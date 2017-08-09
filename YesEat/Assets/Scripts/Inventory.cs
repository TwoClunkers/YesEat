using System;
using System.Collections.Generic;


class Inventory
{
    #region Private members
    private int size;
    private InventoryItem[] inventorySlots;
    private MasterSubjectList masterSubjectList;
    #endregion

    /// <summary>
    /// Constructor for an Inventory object. Must set the initial size in constructor.
    /// </summary>
    /// <param name="newSize"></param>
    public Inventory(int newSize, MasterSubjectList masterSubjectListRef) 
    {
        size = newSize;
        inventorySlots = new InventoryItem[size];
        for (int i = 0; i < size; i++)
        {
            inventorySlots[i] = new InventoryItem();
        }
        masterSubjectList = masterSubjectListRef;
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
        InventoryItem[] newInventorySlots = new InventoryItem[newSize];
        for (int i = 0; i < newInventorySlots.Length; i++)
        {
            //copy into the new array as much as will fit
            if (i < inventorySlots.Length)
            {
                newInventorySlots[i] = inventorySlots[i];
            }
            else newInventorySlots[i] = new InventoryItem();
        }
        //set the new array as the one for this object
        inventorySlots = newInventorySlots;

    }

    /// <summary>
    /// AddGroup will try to add each of the InventoryItem stacks into this object.
    /// </summary>
    /// <param name="newInventoryItems"></param>
    /// <returns></returns>
    public InventoryItem[] AddGroup(InventoryItem[] newInventoryItems)
    {

        for (int i = 0; i < newInventoryItems.Length; i++)
        {
            //attempt to take what we can from each of the items in 
            //the group
            newInventoryItems[i] = Add(newInventoryItems[i]);
        }

        //the array now holds what we could not take
        return newInventoryItems;
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
                int remainder = inventorySlots[i].Add(newInventoryItem, masterSubjectList);
                newInventoryItem.StackSize = remainder;
            }
            //If we still have some, and have an empty slot, we can make a new stack.
            if(inventorySlots[i].StackSize < 1)
            {
                inventorySlots[i].SubjectID = newInventoryItem.SubjectID;
                int remainder = inventorySlots[i].Add(newInventoryItem, masterSubjectList);
                newInventoryItem.StackSize = remainder;
            }
            //If the passed InventoryItem is empty, we can reset the subject id and break
            if(newInventoryItem.StackSize < 1)
            {
                newInventoryItem.SubjectID = -1;
                break;
            }
        }

        //the remainder is passed back
        return newInventoryItem;
    }

    /// <summary>
    /// CheckItem returns true if the quantaty of the neededItem can be found anywhere in this Inventory
    /// </summary>
    /// <param name="neededItem"></param>
    /// <returns></returns>
    public bool CheckItem(InventoryItem neededItem)
    {
        //to track how much we found so far
        int count = 0;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].SubjectID == neededItem.SubjectID)
            {
                count += inventorySlots[i].StackSize;
            }
            //If our needed stack is still more than count than keep going
            if (neededItem.StackSize > count)
                continue;
            else return true;
        }
        //if we are here, we never found the needed amount
        return false;
    }

    /// <summary>
    /// Take pulls matching items from inventory until it reaches the neededItem stacksize
    /// </summary>
    /// <param name="neededItem"></param>
    /// <returns></returns>
    public InventoryItem Take(InventoryItem neededItem)
    {
        //to hold what we have pulled so far
        int count = 0;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            count += inventorySlots[i].Take(neededItem);

            //If we got everything we need return the full neededItem
            if (neededItem.StackSize == count)
                return neededItem;
            else continue;
        }
        //if we are here, we did not get all we wanted, so return the amount we were able to take
        neededItem.StackSize = count;
        return neededItem;
    }

    /// <summary>
    /// TakeType will pull only items that match the Type, and can limit the number of unique items as well as stack size
    /// </summary>
    /// <param name="limitType"></param>
    /// <param name="limitStack"></param>
    /// <param name="limitOptions"></param>
    /// <returns></returns>
    public InventoryItem[] TakeType(Type limitType = null, int limitStack = 1, int limitOptions = 3)
    {
        InventoryItem[] reservedItems = new InventoryItem[limitOptions];

        int position = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].IsOnList(reservedItems)) continue;

            Subject newSubject = masterSubjectList.GetSubject(inventorySlots[i].SubjectID, limitType) as Subject;

            //if we have a non-null subject, than we found a matching type
            if(newSubject != null)
            {
                //this creates an InventoryItem to hold our specific request and pass results into our reservedItems list
                InventoryItem neededItem = new InventoryItem();
                neededItem.SubjectID = newSubject.SubjectID;
                neededItem.StackSize = limitStack;
                //this reduces our inventory and places it into the reserved list
                reservedItems[position] = Take(neededItem);
                position += 1;
                if (position < limitOptions) continue;
                else break;
            }
        }
        //fill remaining positions with null
        for (int i = position; i < reservedItems.Length; i++)
        {
            reservedItems[i] = null;
        }
        
        return reservedItems;
    }

    /// <summary>
    /// FillRatio gives percent of slots not empty
    /// </summary>
    /// <returns></returns>
    public float FillRatio()
    {
        float filled = 0.0f;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].StackSize > 0)
            {
                filled += 1.0f;
            }
                
        }
        return filled / (float)size;
    }

}