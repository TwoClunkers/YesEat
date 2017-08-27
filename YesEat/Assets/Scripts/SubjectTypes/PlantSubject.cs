using System;
using UnityEngine;

/// <summary>
/// Plant Subject extends the Subject Class to describe plants for AI
/// </summary>

public class PlantSubject : Subject
{
    #region Private members
    private int produceID;
    private int produceTime;
    private int maxGrowth;
    private int growthTime;
    private int matureGrowth;
    private int inventorySize;
    private int[] lootIDs;
    #endregion

    public PlantSubject() : base()
    {
        name = "Plant";
        description = "Growing Thing";
        icon = new Sprite();

        produceID = 0;
        produceTime = 0;
        maxGrowth = 3;
        growthTime = 0;
        matureGrowth = 1;
        inventorySize = 3;
        LootIDs = new int[0];
    }

    public PlantSubject(PlantSubject copyPlantSubject) : base(copyPlantSubject)
    {
        produceID = copyPlantSubject.produceID;
        produceTime = copyPlantSubject.produceTime;
        maxGrowth = copyPlantSubject.maxGrowth;
        growthTime = copyPlantSubject.growthTime;
        matureGrowth = copyPlantSubject.matureGrowth;
        inventorySize = copyPlantSubject.inventorySize;
        LootIDs = new int[copyPlantSubject.LootIDs.Length];
        Array.Copy(copyPlantSubject.LootIDs, LootIDs, copyPlantSubject.LootIDs.Length);
    }

    /// <summary>
    /// Copy an existing PlantSubject.
    /// </summary>
    public override Subject Copy()
    {
        return new PlantSubject(this);
    }

    /// <summary>
    /// What does this plant produce?
    /// </summary>
    public int ProduceID
    {
        get { return produceID; }
        set { produceID = value; }
    }

    /// <summary>
    /// How long does it take to produce anything?
    /// </summary>
    public int ProduceTime
    {
        get { return produceTime; }
        set { produceTime = value; }
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
    public int MatureGrowth
    {
        get { return matureGrowth; }
        set { matureGrowth = value; }
    }

    /// <summary>
    /// InventorySize tells us how big the inventory is
    /// </summary>
    public int InventorySize
    {
        get { return inventorySize; }
        set { inventorySize = value; }
    }

    public int[] LootIDs { get { return lootIDs; } set { lootIDs = value; } }

    public override void TeachNpc(NpcCore npcCharacter)
    {
        npcCharacter.Definition.Memories.Add(new SubjectMemory(subjectID, 0, 0));
        SubjectMemory produceMemory = npcCharacter.Definition.Memories.Find(o => o.SubjectID == produceID);
        if (produceMemory != null) produceMemory.AddSource(subjectID);
        if (LootIDs.Length > 0)
        {
            for (int i = 0; i < LootIDs.Length; i++)
            {
                SubjectMemory lootMemory = npcCharacter.Definition.Memories.Find(o => o.SubjectID == LootIDs[i]);
                if (lootMemory != null) lootMemory.AddSource(subjectID);
            }
        }
    }
}


