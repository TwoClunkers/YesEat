using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// In item sitting on the ground that can be picked up via Harvest().
/// </summary>
public class ItemObjectScript : SubjectObjectScript
{
    #region private members
    private InventoryItem inventoryItem;
    #endregion

    public InventoryItem Item { get { return inventoryItem; } set { inventoryItem = value; } }

    ItemObjectScript(ItemObjectScript copyItemObjectScript)
    {
        inventoryItem = new InventoryItem(copyItemObjectScript.inventoryItem);
        subject = copyItemObjectScript.subject.Copy();
        location = new LocationSubject(copyItemObjectScript.location);
    }

    public void Update()
    {
        if (inventoryItem != null)
            if (inventoryItem.Quantity <= 0) Destroy(this.gameObject);
    }

    public override InventoryItem Harvest(int itemIdToHarvest)
    {
        if (inventoryItem.SubjectID == itemIdToHarvest)
            return inventoryItem;
        else
            return new InventoryItem(itemIdToHarvest, 0);
    }

    public override void InitializeFromSubject(Subject newSubject)
    {
        subject = newSubject as AnimalSubject;
    }
}
