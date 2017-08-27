using UnityEngine;
using System.Collections;

public class LeafObjectScript : SubjectObjectScript
{
    #region Private members
    private ItemSubject leafSubject;
    private float currentGrowth;
    private float maxGrowth;
    private bool meshChanged;
    private float length;
    private float width;
    protected MeshFilter filter;
    protected MeshCollider coll;
    #endregion
    // Use this for initialization
    void Awake()
    {
        leafSubject = new ItemSubject();
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
        currentGrowth = 0.1f;
        maxGrowth = 1.0f;
        meshChanged = true;
        length = 0.1f;
        width = 50.0f;
    }

    // Update is called once per frame
    void Update()
    {
        length = Mathf.Min(2.0f, currentGrowth * 1.0f);
        width = Mathf.Min(30.0f, currentGrowth * 15.0f);
        if (meshChanged) UpdateMesh();
    }

    public virtual void UpdateMesh()
    {
        MeshData meshData = new MeshData();
        meshData.AddTriangleArc(new Vector3(0, 0.1f, 0), length, 4, width, Quaternion.AngleAxis(15, Vector3.up));
        meshData.AddTriangleArc(new Vector3(0, 0.05f, 0), length, 4, width, Quaternion.AngleAxis(-15, Vector3.up));
        meshData.AddTriangleArc(new Vector3(0, 0.1f, 0), length, 4, width, Quaternion.AngleAxis(15, Vector3.down));
        meshData.AddTriangleArc(new Vector3(0, 0.05f, 0), length, 4, width, Quaternion.AngleAxis(-15, Vector3.down));
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

    public override void InitializeFromSubject(MasterSubjectList _masterSubjectList, Subject newSubject)
    {
        masterSubjectList = _masterSubjectList;
        subject = newSubject;
        if (newSubject is ItemSubject)
        {
            leafSubject = newSubject.Copy() as ItemSubject;

            


        }
        
    }

    /// <summary>
    /// Harvest this leaf
    /// </summary>
    /// <returns></returns>
    public override InventoryItem Harvest()
    {
        if (leafSubject != null)
        {
            return new InventoryItem(leafSubject.SubjectID, 1);
            //Destroy this leaf object
        }
        else return null;
    }

    public float CurrentGrowth
    {
        get { return currentGrowth; }
        set { currentGrowth = value; }
    }

    public float MaxGrowth
    {
        get { return maxGrowth; }
        set { maxGrowth = value; }
    }

    public bool MeshChanged
    {
        get { return meshChanged; }
        set { meshChanged = value; }
    }
}
