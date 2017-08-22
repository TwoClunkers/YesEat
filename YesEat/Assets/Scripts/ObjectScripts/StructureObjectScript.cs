using UnityEngine;
using System.Collections;

public class StructureObjectScript : SubjectObjectScript
{
    #region Private members
    private Inventory inventory;

    #endregion


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Inventory Inventory { get { return inventory; } set { inventory = value; } }

    public float InventoryPercent()
    {
        return Mathf.Round(inventory.FillRatio() * 100);
    }

    /// <summary>
    /// Currently no harvesting allowed from structures.
    /// </summary>
    /// <returns></returns>
    public override InventoryItem Harvest() { return null; }

    /// <summary>
    /// Set this Plant Object's variables from the subject card
    /// </summary>
    /// <param name="newSubject"></param>
    public override void InitializeFromSubject(Subject newSubject)
    {
        subject = newSubject;
        if (newSubject is StructureSubject)
        {
            StructureSubject structureSubject = newSubject as StructureSubject;
            inventory = new Inventory(structureSubject.InventorySize);
        }
        else
        {
            //default values used if no valid subject for initialization
            inventory = new Inventory(0);
        }

    }

}
