using UnityEngine;
using System.Collections;

public class MeshObject : MonoBehaviour
{
    MeshFilter filter;
    MeshCollider coll;
    public GameObject treePart;
    public GameObject nodeTop;
    public GameObject nodeSide;

    public Vector3 apex;
    public float baseRadius;
    public float apexRadius;
    public float baseRatio;
    public float apexRatio;
    public float growth;
    public float growthLimit;
    public bool isBase = true;
    public float growthSupply;
    public int childCount;
    public int branchLevel;
    public float growthRate;

    // Use this for initialization
    void Awake()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();

        growthRate = Random.value/10;
        growth = 0.1f;
        baseRadius = 0.1f;
        apexRadius = 0.1f;
        apexRatio = 0.2f;
        baseRatio = 0.32f;
        apex = new Vector3();
        growthLimit = Random.value + 0.2f;
        growthSupply = 0.001f;
        branchLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        childCount = transform.childCount;
        
        if(baseRadius < growthLimit)
        {
            growthSupply = (Time.deltaTime * growthRate);
            growth += growthSupply;

            baseRadius += growthSupply * baseRatio;
            apexRadius += growthSupply * apexRatio;
            apex.y = growth;
            
            UpdateChunk();

            if (nodeTop == null)
            {
                if ((baseRadius > 0.1f) && (branchLevel < 5)) 
                {
                    nodeTop = Instantiate(treePart, this.transform);
                    nodeTop.transform.localPosition = apex;
                    nodeTop.transform.localEulerAngles = new Vector3(0, 137.5f, 0);
                    nodeTop.GetComponent<MeshObject>().isBase = false;
                    nodeTop.GetComponent<MeshObject>().branchLevel = branchLevel + 1;
                    nodeTop.GetComponent<MeshObject>().growthRate = growthRate;
                }
            }
            else
            {
                nodeTop.transform.localPosition = apex;
                nodeTop.GetComponent<MeshObject>().growthLimit = apexRadius-0.01f;
            }

        }
          
    }


    void UpdateChunk()
    {
        MeshData meshData = new MeshData();
        meshData.AddTaperTube(Vector3.zero, baseRadius, apex, apexRadius, 15, Quaternion.identity );
        meshData.AddDisk(apex, apexRadius, 7, Quaternion.identity);
        meshData.AddCone(new Vector3(0, 0.1f + apexRadius, 0), apexRadius, new Vector3(0, apexRadius, 0), 10, Quaternion.Euler(-90, 0, 0));
        meshData.AddCone(new Vector3(0, -0.1f - apexRadius, 0), apexRadius, new Vector3(0, apexRadius, 0), 10, Quaternion.Euler(90, 0, 0));
        RenderMesh(meshData);
    }





    // Sends the calculated mesh information
    // to the mesh and collision components
    void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.verts.ToArray();
        filter.mesh.triangles = meshData.tris.ToArray();
        filter.mesh.uv = meshData.uvs.ToArray();
        filter.mesh.RecalculateNormals();
    }
}
