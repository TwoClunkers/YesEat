﻿using System;
using UnityEngine;

public enum PlantTypes
{
    GroundCover = 0,
    Grass = 1,
    Bush = 2,
    Tree = 3,
    Branch = 4
}
/// <summary>
/// Plant Subject extends the Subject Class to describe plants for AI
/// </summary>

public class PlantSubject : Subject
{
    #region Private members
    private int produceID;
    private int produceTime;
    private float maxGrowth;
    private float growthRate;
    private float matureGrowth;
    private int inventorySize;
    private Node[] nodeList;
    private GameObject nodeAttachment;
    private float heightRatio;
    private float taperRatio;
    private PlantTypes plantType;
    private Gene plantGene;
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
        growthRate = 0.01f;
        matureGrowth = 1;
        inventorySize = 3;
        nodeList = new Node[0];
        nodeAttachment = null;
        heightRatio = 5.0f;
        taperRatio = 0.8f;
        plantType = PlantTypes.Bush;
        plantGene = new Gene();
        plantGene.CreateRandom(15);
        LootIDs = new int[0];
    }

    public PlantSubject(PlantSubject copyPlantSubject) : base(copyPlantSubject)
    {
        produceID = copyPlantSubject.produceID;
        produceTime = copyPlantSubject.produceTime;
        maxGrowth = copyPlantSubject.maxGrowth;
        growthRate = copyPlantSubject.growthRate;
        matureGrowth = copyPlantSubject.matureGrowth;
        inventorySize = copyPlantSubject.inventorySize;


        nodeList = new Node[copyPlantSubject.nodeList.Length];
        if (nodeList.Length > 0)
        {
            for (int i = 0; i < copyPlantSubject.nodeList.Length; i++)
            {
                nodeList[i] = copyPlantSubject.nodeList[i].GetNode();
            }
        }
        nodeAttachment = copyPlantSubject.nodeAttachment;
        heightRatio = copyPlantSubject.heightRatio;
        taperRatio = copyPlantSubject.taperRatio;
        plantType = copyPlantSubject.plantType;
        plantGene = copyPlantSubject.plantGene;
        LootIDs = new int[copyPlantSubject.LootIDs.Length];
        Array.Copy(copyPlantSubject.LootIDs, LootIDs, copyPlantSubject.LootIDs.Length);
    }

    /// <summary>
    /// Copy an existing PlantSubject.
    /// </summary>
    public override Subject Copy() { return new PlantSubject(this); }

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
    public float MaxGrowth
    {
        get { return maxGrowth; }
        set { maxGrowth = value; }
    }

    /// <summary>
    /// GrowthTime tells us when you can take a "step" in growth process
    /// </summary>
    public float GrowthRate
    {
        get { return growthRate; }
        set { growthRate = value; }
    }

    /// <summary>
    /// MatureTime tells us at which step we can start producing
    /// </summary>
    public float MatureGrowth
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
    public Node[] NodeList
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
    /// HeightRatio is a multiplier for height
    /// </summary>
    public float HeightRatio
    {
        get { return heightRatio; }
        set { heightRatio = value; }
    }

    public float TaperRatio
    {
        get { return taperRatio; }
        set { taperRatio = value; }
    }

    public PlantTypes PlantType
    {
        get { return plantType; }
        set { plantType = value; }
    }

    public Gene PlantGene
    {
        get { return plantGene; }
        set { plantGene = value; }
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


