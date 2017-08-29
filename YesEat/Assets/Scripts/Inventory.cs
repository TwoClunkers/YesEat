using System;
using System.Collections.Generic;


public class Inventory
{
    #region Private members
    private int slotCount;
    private InventoryItem[] inventorySlots;
    #endregion

    /// <summary>
    /// Constructor for an Inventory object. Must set the initial size in constructor.
    /// </summary>
    /// <param name="newSize"></param>
    public Inventory(int newSize)
    {
        slotCount = newSize;
        inventorySlots = new InventoryItem[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            inventorySlots[i] = new InventoryItem();
        }
    }

    /// <summary>
    /// Resize will allow Inventory to have size adjusted after creation
    /// If decreased in size, items in inventory will be lost
    /// </summary>
    /// <param name="newSize"></param>
    public void Resize(int newSize)
    {
        //create a new array of Inventory Slots that is the correct size
        slotCount = newSize;
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
    public InventoryItem[] Add(InventoryItem[] newInventoryItems)
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
            if (inventorySlots[i].SubjectID == newInventoryItem.SubjectID)
            {
                int remainder = inventorySlots[i].Add(newInventoryItem);
                newInventoryItem.Quantity = remainder;
            }
            //If we still have some, and have an empty slot, we can make a new stack.
            if (inventorySlots[i].Quantity < 1)
            {
                inventorySlots[i].SubjectID = newInventoryItem.SubjectID;
                int remainder = inventorySlots[i].Add(newInventoryItem);
                newInventoryItem.Quantity = remainder;
            }
            //If the passed InventoryItem is empty, we can reset the subject id and break
            if (newInventoryItem.Quantity < 1)
            {
                newInventoryItem.SubjectID = -1;
                break;
            }
        }

        //the remainder is passed back
        return newInventoryItem;
    }

    /// <summary>
    /// Returns true if the quantity of the neededItem can be found anywhere in this Inventory
    /// </summary>
    /// <param name="neededItem"></param>
    /// <returns></returns>
    public bool Contains(InventoryItem neededItem)
    {
        //to track how much we found so far
        int count = 0;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].SubjectID == neededItem.SubjectID)
            {
                count += inventorySlots[i].Quantity;
            }
            //If our needed stack is still more than count than keep going
            if (neededItem.Quantity > count)
                continue;
            else return true;
        }
        //if we are here, we never found the needed amount
        return false;
    }

    /// <summary>
    /// Count how many of items of subjectId are contained in this inventory.
    /// </summary>
    /// <param name="subjectId">The SubjectID to count.</param>
    /// <returns>Quantity of subjectId found.</returns>
    public int Count(int subjectId)
    {
        int foundItems = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].SubjectID == subjectId)
            {
                foundItems += inventorySlots[i].Quantity;
            }
        }
        return foundItems;
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
            if (neededItem.Quantity == count)
                return neededItem;
            else continue;
        }
        //if we are here, we did not get all we wanted, so return the amount we were able to take
        neededItem.Quantity = count;
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

            Subject newSubject = KnowledgeBase.GetSubject(inventorySlots[i].SubjectID, limitType) as Subject;

            //if we have a non-null subject, than we found a matching type
            if (newSubject != null)
            {
                //this creates an InventoryItem to hold our specific request and pass results into our reservedItems list
                InventoryItem neededItem = new InventoryItem();
                neededItem.SubjectID = newSubject.SubjectID;
                neededItem.Quantity = limitStack;
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
            if (inventorySlots[i].Quantity > 0)
            {
                filled += 1.0f;
            }

        }
        return filled / (float)slotCount;
    }

    internal List<InventoryItem> TakeAllItems()
    {
        List<InventoryItem> returnList = new List<InventoryItem>(inventorySlots);
        inventorySlots = new InventoryItem[slotCount];
        return returnList;
    }
}