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
        currentGrowth = 0.5f;
        lastProduce = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime;
        //we grow
        if (currentGrowth < maxGrowth)
        {
            if (Mathf.FloorToInt(age) > (currentGrowth * growthTime))
                GrowthStep();
        }
        //we produce
        if (mature)
        {
            if ((Time.time - lastProduce) > produceTime)
            {
                ProduceStep();
                UpdateFullnessColor();
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

    public float CurrentGrowth
    {
        get { return currentGrowth; }
        set { currentGrowth = value; }
    }

    public float InventoryPercent()
    {
        return Mathf.Round(inventory.FillRatio() * 100);
    }


    /// <summary>
    /// Harvest to take from inventory
    /// </summary>
    /// <returns></returns>
    public override InventoryItem Harvest()
    {
        return inventory.Take(new InventoryItem(produceID, 1));
    }


    /// <summary>
    /// Add our producable item to inventory
    /// </summary>
    void ProduceStep()
    {
        InventoryItem producedItem = new InventoryItem(produceID, 1);
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
    public override void InitializeFromSubject(Subject newSubject)
    {
        subject = newSubject;
        if (newSubject is PlantSubject)
        {
            PlantSubject plantSubject = newSubject as PlantSubject;
            produceID = plantSubject.ProduceID;
            produceTime = plantSubject.ProduceTime;
            maxGrowth = plantSubject.MaxGrowth;
            growthTime = plantSubject.GrowthTime;
            matureGrowth = plantSubject.MatureGrowth;
            inventory = new Inventory(plantSubject.InventorySize);

            mature = false;
            age = 0.1f;
            currentGrowth = 0.5f;
            lastProduce = Time.time;
        }
        else
        {
            //default values used if no valid subject for initialization
            produceID = 1;
            produceTime = 20;
            maxGrowth = 1;
            growthTime = 20;
            matureGrowth = 15;
            inventory = new Inventory(3);

            mature = false;
            age = 0.1f;
            currentGrowth = 5.0f;
            lastProduce = Time.time;
        }
        gameObject.transform.localScale = new Vector3(currentGrowth * 0.01f + 0.5f, currentGrowth * 0.02f + 0.5f, currentGrowth * 0.01f + 0.5f);
    }

}
