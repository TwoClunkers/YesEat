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
    private int growthRate;
    private int matureGrowth;
    private int inventorySize;
    private Vector3[] nodeList;
    private GameObject nodeAttachment;
    private float heightRatio;
    #endregion

    public PlantSubject() : base()
    {
        name = "Plant";
        description = "Growing Thing";
        icon = new Sprite();

        produceID = 0;
        produceTime = 0;
        maxGrowth = 3;
        growthRate = 0;
        matureGrowth = 1;
        inventorySize = 3;
        nodeList = null;
        nodeAttachment = null;
        heightRatio = 1.0f;
    }

    /// <summary>
    /// Copy an existing PlantSubject.
    /// </summary>
    public override Subject Copy()
    {
        PlantSubject newPlantSubject = new PlantSubject();
        newPlantSubject.subjectID = subjectID;
        newPlantSubject.name = name;
        newPlantSubject.description = description;
        newPlantSubject.icon = icon;
        newPlantSubject.prefab = prefab;
        newPlantSubject.relatedSubjects = relatedSubjects;

        newPlantSubject.produceID = produceID;
        newPlantSubject.produceTime = produceTime;
        newPlantSubject.maxGrowth = maxGrowth;
        newPlantSubject.growthRate = growthRate;
        newPlantSubject.matureGrowth = matureGrowth;
        newPlantSubject.inventorySize = inventorySize;
        newPlantSubject.nodeList = nodeList;
        newPlantSubject.nodeAttachment = nodeAttachment;
        return newPlantSubject;
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
    public int GrowthRate
    {
        get { return growthRate; }
        set { growthRate = value; }
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

    /// <summary>
    /// Position of branches
    /// </summary>
    public Vector3[] NodeList
    {
        get { return nodeList; }
        set { nodeList = value; }
    }

    /// <summary>
    /// NodeAttachment is the gameObject used for new branches
    /// </summary>
    public GameObject NodeAttachment
    {
        get { return nodeAttachment; }
        set { nodeAttachment = value; }
    }

    /// <summary>
    /// NodeAttachment is the gameObject used for new branches
    /// </summary>
    public float HeightRatio
    {
        get { return heightRatio; }
        set { heightRatio = value; }
    }

    public override void TeachNpc(NpcCore npcCharacter)
    {
        npcCharacter.Definition.Memories.Add(new SubjectMemory(subjectID, 0, 0));
    }
}


