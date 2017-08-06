using System;
using UnityEngine;

/// <summary>
/// Animal Subject extends the Subject Class to describe Animals for AI
/// </summary>

public class AnimalSubject : Subject
{
    #region Private members
    private int maxGrowth;
    private int growthTime;
    private int matureTime;
    private int inventorySize;
    private NpcDefinition definition;
    #endregion

    public AnimalSubject() : base()
    {
        name = "Animal";
        description = "Moving Thing";
        icon = new Sprite();

        maxGrowth = 3;
        growthTime = 0;
        matureTime = 1;
    }

    /// <summary>
    /// Max Growth tells us at which step you stop growing
    /// </summary>
    public int MaxGrowth
    {
        get { return maxGrowth; }
        set { maxGrowth = value; }
    }

    /// <summary>
    /// GrowthTime tells us when you can take a "step" in growth process
    /// </summary>
    public int GrowthTime
    {
        get { return growthTime; }
        set { growthTime = value; }
    }

    /// <summary>
    /// MatureTime tells us at which step we can start producing
    /// </summary>

    public int MatureTime
    {
        get { return matureTime; }
        set { matureTime = value; }
    }

    public override void TeachNpc(NpcCore npcCharacter)
    {
        npcCharacter.Definition.Memories.Add(new SubjectMemory(subjectID, 0, 0));
    }

    /// <summary>
    /// Main definition information for NpcCharacter.
    /// </summary>
    public NpcDefinition Definition
    {
        get { return definition; }
        set { definition = value; }
    }

    /// <summary>
    /// InventorySize tells us how big the inventory is
    /// </summary>
    public int InventorySize
    {
        get { return inventorySize; }
        set { inventorySize = value; }
    }
}
