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
    private PlantSubject plantSubject;
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
        if (currentGrowth < plantSubject.MaxGrowth)
        {
            if (Mathf.FloorToInt(age) > (currentGrowth * plantSubject.GrowthTime))
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

    public float CurrentGrowth { get { return currentGrowth; } set { currentGrowth = value; } }

    public float InventoryPercent() { return Mathf.Round(inventory.FillRatio() * 100); }

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
        inventory.Add(new InventoryItem(plantSubject.ProduceID, 1));
        foreach (int lootId in plantSubject.LootIDs)
        {
            inventory.Add(new InventoryItem(lootId, 1));
        }
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
    /// <param name="newSubject"></param>
    public override void InitializeFromSubject(Subject newSubject)
    {
        subject = newSubject as PlantSubject;
        plantSubject = subject as PlantSubject;
        if (newSubject is PlantSubject)
        {
            inventory = new Inventory(plantSubject.InventorySize);

            mature = false;
            age = 0.1f;
            currentGrowth = 0.5f;
            lastProduce = Time.time;
        }
        else
        {
            //default values used if no valid subject for initialization
            inventory = new Inventory(3);

            mature = false;
            age = 0.1f;
            currentGrowth = 5.0f;
            lastProduce = Time.time;
        }
        gameObject.transform.localScale = new Vector3(currentGrowth * 0.01f + 0.5f, currentGrowth * 0.02f + 0.5f, currentGrowth * 0.01f + 0.5f);
    }

}
