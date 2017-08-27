using UnityEngine;
using System.Collections;

public class PlantObjectScript : SubjectObjectScript
{
    #region Private members
    protected float currentGrowth;
    private Inventory inventory;
    protected bool mature;
    protected float age;
    protected float lastProduce;
    protected int branchLevel;
    protected MeshFilter filter;
    protected MeshCollider coll;
    public PlantSubject plantSubject;
    public Vector3 apex;
    public MeshData meshData;
    public GameObject[] nodes;
    public Node baseNode;
    public bool meshChanged;
    #endregion
   
    // Use this for initialization
    void Awake()
    {
        plantSubject = new PlantSubject();
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
        branchLevel = 0;

        mature = false;
        age = 0.0f;
        currentGrowth = 0.01f;
        lastProduce = Time.time;
        baseNode = new Node();
        baseNode.Radius = 1.0f;

        meshChanged = true;
    }

    // Update is called once per frame
    void Update()
    {
        age += (Time.deltaTime*1.0f);
        //we grow
        if (currentGrowth < plantSubject.MaxGrowth)
        {
            if (age > (currentGrowth * plantSubject.GrowthRate))
            {
                GrowthStep();
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

        if (meshChanged) UpdateMesh();
    }

    public virtual void UpdateMesh()
    {
        MeshData meshData = new MeshData();
        if(branchLevel>0)
        {
            meshData.AddSplitTube(baseNode.GetScaledNode(), plantSubject.NodeList[0].GetScaledNode(), plantSubject.NodeList[1].GetScaledNode(), 20);
            meshData.AddDisk(plantSubject.NodeList[0].ScaledPosition(), plantSubject.NodeList[0].ScaledRadius(), 20, plantSubject.NodeList[0].Rotation);
            meshData.AddDisk(plantSubject.NodeList[1].ScaledPosition(), plantSubject.NodeList[1].ScaledRadius(), 20, plantSubject.NodeList[1].Rotation);
        }
        else
        {
            meshData.AddTaperTube(baseNode.GetScaledNode(), plantSubject.NodeList[0].GetScaledNode(), 20);
            meshData.AddDisk(plantSubject.NodeList[0].ScaledPosition(), plantSubject.NodeList[0].ScaledRadius(), 20, plantSubject.NodeList[0].Rotation);
        }
        RenderMesh(meshData);
    }

    // Sends the calculated mesh information
    // to the mesh and collision components
    public void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.verts.ToArray();
        filter.mesh.triangles = meshData.tris.ToArray();
        filter.mesh.uv = meshData.uvs.ToArray();
        filter.mesh.RecalculateNormals();

        coll.sharedMesh = filter.mesh;

        meshChanged = false;
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
    /// Grow a single step
    /// </summary>
    void GrowthStep()
    {
        currentGrowth += plantSubject.GrowthRate;
        meshChanged = true;

        baseNode.Scale = new Vector3(currentGrowth, currentGrowth * plantSubject.HeightRatio, currentGrowth);
        if(plantSubject.NodeList[0] != null)
        {
            plantSubject.NodeList[0].Scale = new Vector3(currentGrowth*plantSubject.TaperRatio, currentGrowth*plantSubject.HeightRatio, currentGrowth);
        }
        if (plantSubject.NodeList[1] != null)
        {
            plantSubject.NodeList[1].Scale = new Vector3(currentGrowth * plantSubject.TaperRatio, currentGrowth * plantSubject.HeightRatio, currentGrowth);
        }


        if (nodes.Length < 1) return;
        if (nodes.Length > 1)
        {
            if(branchLevel < 1)
            {
                nodes = new GameObject[1];
            }
        }
        if (plantSubject.NodeAttachment == null) return;
        
        //If this is a tree, the top node is reserved for a tree segment
        if (plantSubject.PlantType == PlantTypes.Tree)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null)
                {
                    if (branchLevel < 10 && mature)
                    {
                        nodes[i] = Instantiate(plantSubject.Prefab, this.transform);
                        nodes[i].transform.localPosition = plantSubject.NodeList[i].Rotation * plantSubject.NodeList[i].ScaledPosition();
                        nodes[i].transform.localRotation = plantSubject.NodeList[i].Rotation * Quaternion.Euler(0, 137.5f, 0);
                        PlantObjectScript script = nodes[i].GetComponent<PlantObjectScript>() as PlantObjectScript;
                        script.InitializeFromSubject(masterSubjectList, plantSubject);
                        script.branchLevel = branchLevel + 1;
                        Node newBase = plantSubject.NodeList[i].GetNode();
                        newBase.Position = new Vector3(0, 0, 0);
                        newBase.Rotation = Quaternion.identity;
                        newBase.Radius = 1.0f;
                        script.baseNode = newBase;
                    }
                    else
                    {
                        nodes[i] = Instantiate(plantSubject.NodeAttachment, this.transform);
                        nodes[i].transform.localPosition = plantSubject.NodeList[i].Rotation * plantSubject.NodeList[i].ScaledPosition();
                        nodes[i].transform.localRotation = plantSubject.NodeList[i].Rotation * Quaternion.Euler(0, 10.5f, 0); 
                    }
                }
                else
                {
                    nodes[i].transform.localPosition = plantSubject.NodeList[i].Rotation * plantSubject.NodeList[i].ScaledPosition();

                    LeafObjectScript leafScript = nodes[i].GetComponent<LeafObjectScript>() as LeafObjectScript;
                    if(leafScript != null)
                    {
                        leafScript.CurrentGrowth += Time.deltaTime * 0.5f;
                        leafScript.MeshChanged = true;
                        if(leafScript.CurrentGrowth > leafScript.MaxGrowth)
                        {
                            Destroy(nodes[i].gameObject);
                            nodes[i] = null;
                        }
                    }
                    else
                    {
                        PlantObjectScript plantScript = nodes[i].GetComponent<PlantObjectScript>() as PlantObjectScript;
                        plantScript.plantSubject.MaxGrowth = plantSubject.NodeList[i].ScaledRadius();
                    }
                }
            }

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

            inventory = new Inventory(plantSubject.InventorySize);
            mature = false;
            age = 0.1f;
            currentGrowth = 0.01f;
            lastProduce = Time.time;

            nodes = new GameObject[plantSubject.NodeList.Length];


        }
        else
        {
            //default values used if no valid subject for initialization
            
            plantSubject.ProduceID = 1;
            plantSubject.ProduceTime = 20;
            plantSubject.MaxGrowth = 1;
            plantSubject.GrowthRate = 20;
            plantSubject.MatureGrowth = 15;

            inventory = new Inventory(3);
            mature = false;
            age = 0.1f;
            currentGrowth = 5.0f;
            lastProduce = Time.time;
        }
        //gameObject.transform.localScale = new Vector3(currentGrowth * 0.01f + 0.5f, currentGrowth * 0.02f + 0.5f, currentGrowth * 0.01f + 0.5f);
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
}
