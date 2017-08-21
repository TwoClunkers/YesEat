using UnityEngine;
using System.Collections;

public class PlantObjectScript : SubjectObjectScript
{
    #region Private members
    private float currentGrowth;
    private Inventory inventory;
    private bool mature;
    private float age;
    private float lastProduce;
    private int branchLevel;
    private MeshFilter filter;
    private MeshCollider coll;
    PlantSubject plantSubject;
    public Vector3 apex;

    #endregion
   
    // Use this for initialization
    void Awake()
    {
        plantSubject = subject as PlantSubject;

        mature = false;
        age = 0.0f;
        currentGrowth = 0.01f;
        lastProduce = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime;
        //we grow
        if (currentGrowth < plantSubject.MaxGrowth)
        {
            if (age > (currentGrowth * plantSubject.GrowthRate))
                GrowthStep();
        }
        //we produce
        if (mature)
        {
            if ((Time.time - lastProduce) > plantSubject.ProduceTime)
            {
                ProduceStep();
                UpdateFullnessColor();
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

    public int BranchLevel
    {
        get { return branchLevel; }
        set { branchLevel = value; }
    }
    /// <summary>
    /// Harvest to take from inventory
    /// </summary>
    /// <returns></returns>
    public override InventoryItem Harvest()
    {
        return inventory.Take(new InventoryItem(plantSubject.ProduceID, 1));
    }


    /// <summary>
    /// Add our producable item to inventory
    /// </summary>
    void ProduceStep()
    {
        InventoryItem producedItem = new InventoryItem(plantSubject.ProduceID, 1);
        producedItem = inventory.Add(producedItem);
        lastProduce = Time.time;
    }

    /// <summary>
    /// Grow a single step
    /// </summary>
    void GrowthStep()
    {
        currentGrowth += 1;
        gameObject.transform.localScale = new Vector3(currentGrowth * 0.04f + 0.5f, currentGrowth * 0.03f + 0.5f, currentGrowth * 0.04f + 0.5f);
    }

    void UpdateFullnessColor()
    {
        transform.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.2F, 0.9F, 0.3F, 0.8F), new Color(0.8F, 0.2F, 0.8F, 0.8F), inventory.FillRatio());
    }

    /// <summary>
    /// Set this Plant Object's variables from the subject card
    /// </summary>
    /// <param name="_masterSubjectList"></param>
    /// <param name="newSubject"></param>
    public override void InitializeFromSubject(MasterSubjectList _masterSubjectList, Subject newSubject)
    {
        masterSubjectList = _masterSubjectList;
        subject = newSubject;
        if (newSubject is PlantSubject)
        {
            PlantSubject plantSubject = newSubject.Copy() as PlantSubject;

            inventory = new Inventory(plantSubject.InventorySize, _masterSubjectList);
            mature = false;
            age = 0.1f;
            currentGrowth = 0.01f;
            lastProduce = Time.time;

        }
        else
        {
            //default values used if no valid subject for initialization
            
            plantSubject.ProduceID = 1;
            plantSubject.ProduceTime = 20;
            plantSubject.MaxGrowth = 1;
            plantSubject.GrowthRate = 20;
            plantSubject.MatureGrowth = 15;

            inventory = new Inventory(3, _masterSubjectList);
            mature = false;
            age = 0.1f;
            currentGrowth = 5.0f;
            lastProduce = Time.time;
        }
        gameObject.transform.localScale = new Vector3(currentGrowth * 0.01f + 0.5f, currentGrowth * 0.02f + 0.5f, currentGrowth * 0.01f + 0.5f);
    }

}
