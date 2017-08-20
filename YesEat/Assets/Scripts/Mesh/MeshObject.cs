using UnityEngine;
using System.Collections;

public class MeshObject : MonoBehaviour
{
    MeshFilter filter;
    MeshCollider coll;
    public GameObject treePart;
    public GameObject nodeTop;

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

    // Use this for initialization
    void Awake()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
        
        growth = 0.1f;
        baseRadius = 0.1f;
        apexRadius = 0.1f;
        apexRatio = 0.3f;
        baseRatio = 0.35f;
        apex = new Vector3();
        growthLimit = 1.0f;
        growthSupply = 0.001f;
        branchLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        childCount = transform.childCount;
        if(isBase)
        {
            growthSupply = (Time.deltaTime * 0.01f);
        }
        if(baseRadius < growthLimit)
        {
            growth += growthSupply;
            baseRatio -= (Time.deltaTime * 0.001f);
            apexRatio -= (Time.deltaTime * 0.001f);

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
                    nodeTop.GetComponent<MeshObject>().isBase = false;
                    nodeTop.GetComponent<MeshObject>().branchLevel = branchLevel + 1;
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
        meshData = AddTaperTube(Vector3.zero, baseRadius, apex, apexRadius, 7, meshData);
        meshData = AddDisk(apex, apexRadius, 7, meshData);
        RenderMesh(meshData);
    }

    public MeshData AddTaperTube(Vector3 basePoint, float baseRadius, Vector3 apexPoint, float apexRadius, int sides, MeshData meshData)
    {
        Quaternion rotation = new Quaternion();
        Vector3 lastVert = new Vector3();
        Vector3 nextVert = new Vector3();
        float texStep = 1.0f / sides;
        rotation = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        lastVert = rotation * Vector3.forward;
        for (int i = 0; i < sides; i++)
        {
            rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotation * Vector3.forward;
            meshData.verts.Add(lastVert * apexRadius + apexPoint);
            meshData.verts.Add(lastVert * baseRadius + basePoint);
            meshData.verts.Add(nextVert * baseRadius + basePoint);
            meshData.uvs.Add(new Vector2(texStep * i, 1.0f));
            meshData.uvs.Add(new Vector2(texStep * i, 0.0f));
            meshData.uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            meshData.AddTriangle();
            meshData.verts.Add(lastVert * apexRadius + apexPoint);
            meshData.verts.Add(nextVert * baseRadius + basePoint);
            meshData.verts.Add(nextVert * apexRadius + apexPoint);
            meshData.uvs.Add(new Vector2(texStep * i, 1.0f));
            meshData.uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            meshData.uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            meshData.AddTriangle();
            lastVert = nextVert;
        }
        return meshData;
    }

    public MeshData AddCylinder(Vector3 basePoint, Vector3 apexPoint, int sides, MeshData meshData)
    {
        meshData = AddDisk(apexPoint, 1.0f, sides, meshData);
        meshData = AddTube(basePoint, apexPoint, sides, meshData);
        meshData = AddDisk(basePoint, 1.0f, sides, meshData);
        return meshData;
    }

    public MeshData AddDisk(Vector3 centerPoint, float radius, int sides, MeshData meshData)
    {
        Quaternion rotation = new Quaternion();
        Vector3 lastVert = new Vector3();
        Vector3 nextVert = new Vector3();
        rotation = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        lastVert = rotation * Vector3.forward;
        for (int i = 0; i < sides; i++)
        {
            rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotation * Vector3.forward;
            meshData.verts.Add(centerPoint);
            meshData.verts.Add(lastVert*radius + centerPoint);
            meshData.verts.Add(nextVert*radius + centerPoint);
            meshData.uvs.Add(new Vector2(0.5f, 0.5f));
            meshData.uvs.Add(new Vector2(lastVert.x / 2 + 0.5f, lastVert.z / 2 + 0.5f));
            meshData.uvs.Add(new Vector2(nextVert.x / 2 + 0.5f, nextVert.z / 2 + 0.5f));
            meshData.AddTriangle();
            lastVert = nextVert;
        }
        return meshData;
    }

