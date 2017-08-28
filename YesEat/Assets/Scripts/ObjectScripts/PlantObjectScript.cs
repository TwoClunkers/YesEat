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
    private float activityThrottle;
    private bool growthActive;
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
        baseNode.Radius = 0.8f;

        meshChanged = true;

        growthActive = true;
        activityThrottle = 10;
    }

    // Update is called once per frame
    void Update()
    {
        age += (Time.deltaTime*1.0f);

        if(growthActive)
        {
            if (age > activityThrottle)
            {
                growthActive = false;
                activityThrottle = age + 10;
            }

            if (currentGrowth < plantSubject.MaxGrowth)
            {
                GrowthStep();
            }

            NodeCycle();
        }
        else
        {
            if (age > activityThrottle)
            {
                growthActive = true;
                activityThrottle = age + 3;
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
    public override InventoryItem Harvest()
    {
        return inventory.Take(new InventoryItem(plantSubject.ProduceID, 1));
    }


    /// <summary>
    /// Add our producable item to inventory
    /// </summary>
    void ProduceStep()
    {
        if(plantSubject.ProduceID > 0)
        {
            InventoryItem producedItem = new InventoryItem(plantSubject.ProduceID, 1);
            producedItem = inventory.Add(producedItem);
            lastProduce = Time.time;
        }
        
    }

    void NodeCycle()
    {
        baseNode.Scale = new Vector3(currentGrowth, currentGrowth * plantSubject.HeightRatio, currentGrowth);
        for (int i = 0; i < plantSubject.NodeList.Length; i++)
        {
            if (plantSubject.NodeList[i] != null)
            {
                plantSubject.NodeList[i].Scale = baseNode.Scale;
            }
        }
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null) //reposition stuff on filled nodes
            {
                nodes[i].transform.localPosition = plantSubject.NodeList[i].Rotation * plantSubject.NodeList[i].ScaledPosition();

                LeafObjectScript leafScript = nodes[i].GetComponent<LeafObjectScript>() as LeafObjectScript;
                if (leafScript != null)
                {
                    if (leafScript.CurrentGrowth < leafScript.MaxGrowth)
                    {
                        leafScript.CurrentGrowth += Time.deltaTime * 0.1f;
                        leafScript.MeshChanged = true;
                    }
                }
                else
                {
                    PlantObjectScript plantScript = nodes[i].GetComponent<PlantObjectScript>() as PlantObjectScript;
                    plantScript.plantSubject.MaxGrowth = plantSubject.NodeList[i].ScaledRadius();
                }
            }
            else //grow leaves on open nodes
            {
                nodes[i] = Instantiate(plantSubject.NodeAttachment, this.transform);
                nodes[i].transform.localPosition = plantSubject.NodeList[i].Rotation * plantSubject.NodeList[i].ScaledPosition();
                nodes[i].transform.localRotation = plantSubject.NodeList[i].Rotation * Quaternion.Euler(0, 10.5f, 0);
            }
        }
    }

    /// <summary>
    /// Grow a single step
    /// </summary>
    void GrowthStep()
    {
        currentGrowth += (plantSubject.GrowthRate * Time.deltaTime);
        meshChanged = true;

        if (nodes.Length > 1)
        {
            if(branchLevel < 1)
            {
                //if we are the base branch, we only will have the above node.
                nodes = new GameObject[1];
            }
        }

        //tree segments on all
        if (plantSubject.PlantType == PlantTypes.Tree)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (branchLevel < 10 && mature)
                {
                    if (nodes[i] != null) //destroy leaves off of mature nodes
                    {
                        nodes[i].transform.localPosition = plantSubject.NodeList[i].Rotation * plantSubject.NodeList[i].ScaledPosition();

                        LeafObjectScript leafScript = nodes[i].GetComponent<LeafObjectScript>() as LeafObjectScript;
                        if (leafScript != null)
                        {
                            Destroy(nodes[i].gameObject);
                            nodes[i] = null;
                        }
                        else
                        {
                            PlantObjectScript plantScript = nodes[i].GetComponent<PlantObjectScript>() as PlantObjectScript;
                            plantScript.plantSubject.MaxGrowth = plantSubject.NodeList[i].ScaledRadius();
                        }
                    }
                    if (nodes[i] == null) //start branches off of open nodes
                    {
                        nodes[i] = Instantiate(plantSubject.Prefab, this.transform);
                        nodes[i].transform.localPosition = plantSubject.NodeList[i].Rotation * plantSubject.NodeList[i].ScaledPosition();
                        nodes[i].transform.localRotation = plantSubject.NodeList[i].Rotation * Quaternion.Euler(0, 137.5f, 0);
                        PlantObjectScript script = nodes[i].GetComponent<PlantObjectScript>() as PlantObjectScript;
                        script.InitializeFromSubject(masterSubjectList, plantSubject);
                        script.branchLevel = branchLevel + 1;
                        script.plantSubject.MaxGrowth = plantSubject.NodeList[i].ScaledRadius();
                        script.CurrentGrowth = currentGrowth;
                        Node newBase = plantSubject.NodeList[i].GetNode();
                        newBase.Position = new Vector3(0, 0, 0);
                        newBase.Rotation = Quaternion.identity;
                        newBase.Radius = 1.0f;
                        newBase.Scale = new Vector3(0.1f, 0.1f * plantSubject.HeightRatio, 0.1f);

                        script.baseNode = newBase;

                    }
                }
            }
        }
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
            plantSubject = newSubject.Copy() as PlantSubject;

            plantSubject.GrowthRate = plantSubject.PlantGene.Value(0.2f);
            plantSubject.MaxGrowth = plantSubject.PlantGene.Value(3.0f) + 0.2f;
            plantSubject.MatureGrowth = 0.01f;
            plantSubject.HeightRatio = plantSubject.PlantGene.Value(4.0f) + 0.3f;
            plantSubject.TaperRatio = 1.0f;
            plantSubject.InventorySize = plantSubject.PlantGene.Value(5);
            plantSubject.ProduceTime = plantSubject.PlantGene.Value(10);

            
            Node newNode = new Node();
            
            newNode.Radius = plantSubject.PlantGene.Value(0.8f) + 0.2f;
            float dist = plantSubject.PlantGene.Value(0.6f);
            newNode.Rotation = Quaternion.Euler(0, 0, plantSubject.PlantGene.Value(-60));
            newNode.Position = new Vector3(dist, 1.0f, 0);
            plantSubject.NodeList[0] = newNode.GetNode();

            newNode.Radius = plantSubject.PlantGene.Value(0.5f) + 0.1f;
            dist = plantSubject.PlantGene.Value(0.8f);
            newNode.Rotation = Quaternion.Euler(0, 0, plantSubject.PlantGene.Value(90));
            newNode.Position = new Vector3(-dist, dist/2, 0);
            plantSubject.NodeList[1] = newNode.GetNode();

            inventory = new Inventory(plantSubject.InventorySize, _masterSubjectList);
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

            inventory = new Inventory(3, _masterSubjectList);
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
