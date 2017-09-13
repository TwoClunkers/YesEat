using UnityEngine;
using System.Collections;

public class PlantObjectScript : SubjectObjectScript
{
    #region Private members
    protected float currentGrowth;
    private Inventory inventory;
    
    protected float age;
    protected float nextState;
    protected float lastProduce;
    protected MeshFilter filter;
    protected MeshCollider coll;
    public PlantSubject plantSubject;
    protected bool mature;
    protected bool growthActive;

    #endregion


    
    // Use this for initialization
    void Awake()
    {
        plantSubject = new PlantSubject();
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();

        mature = false;
        growthActive = true;
        nextState = 0.0f;
        age = 0.0f;
        currentGrowth = 0.01f;
        lastProduce = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        age += (Time.deltaTime*5.0f);

        if(growthActive)
        {
            if (age > nextState)
            {
                growthActive = false;
                nextState = age + 30;
            }

            if (currentGrowth < plantSubject.MaxGrowth)
            {
                currentGrowth += Time.deltaTime * plantSubject.GrowthRate * 3;
            }
        }
        else
        {
            if (age > nextState)
            {
                growthActive = true;
                nextState = age + 15;
            }
        }

        //we produce
        if (mature)
        {
            if ((Time.time - lastProduce) > plantSubject.ProduceTime)
            {
                ProduceStep();
            }
        }
        else
        {
            if (currentGrowth >= plantSubject.MatureGrowth)
            {
                mature = true;
                lastProduce = Time.time;
            }
        }
        //create our proceedural mesh

        if(growthActive)
        {
            ProcBase proc = gameObject.GetComponent<ProcBase>() as ProcBase;
            if (proc != null)
            {
                proc.m_GrowthIndex = currentGrowth;
                proc.Rebuild();
            }
        }
    }

    /// <summary>
    /// Harvest to take from inventory
    /// </summary>
    /// <returns></returns>
    public override InventoryItem Harvest(int itemIdToHarvest)
    {
        return inventory.Take(new InventoryItem(itemIdToHarvest, 1));
    }

    /// <summary>
    /// Add our producable items to inventory
    /// </summary>
    void ProduceStep()
    {
        if (plantSubject.ProduceID > 0)
        {
            inventory.Add(new InventoryItem(plantSubject.ProduceID, 1));
            foreach (int lootId in plantSubject.LootIDs)
            {
                inventory.Add(new InventoryItem(lootId, 1));
            }
            lastProduce = Time.time;
        }
    }

    /// <summary>
    /// Set this Plant Object's variables from the subject card
    /// </summary>
    /// <param name="newSubject"></param>
    public override void InitializeFromSubject(Subject newSubject)
    {
        subject = newSubject as PlantSubject;
        plantSubject = subject as PlantSubject;
        if (newSubject is PlantSubject)
        {
            plantSubject = newSubject.Copy() as PlantSubject;

            plantSubject.GrowthRate = plantSubject.PlantGene.Value(1.0f) + 0.5f;
            plantSubject.MaxGrowth = plantSubject.PlantGene.Value(2.0f) + 1.0f;
            plantSubject.MatureGrowth = 0.01f;
            plantSubject.HeightRatio = plantSubject.PlantGene.Value(4.0f) + 0.3f;
            plantSubject.InventorySize = plantSubject.PlantGene.Value(5);
            plantSubject.ProduceTime = plantSubject.PlantGene.Value(10);

            inventory = new Inventory(plantSubject.InventorySize);
            mature = false;
            age = 20f;
            currentGrowth = 0.01f;
            lastProduce = Time.time;

            transform.GetComponent<MeshRenderer>().material.color = plantSubject.PlantGene.GetColor();
        }
        else
        {
            //default values used if no valid subject for initialization
            
            plantSubject.ProduceID = 1;
            plantSubject.ProduceTime = 20;
            plantSubject.MaxGrowth = 1;
            plantSubject.GrowthRate = 20;
            plantSubject.MatureGrowth = 15;
            plantSubject.HeightRatio = 2.0f;

            inventory = new Inventory(3);
            mature = false;
            age = 20f;
            currentGrowth = 5.0f;
            lastProduce = Time.time;
        }

        if(plantSubject.PlantType == PlantTypes.GroundCover)
        {
            ProcGroundCover proc = gameObject.AddComponent<ProcGroundCover>();
            proc.AssignGene(plantSubject.PlantGene);
            proc.Rebuild();
        }
        else if (plantSubject.PlantType == PlantTypes.Grass)
        {
            ProcGrass proc = gameObject.AddComponent<ProcGrass>();
            proc.AssignGene(plantSubject.PlantGene);
            proc.Rebuild();
        }
        else if (plantSubject.PlantType == PlantTypes.Herb)
        {
            ProcHerb proc = gameObject.AddComponent<ProcHerb>();
            proc.AssignGene(plantSubject.PlantGene);
            proc.Rebuild();
        }
    }

    public float CurrentGrowth
    {
        get { return currentGrowth; }
        set { currentGrowth = value; }
    }

    public float InventoryPercent()
    {
        return inventory.FillRatio();
    }

}
