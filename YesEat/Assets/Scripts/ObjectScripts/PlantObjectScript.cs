using UnityEngine;
using System.Collections;

public class PlantObjectScript : SubjectObjectScript
{
    #region Private members
    private int produceID;
    private int produceTime;
    private int maxGrowth;
    private int growthTime;
    private int matureGrowth;
    private float currentGrowth;
    private Inventory inventory;
    private bool mature;
    private float age;
    private float lastProduce;
    #endregion

    // Use this for initialization
    void Start()
    {
        mature = false;
        age = 0.0f;
        currentGrowth = 1.0f;
        lastProduce = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime;
        if (currentGrowth < maxGrowth)
        {
            if (Mathf.FloorToInt(age) > (currentGrowth * growthTime))
                GrowthStep();
        }
        if (mature)
        {
            if ((Time.time - lastProduce) > produceTime)
            {
                ProduceStep();
            }
        }
        else
        {
            if (currentGrowth >= matureGrowth)
            {
                mature = true;
                lastProduce = Time.time;
            }
        }
        
    }

    /// <summary>
    /// Add our producable item to inventory
    /// </summary>
    void ProduceStep()
    {
        //InventoryItem producedItem = new InventoryItem(ref masterSubjectList, produceID, 1);
        //inventory.Add(producedItem);
        lastProduce = Time.time;
    }

    /// <summary>
    /// Grow a single step
    /// </summary>
    void GrowthStep()
    {
        currentGrowth += 1;
        gameObject.transform.localScale = new Vector3(currentGrowth * 0.01f + 0.5f, currentGrowth * 0.02f + 0.5f, currentGrowth * 0.01f + 0.5f);
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
            PlantSubject plantSubject = newSubject as PlantSubject;
            produceID = plantSubject.ProduceID;
            produceTime = plantSubject.ProduceTime;
            maxGrowth = plantSubject.MaxGrowth;
            growthTime = plantSubject.GrowthTime;
            matureGrowth = plantSubject.MatureGrowth;
            inventory = new Inventory(plantSubject.InventorySize, ref _masterSubjectList);

            mature = false;
            age = 0.0f;
            currentGrowth = 1.0f;
            lastProduce = Time.time;
        }
        else
        {
            produceID = 1;
            produceTime = 20;
            maxGrowth = 20;
            growthTime = 20;
            matureGrowth = 15;
            inventory = new Inventory(3, ref _masterSubjectList);

            mature = false;
            age = 0.0f;
            currentGrowth = 1.0f;
            lastProduce = Time.time;
        }

    }
    
}
