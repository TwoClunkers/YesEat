using UnityEngine;
using System.Collections.Generic;

public class MeshData
{

    public List<Vector3> verts = new List<Vector3>();
    public List<int> tris = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();

    public List<Vector3> colVerts = new List<Vector3>();
    public List<int> colTris = new List<int>();

    /// <summary>
    /// Adds one triangle
    /// </summary>
    public void AddTriangle()
    {
        tris.Add(verts.Count - 3);
        tris.Add(verts.Count - 2);
        tris.Add(verts.Count - 1);
    }

    public void AddTriangle(int first, int second, int third)
    {
        tris.Add(first);
        tris.Add(second);
        tris.Add(third);
    }

    /// <summary>
    /// Adds 2 triangles at the same time (forming a quad)
    /// </summary>
    public void AddQuadTriangles()
    {
        tris.Add(verts.Count - 4);
        tris.Add(verts.Count - 3);
        tris.Add(verts.Count - 2);
        tris.Add(verts.Count - 4);
        tris.Add(verts.Count - 2);
        tris.Add(verts.Count - 1);
    }
    
    /// <summary>
    /// Should clear all details out of the mesh data object
    /// </summary>
    public void Clear()
    {
        verts = new List<Vector3>();
        colVerts = new List<Vector3>();
        tris = new List<int>();
        colTris = new List<int>();
        uvs = new List<Vector2>();
    }
}