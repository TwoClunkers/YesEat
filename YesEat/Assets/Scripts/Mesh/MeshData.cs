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
    /// Adds one triangle using the last 3 coordinates
    /// </summary>
    public void AddTriangle()
    {
        tris.Add(verts.Count - 3);
        tris.Add(verts.Count - 2);
        tris.Add(verts.Count - 1);
    }

    /// <summary>
    /// Adds 2 triangles at the same time (forming a quad) using the last 4 coordinates
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

    //---------Special Shapes ----------------//

    /// <summary>
    /// Creates and adds a disk to this mesh. Normal starts as "up". Use relativeRotation to set normal.
    /// </summary>
    /// <param name="centerPoint"></param>
    /// <param name="radius"></param>
    /// <param name="sides"></param>
    /// <param name="relativeRotation"></param>
    public void AddDisk(Vector3 centerPoint, float radius, int sides, Quaternion relativeRotation)
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
            verts.Add(relativeRotation * centerPoint);
            verts.Add(relativeRotation * (lastVert * radius + centerPoint));
            verts.Add(relativeRotation * (nextVert * radius + centerPoint));
            uvs.Add(new Vector2(0.5f, 0.5f));
            uvs.Add(new Vector2(lastVert.x / 2 + 0.5f, lastVert.z / 2 + 0.5f));
            uvs.Add(new Vector2(nextVert.x / 2 + 0.5f, nextVert.z / 2 + 0.5f));
            AddTriangle();
            lastVert = nextVert;
        }
        return;
    }

    /// <summary>
    /// Creates and adds a tube to this mesh. Use relativeRotation to set direction of apex.
    /// </summary>
    /// <param name="basePoint"></param>
    /// <param name="baseRadius"></param>
    /// <param name="apexPoint"></param>
    /// <param name="apexRadius"></param>
    /// <param name="sides"></param>
    /// <param name="relativeRotation"></param>
    public void AddTaperTube(Vector3 basePoint, float baseRadius, Vector3 apexPoint, float apexRadius, int sides, Quaternion relativeRotation)
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
            verts.Add(relativeRotation * (lastVert * apexRadius + apexPoint));
            verts.Add(relativeRotation * (lastVert * baseRadius + basePoint));
            verts.Add(relativeRotation * (nextVert * baseRadius + basePoint));
            uvs.Add(new Vector2(texStep * i, 1.0f));
            uvs.Add(new Vector2(texStep * i, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            AddTriangle();
            verts.Add(relativeRotation * (lastVert * apexRadius + apexPoint));
            verts.Add(relativeRotation * (nextVert * baseRadius + basePoint));
            verts.Add(relativeRotation * (nextVert * apexRadius + apexPoint));
            uvs.Add(new Vector2(texStep * i, 1.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            AddTriangle();
            lastVert = nextVert;
        }
        return;
    }

    /// <summary>
    /// Similar to a triangle fan, but the total angle is centered on "forward", normal is "up". Use relativeRotation to give facing direction
    /// </summary>
    /// <param name="basePoint"></param>
    /// <param name="radius"></param>
    /// <param name="steps"></param>
    /// <param name="angleStep"></param>
    /// <param name="relativeRotation"></param>
    public void AddTriangleArc(Vector3 basePoint, float radius, int steps, float angleStep, Quaternion relativeRotation)
    {
        Quaternion rotation = new Quaternion();
        Vector3 lastVert = new Vector3();
        Vector3 nextVert = new Vector3();
        float angleStart = angleStep * steps * (-0.5f);
        rotation = Quaternion.AngleAxis(angleStart, Vector3.up);
        lastVert = rotation * Vector3.forward;

        for (int i = 0; i < steps; i++)
        {
            rotation = Quaternion.AngleAxis(angleStart + angleStep * i, Vector3.up);
            nextVert = rotation * Vector3.forward;
            verts.Add(relativeRotation * basePoint);
            verts.Add(relativeRotation * (lastVert * radius + basePoint));
            verts.Add(relativeRotation * (nextVert * radius + basePoint));
            uvs.Add(new Vector2(0.5f, 0.5f));
            uvs.Add(new Vector2(0.5f + lastVert.x, 0.5f + lastVert.z));
            uvs.Add(new Vector2(0.5f + nextVert.x, 0.5f + nextVert.z));
            AddTriangle();
            lastVert = nextVert;
        }

        return;
    }

    /// <summary>
    /// AddCode is similar to AddDisk, the texture faces the apex.
    /// </summary>
    /// <param name="basePoint"></param>
    /// <param name="baseRadius"></param>
    /// <param name="apexPoint"></param>
    /// <param name="sides"></param>
    /// <param name="relativeRotation"></param>
    public void AddCone(Vector3 basePoint, float baseRadius, Vector3 apexPoint, int sides, Quaternion relativeRotation)
    {
        Quaternion rotation = new Quaternion();
        Vector3 lastVert = new Vector3();
        Vector3 nextVert = new Vector3();
        rotation = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        lastVert = rotation * (Vector3.forward * baseRadius);
        for (int i = 0; i < sides; i++)
        {
            rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotation * (Vector3.forward * baseRadius);
            verts.Add(relativeRotation * apexPoint);
            verts.Add(relativeRotation * (lastVert + basePoint));
            verts.Add(relativeRotation * (nextVert + basePoint));
            uvs.Add(new Vector2(0.5f, 0.5f));
            uvs.Add(new Vector2(lastVert.x / 2 + 0.5f, lastVert.z / 2 + 0.5f));
            uvs.Add(new Vector2(nextVert.x / 2 + 0.5f, nextVert.z / 2 + 0.5f));
            AddTriangle();
            lastVert = nextVert;
        }
        return;
    }

}