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
    public void AddDisk(Vector3 centerPoint, float radius, int sides, Quaternion relativeRotation, Vector3 relativePosition)
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
            verts.Add((relativeRotation * centerPoint) + relativePosition);
            verts.Add((relativeRotation * (lastVert * radius + centerPoint)) + relativePosition);
            verts.Add((relativeRotation * (nextVert * radius + centerPoint)) + relativePosition);
            uvs.Add(new Vector2(0.5f, 0.5f));
            uvs.Add(new Vector2(lastVert.x / 2 + 0.5f, lastVert.z / 2 + 0.5f));
            uvs.Add(new Vector2(nextVert.x / 2 + 0.5f, nextVert.z / 2 + 0.5f));
            AddTriangle();
            lastVert = nextVert;
        }
        return;
    }

    /// <summary>
    /// AddSplitTube takes a base node and draws out to to children
    /// </summary>
    /// <param name="baseNode"></param>
    /// <param name="apexOne"></param>
    /// <param name="apexTwo"></param>
    /// <param name="sides"></param>
    /// <param name="relativeRotation"></param>
    public void AddSplitTube(Node baseNode, Node apexOne, Node apexTwo, int sides, Quaternion relativeRotation, Vector3 relativePosition)
    {
        Quaternion rotStep = new Quaternion();
        Quaternion centerRot = new Quaternion();
        Vector3 lastCenter = new Vector3();
        Vector3 lastVert = new Vector3();
        Vector3 nextCenter = new Vector3();
        Vector3 nextVert = new Vector3();
        float texStep = 1.0f / sides;
        float oneRad = apexOne.Radius * apexOne.Scale;
        float twoRad = apexTwo.Radius * apexTwo.Scale;
        float baseRad = baseNode.Radius * baseNode.Scale;
        Vector3 onePos = apexOne.ScaledPosition(baseNode.Scale);
        Vector3 twoPos = apexTwo.ScaledPosition(baseNode.Scale);


        rotStep = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        centerRot = Quaternion.AngleAxis((360 / sides) * (sides - 1), onePos - twoPos);
        lastVert = rotStep * Vector3.forward;
        lastCenter = centerRot * Vector3.forward;
        for (int i = 0; i < Mathf.FloorToInt(sides / 2); i++)
        {
            rotStep = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotStep * Vector3.forward;
            verts.Add(relativePosition + (relativeRotation * (apexOne.Rotation * ((lastVert * oneRad) + onePos) )));
            verts.Add(relativePosition + (relativeRotation * (lastCenter * baseRad) ) );
            verts.Add(relativePosition + (relativeRotation * (nextVert * baseRad) ) );
            verts.Add(relativePosition + (relativeRotation * (apexOne.Rotation * ((nextVert * oneRad) + onePos) )));
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
            centerRot = Quaternion.AngleAxis((360 / sides) * i, onePos - twoPos);
            nextVert = rotStep * Vector3.forward;
            nextCenter = centerRot * Vector3.forward;
            verts.Add(relativePosition + (relativeRotation * (apexOne.Rotation * ((lastVert * oneRad) + onePos) )));
            verts.Add(relativePosition + (relativeRotation * (lastCenter * baseRad) ) );
            verts.Add(relativePosition + (relativeRotation * (nextCenter * baseRad) ) );
            verts.Add(relativePosition + (relativeRotation * (apexOne.Rotation * ((nextVert * oneRad) + onePos) )));
            uvs.Add(new Vector2(texStep * i, 1.0f));
            uvs.Add(new Vector2(texStep * i, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 0.0f));
            uvs.Add(new Vector2(texStep * i + texStep, 1.0f));
            AddQuadTriangles();
            lastVert = nextVert;
            lastCenter = nextCenter;
        }

        rotStep = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        centerRot = Quaternion.AngleAxis((360 / sides) * (sides - 1), twoPos - onePos);
        lastVert = rotStep * Vector3.forward;
        lastCenter = lastVert;
        for (int i = 0; i < Mathf.FloorToInt(sides / 2)+1; i++)
        {
            rotStep = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            centerRot = Quaternion.AngleAxis((360 / sides) * i, twoPos - onePos);
            nextVert = rotStep * Vector3.forward;
            nextCenter = centerRot * Vector3.forward;
            verts.Add(relativePosition + (relativeRotation * (apexTwo.Rotation * ((lastVert * twoRad) + twoPos) )) );
            verts.Add(relativePosition + (relativeRotation * (lastCenter * baseRad) ) );
            verts.Add(relativePosition + (relativeRotation * (nextCenter * baseRad) ) );
            verts.Add(relativePosition + (relativeRotation * (apexTwo.Rotation * ((nextVert * twoRad) + twoPos) )) );
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
            verts.Add(relativePosition + (relativeRotation * (apexTwo.Rotation * ((lastVert * twoRad) + twoPos) )) );
            verts.Add(relativePosition + (relativeRotation * (lastVert * baseRad) ) );
            verts.Add(relativePosition + (relativeRotation * (nextVert * baseRad) ) );
            verts.Add(relativePosition + (relativeRotation * (apexTwo.Rotation * ((nextVert * twoRad) + twoPos) )) );
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
    /// Creates and adds a Tube defined by the nodes and rotated by relativeRotation
    /// </summary>
    /// <param name="baseNode"></param>
    /// <param name="apexNode"></param>
    /// <param name="sides"></param>
    /// <param name="relativeRotation"></param>
    public void AddTaperTube(Node baseNode, Node apexNode, int sides, Quaternion relativeRotation, Vector3 relativePosition)
    {
        Quaternion rotation = new Quaternion();
        Vector3 lastVert = new Vector3();
        Vector3 nextVert = new Vector3();
        float apexRad = apexNode.Radius * apexNode.Scale;
        float baseRad = baseNode.Radius * baseNode.Scale;
        Vector3 apexPos = apexNode.ScaledPosition(baseNode.Scale);
        float texStep = 1.0f / sides;
        rotation = Quaternion.AngleAxis((360 / sides) * (sides - 1), Vector3.up);
        lastVert = rotation * Vector3.forward;
        for (int i = 0; i < sides; i++)
        {
            rotation = Quaternion.AngleAxis((360 / sides) * i, Vector3.up);
            nextVert = rotation * Vector3.forward;
            verts.Add(relativePosition + (relativeRotation * (apexNode.Rotation * (lastVert * apexRad) + apexPos) ));
            verts.Add(relativePosition + (relativeRotation * (lastVert * baseRad) ));
            verts.Add(relativePosition + (relativeRotation * (nextVert * baseRad) ));
            verts.Add(relativePosition + (relativeRotation * (apexNode.Rotation * (nextVert * apexRad) + apexPos) ));
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