    public MeshData AddTube(Vector3 basePoint, Vector3 apexPoint, int sides, MeshData meshData)
    {
        Quaternion rotation = new Quaternion();
        Vector3 lastVert = new Vector3();
        Vector3 nextVert = new Vector3();
        float texStep = 1.0f / sides;
        rotation = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        lastVert = rotation * Vector3.forward;
        for (int i = 0; i < sides; i++)
        {
            rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotation * Vector3.forward;
            meshData.verts.Add(lastVert + apexPoint);
            meshData.verts.Add(lastVert + basePoint);
            meshData.verts.Add(nextVert + basePoint);
            meshData.uvs.Add(new Vector2(texStep * i, 1.0f));
            meshData.uvs.Add(new Vector2(texStep * i, 0.0f));
            meshData.uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            meshData.AddTriangle();
            meshData.verts.Add(lastVert + apexPoint);
            meshData.verts.Add(nextVert + basePoint);
            meshData.verts.Add(nextVert + apexPoint);
            meshData.uvs.Add(new Vector2(texStep * i, 1.0f));
            meshData.uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            meshData.uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            meshData.AddTriangle();
            lastVert = nextVert;
        }
        return meshData;
    }

    public MeshData AddCone(Vector3 basePoint, Vector3 apexPoint, int sides, MeshData meshData)
    {
        Quaternion rotation = new Quaternion();
        Vector3 lastVert = new Vector3();
        Vector3 nextVert = new Vector3();
        rotation = Quaternion.AngleAxis((360 / sides) * (sides-1), Vector3.up);
        lastVert = rotation * Vector3.forward;
        for (int i = 0; i < sides; i++)
        {
            rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotation * Vector3.forward;
            meshData.verts.Add(apexPoint);
            meshData.verts.Add(lastVert + basePoint);
            meshData.verts.Add(nextVert + basePoint);
            meshData.uvs.Add(new Vector2(0.5f, 0.5f));
            meshData.uvs.Add(new Vector2(lastVert.x/2 + 0.5f, lastVert.z/2 + 0.5f));
            meshData.uvs.Add(new Vector2(nextVert.x/2 + 0.5f, nextVert.z/2 + 0.5f));
            meshData.AddTriangle();
            lastVert = nextVert;
        }
        return meshData;
    }

    public MeshData AddTetrahedron(Vector3[] points, MeshData meshData)
    {
        meshData.verts.Add(points[0]);
        meshData.verts.Add(points[1]);
        meshData.verts.Add(points[2]);
        meshData.uvs.Add(new Vector2(0.25f, 0.4f));
        meshData.uvs.Add(new Vector2(0.5f, 0.0f));
        meshData.uvs.Add(new Vector2(0.0f, 0.0f));
        meshData.AddTriangle();

        meshData.verts.Add(points[0]);
        meshData.verts.Add(points[2]);
        meshData.verts.Add(points[3]);
        meshData.uvs.Add(new Vector2(0.25f, 0.4f));
        meshData.uvs.Add(new Vector2(0.5f, 0.8f));
        meshData.uvs.Add(new Vector2(0.75f, 0.4f));
        meshData.AddTriangle();

        meshData.verts.Add(points[0]);
        meshData.verts.Add(points[3]);
        meshData.verts.Add(points[1]);
        meshData.uvs.Add(new Vector2(0.25f, 0.4f));
        meshData.uvs.Add(new Vector2(0.75f, 0.4f));
        meshData.uvs.Add(new Vector2(0.5f, 0.0f));
        meshData.AddTriangle();

        meshData.verts.Add(points[1]);
        meshData.verts.Add(points[3]);
        meshData.verts.Add(points[2]);
        meshData.uvs.Add(new Vector2(0.5f, 0.0f));
        meshData.uvs.Add(new Vector2(0.75f, 0.4f));
        meshData.uvs.Add(new Vector2(1.0f, 0.0f));
        meshData.AddTriangle();

        return meshData;
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
