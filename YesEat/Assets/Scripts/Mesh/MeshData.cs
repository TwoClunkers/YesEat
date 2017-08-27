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

    //public void AddSplitTube(Vector3 basePoint, float baseRadius, Vector3 apexOne, Vector3 apexTwo, float apexOneRadius, float apexTwoRadius, int sides, Quaternion relativeRotation)
    //{
    //    Quaternion rotation = new Quaternion();
    //    Quaternion centerRot = new Quaternion();
    //    Vector3 lastCenter = new Vector3();
    //    Vector3 lastVert = new Vector3();
    //    Vector3 nextCenter = new Vector3();
    //    Vector3 nextVert = new Vector3();
    //    float texStep = 1.0f / sides;
    //    rotation = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
    //    centerRot = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.right);
    //    lastVert = rotation * Vector3.forward;
    //    lastCenter = centerRot * Vector3.forward;
    //    for (int i = 0; i < Mathf.FloorToInt(sides/2); i++)
    //    {
    //        rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
    //        nextVert = rotation * Vector3.forward;
    //        verts.Add(relativeRotation * (lastVert * apexOneRadius + apexOne));
    //        verts.Add(relativeRotation * (lastCenter * baseRadius + basePoint));
    //        verts.Add(relativeRotation * (nextVert * baseRadius + basePoint));
    //        verts.Add(relativeRotation * (nextVert * apexOneRadius + apexOne));
    //        uvs.Add(new Vector2(texStep * i, 1.0f));
    //        uvs.Add(new Vector2(texStep * i, 0.0f));
    //        uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
    //        uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
    //        AddQuadTriangles();
    //        lastVert = nextVert;
    //        lastCenter = nextVert;
    //    }
    //    lastCenter = lastVert;
    //    for (int i = Mathf.FloorToInt(sides / 2); i < sides; i++)
    //    {
    //        rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
    //        centerRot = Quaternion.AngleAxis((360 / sides) * i, Vector3.right);
    //        nextVert = rotation * Vector3.forward;
    //        nextCenter = centerRot * Vector3.forward;
    //        verts.Add(relativeRotation * (lastVert * apexOneRadius + apexOne));
    //        verts.Add(relativeRotation * (lastCenter * baseRadius + basePoint));
    //        verts.Add(relativeRotation * (nextCenter * baseRadius + basePoint));
    //        verts.Add(relativeRotation * (nextVert * apexOneRadius + apexOne));
    //        uvs.Add(new Vector2(texStep * i, 1.0f));
    //        uvs.Add(new Vector2(texStep * i, 0.0f));
    //        uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
    //        uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
    //        AddQuadTriangles();
    //        lastVert = nextVert;
    //        lastCenter = nextCenter;
    //    }

    //    rotation = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
    //    centerRot = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.left);
    //    lastVert = rotation * Vector3.forward;
    //    lastCenter = lastVert;
    //    for (int i = 0; i < Mathf.FloorToInt(sides / 2); i++)
    //    {
    //        rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
    //        centerRot = Quaternion.AngleAxis((360 / sides) * i, Vector3.left);
    //        nextVert = rotation * Vector3.forward;
    //        nextCenter = centerRot * Vector3.forward;
    //        verts.Add(relativeRotation * (lastVert * apexTwoRadius + apexTwo));
    //        verts.Add(relativeRotation * (lastCenter * baseRadius + basePoint));
    //        verts.Add(relativeRotation * (nextCenter * baseRadius + basePoint));
    //        verts.Add(relativeRotation * (nextVert * apexTwoRadius + apexTwo));
    //        uvs.Add(new Vector2(texStep * i, 1.0f));
    //        uvs.Add(new Vector2(texStep * i, 0.0f));
    //        uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
    //        uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
    //        AddQuadTriangles();
    //        lastVert = nextVert;
    //        lastCenter = nextCenter;
    //    }

    //    for (int i = Mathf.FloorToInt(sides / 2); i < sides; i++)
    //    {
    //        rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
    //        nextVert = rotation * Vector3.forward;
    //        verts.Add(relativeRotation * (lastVert * apexTwoRadius + apexTwo));
    //        verts.Add(relativeRotation * (lastVert * baseRadius + basePoint));
    //        verts.Add(relativeRotation * (nextVert * baseRadius + basePoint));
    //        verts.Add(relativeRotation * (nextVert * apexTwoRadius + apexTwo));
    //        uvs.Add(new Vector2(texStep * i, 1.0f));
    //        uvs.Add(new Vector2(texStep * i, 0.0f));
    //        uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
    //        uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
    //        AddQuadTriangles();
    //        lastVert = nextVert;
    //    }
    //    return;
    //}

    public void AddSplitTube(Node baseNode, Node apexOne, Node apexTwo, int sides)
    {
        Quaternion rotStep = new Quaternion();
        Quaternion centerRot = new Quaternion();
        Vector3 lastCenter = new Vector3();
        Vector3 lastVert = new Vector3();
        Vector3 nextCenter = new Vector3();
        Vector3 nextVert = new Vector3();
        float texStep = 1.0f / sides;
        rotStep = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        centerRot = Quaternion.AngleAxis((360 / sides) * (sides - 1), apexOne.Position-apexTwo.Position);
        lastVert = rotStep * Vector3.forward;
        lastCenter = centerRot * Vector3.forward;
        for (int i = 0; i < Mathf.FloorToInt(sides / 2); i++)
        {
            rotStep = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotStep * Vector3.forward;
            verts.Add( (apexOne.Rotation * (lastVert * apexOne.Radius + apexOne.Position)));
            verts.Add( (lastCenter * baseNode.Radius));
            verts.Add( (nextVert * baseNode.Radius));
            verts.Add( (apexOne.Rotation * (nextVert * apexOne.Radius + apexOne.Position)));
            uvs.Add(new Vector2(texStep * i, 1.0f));
            uvs.Add(new Vector2(texStep * i, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            AddQuadTriangles();
            lastVert = nextVert;
            lastCenter = nextVert;
        }
        lastCenter = lastVert;
        for (int i = Mathf.FloorToInt(sides / 2); i < sides; i++)
        {
            rotStep = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            centerRot = Quaternion.AngleAxis((360 / sides) * i, apexOne.Position - apexTwo.Position);
            nextVert = rotStep * Vector3.forward;
            nextCenter = centerRot * Vector3.forward;
            verts.Add( (apexOne.Rotation * (lastVert * apexOne.Radius + apexOne.Position)));
            verts.Add( (lastCenter * baseNode.Radius));
            verts.Add( (nextCenter * baseNode.Radius));
            verts.Add( (apexOne.Rotation * (nextVert * apexOne.Radius + apexOne.Position)));
            uvs.Add(new Vector2(texStep * i, 1.0f));
            uvs.Add(new Vector2(texStep * i, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            AddQuadTriangles();
            lastVert = nextVert;
            lastCenter = nextCenter;
        }

        rotStep = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        centerRot = Quaternion.AngleAxis((360 / sides) * (sides - 1), apexTwo.Position - apexOne.Position);
        lastVert = rotStep * Vector3.forward;
        lastCenter = lastVert;
        for (int i = 0; i < Mathf.FloorToInt(sides / 2)+1; i++)
        {
            rotStep = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            centerRot = Quaternion.AngleAxis((360 / sides) * i, apexTwo.Position - apexOne.Position);
            nextVert = rotStep * Vector3.forward;
            nextCenter = centerRot * Vector3.forward;
            verts.Add( (apexTwo.Rotation * (lastVert * apexTwo.Radius + apexTwo.Position )));
            verts.Add( (lastCenter * baseNode.Radius));
            verts.Add( (nextCenter * baseNode.Radius));
            verts.Add( (apexTwo.Rotation * (nextVert * apexTwo.Radius + apexTwo.Position )));
            uvs.Add(new Vector2(texStep * i, 1.0f));
            uvs.Add(new Vector2(texStep * i, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            AddQuadTriangles();
            lastVert = nextVert;
            lastCenter = nextCenter;
        }

        for (int i = Mathf.FloorToInt(sides / 2); i < sides; i++)
        {
            rotStep = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotStep * Vector3.forward;
            verts.Add( (apexTwo.Rotation * (lastVert * apexTwo.Radius + apexTwo.Position )));
            verts.Add( (lastVert * baseNode.Radius));
            verts.Add( (nextVert * baseNode.Radius));
            verts.Add( (apexTwo.Rotation * (nextVert * apexTwo.Radius + apexTwo.Position )));
            uvs.Add(new Vector2(texStep * i, 1.0f));
            uvs.Add(new Vector2(texStep * i, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            AddQuadTriangles();
            lastVert = nextVert;
        }
        return;
    }

    /// <summary>
    /// Creates and adds a tube to this mesh. Use relativeRotation to set direction of apex.
    /// </summary>
    /// <param name="baseNode"></param>
    /// <param name="apexNode"></param>
    /// <param name="sides"></param>
    public void AddTaperTube(Node baseNode, Node apexNode, int sides)
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
            verts.Add( apexNode.Rotation * (lastVert * apexNode.Radius + apexNode.Position));
            verts.Add( (lastVert * baseNode.Radius + baseNode.Position));
            verts.Add( (nextVert * baseNode.Radius + baseNode.Position));
            verts.Add(apexNode.Rotation * (nextVert * apexNode.Radius + apexNode.Position));
            uvs.Add(new Vector2(texStep * i, 1.0f));
            uvs.Add(new Vector2(texStep * i, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            AddQuadTriangles();
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

        for (int i = 1; i < steps; i++)
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